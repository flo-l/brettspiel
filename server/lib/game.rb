require 'securerandom'
require 'csv'

require_relative 'message'
require_relative 'message_formatting.rb'
require_relative 'character.rb'
require_relative 'event.rb'
require_relative 'location.rb'
require_relative 'player.rb'

# Game represents a game with all mutable data.
class Game
  include MessageFormatting

  STORE_PATH = File.expand_path('games/') + '/'
  FILE_EXTENSION = '.game'

  ## API for events:
  # This let's the given player be next
  def next_player=(player)
    # check if next player is actually a Player object
    raise ProgrammerError unless player.is_a? Player

    @next_player = player
  end

  attr_reader :id, :players, :characters, :locations, :current_player, :answers_buffer

  # most of the things in here is mutable by the events
  def initialize(pack_name)
    # id and if the game has already started
    @id = SecureRandom.hex
    @started = false

    # content pack management
    @pack_folder = File.expand_path('content/' << pack_name)
    @pack_file   = pack_name

    # player things
    @players = []
    @next_player = nil

    # responses for the client
    # the server is expected to clear the responses if it sent them
    @response = []

    # the current event, in case the game must be marshalled and loaded again during the execution
    # of the event code. Eg: during a question.
    @current_event = nil

    # Characters
    load_characters

    # Locations
    load_locations
  end

  ## API for the server:
  # Loads a Game with a given id
  def self.load(id)
    path = STORE_PATH + id + FILE_EXTENSION

    # Open the file or raise an exception
    begin
      source = File.open(path, 'r')
    rescue SystemCallError
      raise GameIdWrongError
    end

    # Load the game object
    game = Marshal.load(source)
  end

  # Saves the Game to disk
  def save!
    target = File.new(STORE_PATH + @id + FILE_EXTENSION, 'w')
    Marshal.dump(self, target)
    target.close
  end

  # This handles messages infinitely. It yields requests from the game and expects to be passed the response via Fiber.resume
  def main_loop(msg)
    loop do
      msg = Fiber.yield handle(msg)
    end
  end

  private

  # This handles a message and returns the response(s) in an array
  def handle(msg)
    raise ProgrammerError unless @response.empty? # expect response to be cleared

    if [:join, :start].member? msg.type.to_sym
      # Check that the game has not already started
      raise GameAlreadyStartedError if @started

      if msg.type.to_sym == :join
        join(msg.name)
      else
        start
      end
    elsif msg.type.to_sym == :answer
      raise InvalidMessageError
    elsif msg.type.to_sym == :move_request
      # Check that the game has already started
      raise GameNotStartedError if not @started

      # check if the mode is present and valid
      if msg.respond_to? :mode
        mode = msg.mode.to_sym

        # check if the mode is valid
        raise InvalidMessageError unless [:active, :passive].include? mode
      else
        raise InvalidMessageError
      end

      do_round(msg.player_id, msg.location_id, mode)
    else
      raise MessageTypeUnknownError, msg.type.to_sym
    end

    @response
  end

  ## Internal methods, not part of the event API
  # Add a player to the game
  def join(name)
    # Check if the players name is already taken
    raise PlayerNameTakenError if @players.any? { |player| player.name == name }

    # Player with name and id
    player = Player.new(name, self)
    @players << player
    message_registered(player.id, player.name)
  end

  # Start the game
  def start
    # Check if there has at least one player joined
    raise NoPlayerError if @players.empty?

    # The game has now started
    @started = true

    # Set the current player to the first player
    @current_player = @players.first

    # First player starts!
    message_next(@current_player.id)

    # Prepare the players
    @players.each { |player| player.prepare! }
  end

  # Starts a round of the game, given the player and a location they want to visit
  def do_round(player_id, location_id, mode)
    # check if the right player wants to move
    raise WrongPlayerMoveError unless @current_player.id == player_id

    # resolve the location_id
    location = @locations[location_id]
    raise LocationNotPresentError unless location

    # check if new location is reachable from the old location
    old_location = @current_player.current_location
    raise LocationNotReachableError unless (old_location.neighbour_ids + [old_location.id]).include? location.id

    # everything ok, move the player
    @current_player.current_location = location

    # select an event to occur
    @current_event = location.select_event(self, mode)

    # let it occur
    @current_event.prepare_and_occur!(self)

    # set next player etc.
    end_round
  end

  def end_round
    # set next player as @current_player
    set_next_player

    # ask the next player to do his turn
    message_next(@current_player.id)
  end

  # This sets the next player
  def set_next_player
    # has somebody changed the player's order?
    if @next_player
      # set current_player and reset next_player
      @current_player = @next_player
      @next_player = nil
      return
    end

    index = @players.find_index(@current_player) # of current player

    unless @current_player = @players[index+1]
      # next player unless we've had the last player playing last round, start over in that case
      @current_player = @players.first
    end
  end

  # content loading
  def load_characters
    @characters = {}

    path = File.expand_path(@pack_folder + '/characters/characters.csv')
    CSV.foreach(path, {col_sep: ";", headers: true, encoding: "UTF-8"}) do |row|
      id, name = row.to_hash.values
      @characters[id] = Character.new(id, name)
    end
  end

  def load_locations
    # key is id, value: [name, neighbour_ids, events]
    @locations = {}

    # load location ids, names and neighbours
    path = @pack_folder + '/locations/locations.csv'
    CSV.foreach(path, {col_sep: ";", encoding: "UTF-8"}) do |id, name, _, _, _, *neighbours|
      next if id == "id" #skip the header line

      @locations[id.to_i] = [name, neighbours.map(&:to_i)]
    end

    # add an empty ary for events
    @locations.each_value { |ary| ary << [] }

    Dir.open(@pack_folder + '/events/').each do |file|
      next unless file =~ /.rb$/
      require_relative @pack_folder + '/events/' + file
    end

    @locations.each do |id, (name, neighbour_ids, events)|
      # instantiate Location object with name, id and neighbour_ids and store it
      @locations[id] = Location.new(id, name, neighbour_ids)
    end

    # add events
    Event::events.each do |event|
      # setup event
      event_obj = event.new
      event_obj.setup!(self)

      if event.all?
        @locations.each_value { |location| location.events << event_obj }
      else
        locations = @locations.values.find_all { |loc| event.names.include? loc.name }
        locations.each { |location| location.events << event_obj }
      end
    end
  end
end

require 'securerandom'

require_relative 'message_formatting.rb'
require_relative 'character.rb'
require_relative 'event.rb'
require_relative 'location.rb'
require_relative 'player.rb'

# Content, yeah!
require_relative '../content/characters.rb'
require_relative '../content/locations.rb'
Dir.open('content/events/').each do |file|
  next unless file =~ /.rb$/
  require_relative '../content/events/' << file
end

# Game represents a game with all mutable data.
class Game
  include MessageFormatting

  STORE_PATH = File.expand_path('games/') + '/'
  FILE_EXTENSION = '.game'

  ## API for events:
  # This let's the given player be next
  attr_writer :next_player

  attr_reader :id, :players, :characters, :locations, :current_player, :answers_buffer

  # everything in here is mutable by the events
  def initialize
    # id and if the game has already started
    @id = SecureRandom.hex
    @started = false

    # player things
    @players = []
    @next_player = nil

    # responses for the client
    @response = []

    # answers (actually their keys) already given by the client
    @answers = []
    @answers_buffer = []

    # the current event, in case the game must be marshalled and loaded again during the execution
    # of the event code. Eg: during a question.
    @current_event = nil

    # Characters
    @characters = {}
    CHARACTERS_HASH.each { |id, name| @characters[id] = Character.new(name, id) }

    # Locations
    @locations = {}
    LOCATIONS_HASH.each do |id, ary|
      # instantiate Location object with name, id and neighbour_ids
      location = Location.new(ary[0], id, ary[1])

      # add events
      ary[2].each do |event|
        event = event.new
        event.setup!(self)
        location.events << event
      end

      # store the location object
      @locations[id] = location
    end
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

  # This handles a message and returns the response(s) in an array
  def handle(msg)
    # clear the response buffer
    @response.clear

    # in case an answer from the client is needed
    catch :stop do

      if [:join, :start].member? msg.type.to_sym
        # Check that the game has not already started
        raise GameAlreadyStartedError if @started

        if msg.type.to_sym == :join
          join(msg.name)
        else
          start
        end

      elsif [:answer, :move_request].member? msg.type.to_sym
        # Check that the game has already started
        raise GameNotStartedError if not @started

        if msg.type.to_sym == :answer      
          resume(msg.answer)
        else
          do_round(msg.player_id, msg.location_id)
        end

      else
        raise MessageTypeUnknownError, msg.type.to_sym
      end

    end

    @response
  end

  private
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

  # Resume execution of the event after a question
  def resume(answer_index) # 0-indexed
    # add the answer to the buffer
    answer_key = @options_buffer[answer_index]

    # check if the answer_index is out of bounds
    raise ImpossibleResponseError unless answer_index < @options_buffer.count

    @answers << answer_key

    # buffer so one can pop shift the answers atop the ary
    @answers_buffer = @answers.dup

    # resume the execution of the event code
    @current_event.prepare_and_occur!(self)

    # set next player etc.
    end_round
  end

  # Starts a round of the game, given the player and a location they want to visit
  def do_round(player_id, location_id)
    # check if the right player wants to move
    raise WrongPlayerMoveError unless @current_player.id == player_id

    # resolve the location_id
    location = @locations[location_id]
    raise LocationNotPresentError unless location

    # move the player
    @current_player.current_location = location

    # select an event to occur
    @current_event = location.select_event(self, mode=:investigate) #FIXME ask player for mode

    # let it occur
    @current_event.prepare_and_occur!(self)

    # set next player etc.
    end_round
  end

  def end_round
    # we don't need these anymore
    @answers.clear
    @answers_buffer.clear

    # set next player as @current_player
    set_next_player

    # ask the next player to do his turn
    message_next(@current_player.id)
  end

  # This sets the next player
  def set_next_player
    # has somebody changed the player's order?
    if @next_player
      player = @next_player #buffer
      @next_player = nil    #reset @next_player
      return player         #early return!! :P
    end

    index = @players.find_index(@current_player) # of current player

    unless @current_player = @players[index+1]
      # next player unless we've had the last player playing last round, start over in that case
      @current_player = @players.first
    end
  end
end

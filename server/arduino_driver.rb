require 'json'
require_relative './lib/game.rb'
require_relative './arduino_display.rb'
require_relative './arduino_serial.rb'

# Exceptions
class DriverError < StandardError; nil; end

class InvalidMessageError < DriverError; nil; end
class LackingGameIdError < InvalidMessageError; nil; end
class MessageTypeUnknownError < InvalidMessageError; nil; end

class UserInputError < DriverError; nil; end
class TooManyButtonsPressedError < UserInputError; nil; end
class GameIdWrongError < UserInputError; nil; end
class PlayerNameTakenError < UserInputError; nil; end
class WrongPlayerMoveError < UserInputError; nil; end
class ImpossibleResponseError < UserInputError; nil; end
class LocationNotPresentError < UserInputError; nil; end
class LocationNotReachableError < UserInputError; nil; end

class GameAlreadyStartedError < DriverError; nil; end
class GameNotStartedError < DriverError; nil; end
class NoPlayerError < DriverError; nil; end

class ProgrammerError < DriverError; nil; end

# this is a replacement for the Server, which uses an Arduino board for user I/O
# and can only manage one Game instance at a time
class Driver
  # init content pack stuff and set buttons
  CONTENT_PACK = Dir.open(File.expand_path 'content').find { |x| x =~ /dev/ }

  # button_number => action_number
  ACTION_BUTTONS = {
    1 => 1,
    2 => 2,
    3 => 3,
    4 => 4,
    5 => 5,
    6 => 6
  }  

  # button_number => location_id
  LOCATION_BUTTONS = {
    7 => 1,
    8 => 2,
    9 => 3
  }

  def initialize
    @arduino = FakeArduino.new
    @buttons = Array.new(@arduino.class::BUTTON_NUM, false)
    @display = FakeDisplay.new

    @game = Game.new(CONTENT_PACK)
  end

  # this blocks until some button(s) on the arduino were changed
  # returns an ary of arys of button_ids+state which changed
  def buttons_changed
    # read new button state
    new_buttons = @arduino.buttons_changed

    # replace all changed values with button id + new state
    changed = []
    @buttons.zip(new_buttons).each_with_index do |(x,y),i|
      if x != y
        # [id, new_state]
        changed << [i,y]
      end
    end

    @buttons = new_buttons
    changed
  end

  def start!
    @display.text("NEUES SPIEL")

    loop do
      name = @display.get_name
      break if name == ""

      request = {
        :type => :join,
        :name => name
      }.to_json

      begin
        @game.handle(Message.new(request))
      rescue DriverError => e
        p e
        next
      rescue => e
        p e
        exit -1
      end
    end

    start_request = { :type => :start }.to_json
    next_actions = [Message.new(start_request)]

    while !next_actions.empty? do
      current_action = next_actions.shift
      responses = @game.handle(current_action)

      responses.each do |res|
        next_action = handle(Message.new(res))
        next_actions << next_action if next_action
      end
      begin
        #TODO: move code in here
      rescue DriverError => e
        p e
        next
      rescue => e
        p e
        exit -1
      end
    end
  end

  # Begin of a round
  def message_next(player_id)
    name = @game.current_player.name
    @display.next_player(name)

    changed = []
    loop do
      # read which button(s) changed
      changed = buttons_changed

      # ignore buttons which don't map to locations
      changed.select! { |id,_| LOCATION_BUTTONS.keys.member? id }

      # ignore button releases
      changed.select! { |_, pressed| pressed }

      raise TooManyButtonsPressedError if changed.count > 1
      break unless changed.empty?
    end

    button_id, state = *changed[0]

    location_id = LOCATION_BUTTONS[button_id]
    raise LocationNotPresentError unless location_id

    response = {:type => "move_request"}
    response["player_id"]   = @game.current_player.id
    response["location_id"] = location_id
    response["mode"]        = "active" #TODO remove active/passive?
    Message.new(response.to_json)
  end

  # This asks the player a question and returns the selected option's *number* not index to the server
  def message_question(player_id, character_id, question, options)
    player_name = @game.players[player_id]
    character_name = @game.characters[character_id]
    selection = @display.question(player_name, character_name, question, options)

    response = {:type => "answer"}
    response["answer"] = selection
    Message.new(response.to_json)
  end

  # calls the right method for each game response
  def handle(msg)
    case msg.type.to_sym
      when :game_created
      then message_game_created msg.game_id

      when :registered
      then message_registered msg.player_id, msg.player_name

      when :text
      then message_text msg.text

      when :character
      then message_character msg.character_id, msg.text

      when :move
      then message_move msg.player_id, msg.location_id

      when :next
      then message_next msg.player_id

      when :question
      then message_question msg.player_id, msg.character_id, msg.question, msg.options

      when :honor
      then message_honor msg.player_id, msg.amount

      when :change_cards
      then message_change_cards msg.player_id, msg.swords, msg.shields, msg.supply

      when :item_gained
      then message_item_gained msg.player_id, msg.item_name

      when :item_lost
      then message_item_lost msg.player_id, msg.item_name

      when :win
      then message_win msg.player_id

      else raise NotImplementedError, "message :#{msg.type} is not implemented!!"
    end
  end

  ###############################################################################
  # All these methods just delegate info to the Display

  # A new game with id has successfully been created
  def message_game_created(id)
    # no-op
  end

  # This tells a player that they have been registered
  def message_registered(player_id, player_name)
    # for now a no-op, maybe later some reaction with Display
  end

  # This should output the text
  def message_text(text)
    @display.text text
    nil
  end

  # This should output the text said by character
  def message_character(character_id, text)
    name = @game.characters[character_id].name
    @display.character(name, text)
    nil
  end

  def message_move(player_id, location_id)
    player_name = @game.players[player_id].name
    location_name = @game.locations[location_id].name
    @display.move(player_name, location_name)
    nil
  end

  def message_honor(player_id, amount)
    name = @game.players[player_id].name
    @display.honor(name, amount)
    nil
  end

  def message_change_cards(player_id, swords, shields, supply)
    name = @game.players[player_id].name
    @display.change_cards(name, swords, shields, supply)
    nil
  end

  def message_item_gained(player_id, item_name)
    player_name = @game.players[player_id].name
    @display.item_gained(player_name, item_name)
    nil
  end

  def message_item_lost(player_id, item_name)
    player_name = @game.players[player_id].name
    @display.item_lost(player_name, item_name)
    nil
  end

  def message_win(player_id)
    name = @game.players[player_id].name
    @display.win(name)
    exit # THE END
  end
end

Driver.new.start!

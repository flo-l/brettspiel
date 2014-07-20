require 'socket'
require 'json'

# This module holds all methods for message processing
# TODO: integrate at least all tcp_* methods into client.rb
module TCPMessages
  def process(msg)
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
  # This is the command line frontend.

  # A new game with id has successfully been created
  def message_game_created(id)
    @game_id = id

    # Start with joining players after connecting
    # This method will get called again after each registered message is recieved
    join_player
  end

  # This tells a player that they have been registered
  def message_registered(player_id, player_name)
    puts "You have been registered as player \##{player_id}, #{player_name}!"
    # Create Player object with name from hacky @player_names
    @players << Player.new(@player_names.shift, player_id, @locations.values.first)

    # join the next player
    join_player
  end

  # This should output the text
  def message_text(text)
    puts text
  end

  # This should output the text said by character
  def message_character(character_id, text)
    puts "Charakter \##{character_id} sagt: \"#{text}\"."
  end

  def message_move(player_id, location_id)
    puts "Player \##{player_id} moved to location \##{location_id}."
  end

  # Begin of a round
  def message_next(player_id)
    @current_player = @players.find { |p| p.id == player_id }

    puts "It's Player \##{player_id}s turn!"

    response = {:type => "move_request"}
    response["game_id"]     = @game_id
    response["player_id"]   = @current_player.id
    response["location_id"] = ask_next_location_id
    @socket.puts response.to_json
  end

  # This asks the player a question and returns the selected option's *number* not index to the server
  def message_question(player_id, character_id, question, options)
    puts "question: from character \##{character_id} to player \##{player_id}: #{question}"
    options.each_with_index { |option,i| puts "(#{i+1}) => #{option}" }

    print "Auswahl: "
    auswahl = gets.chomp.to_i

    response = {:type => "answer"}
    response["game_id"]     = @game_id
    response["answer"] = auswahl - 1
    @socket.puts response.to_json
  end

  def message_honor(player_id, amount)
    puts "Player \##{player_id}s honor changes by #{amount}"
  end

  def message_change_cards(player_id, swords, shields, supply)
    puts "Player \##{player_id} gains #{swords} sword(s), #{shields} shield(s) and #{supply} supply"
  end

  def message_item_gained(player_id, item_name)
    puts "Player \##{player_id} gains the item #{item_name}"
  end

  def message_item_lost(player_id, item_name)
    puts "Player \##{player_id} lost the item #{item_name}"
  end

  def message_win(player_id)
    puts "Player \##{player_id} won the game!"
    exit # THE END
  end
end

class Player
  attr_reader :name, :id
  attr_accessor :current_location

  def initialize(name, id, current_location)
    @name = name
    @id = id
    @current_location = current_location
  end

  def to_s
    @name
  end
end

class Character
  attr_reader :name, :id

  def initialize(name, id)
    @name = name
    @id = id
  end

  def to_s
    @name
  end
end

class Location
  attr_reader :name, :id, :neighbour_ids
  attr_accessor :events

  def initialize(name, id, neighbour_ids)
    @name = name
    @id = id
    @neighbour_ids = neighbour_ids

    @events = []
  end

  # returns all neighbours as objects
  def neighbours(locations)
    neighbour_ids.map { |id| locations[id] }
  end

  def to_s
    @name
  end
end

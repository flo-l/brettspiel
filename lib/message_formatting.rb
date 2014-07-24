require 'json'

# Message sending made easy
# This Module provides methods for message formatting
module MessageFormatting
  # This tells a player that they have been registered
  def message_registered(player_id, player_name)
    @response << {:type => "registered", "player_id" => player_id, "player_name" => player_name, "content_pack" => @pack_file}.to_json
  end

  # This should output the text
  def message_text(text)
    @response << {:type => "text", "text" => text}.to_json
  end

  # This should output the text said by character
  def message_character(character_id, text)
    @response << {:type => "character", "character_id" => character_id, "text" => text}.to_json
  end

  # A character has moved
  def message_move(player_id, location_id)
    @response << {:type => "move", "player_id" => player_id, "location_id" => location_id}.to_json
  end

  # Next player
  def message_next(player_id)
    @response << {:type => "next", "player_id" => player_id}.to_json
  end

  # This asks the player a question and should be able to give back
  # the selected option's *key*, even though this must happen somewhere else,
  # because the request ends before the answer is recieved of course.
  #
  # The player should answer with the number of the selected option (1-indexed)
  def message_question(player_id, character_id, question, options)
    # The options keys must be buffered, so that the key of the answer can be returned by another method
    @options_buffer = options.keys

    response = {:type => "question"}
    response["player_id"]    = player_id
    response["character_id"] = character_id
    response["question"]     = question
    response["options"]      = options.values
    @response << response.to_json

    # stop the request
    throw :stop
  end

  def message_honor(player_id, amount)
    @response << {:type => "honor", "player_id" => player_id, "amount" => amount}.to_json
  end

  def message_change_cards(player_id, swords, shields, supply)
    response = {:type => "change_cards", "player_id" => player_id}
    response["swords"]  = swords
    response["shields"] = shields
    response["supply"]  = supply
    @response << response.to_json
  end

  def message_item_gained(player_id, item_name)
    @response << {:type => "item_gained", "player_id" => player_id, "item_name" => item_name}.to_json
  end

  def message_item_lost(player_id, item_name)
    @response << {:type => "item_lost", "player_id" => player_id, "item_name" => item_name}.to_json
  end

  def message_win(player_id)
    @response << {:type => "win", "player_id" => player_id}.to_json
  end
end

# For later use if needed
=begin
  def message(type, *args)
    case type
      when :registered
      then message_registered args[0], args[1]

      when :text
      then message_text args[0]

      when :character
      then message_character args[0], args[1]

      when :move
      then message_move args[0], args[1]

      when :next
      then message_next args[0]

      when :question
      then message_question args[0], args[1], args[2], args[3] #should return the key of the selected answer

      when :honor
      then message_honor args[0], args[1]

      when :change_cards
      then message_change_cards args[0], args[1], args[2], args[3]

      when :item_gained
      then message_item_gained args[0], args[1]

      when :item_lost
      then message_item_lost args[0], args[1]

      when :win
      then message_win args[0]

      else raise NotImplementedError, "message :#{type} is not implemented!!"
    end
  end
=end

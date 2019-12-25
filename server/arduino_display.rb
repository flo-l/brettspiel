# display replica for debug
class FakeDisplay
  # just output s as plain text
  def text(text)
    puts "# #{text}"
  end

  # output what character says
  def character(name, text)
    puts "#{name}: #{text}"
  end

  # visualize player has moved (maybe unnecessary)
  def move(player_name, location_name)
    puts "#{player_name} => #{location_name}"
  end

  # display who's turn it is
  def next_player(name)
    puts "Nächster Spieler: #{name}"
  end

  def honor(name, amount)
    amount = "+#{amount}" if amount >= 0
    puts "#{name}: #{amount} Ehre"
  end

  # TODO replace with other items
  def change_cards(name, swords, shields, supply)
    puts "#{name} bekommt: #{swords} sword(s), #{shields} shield(s) and #{supply} supply"
  end

  def item_gained(player_name, item_name)
    puts "#{player_name} erhält #{item_name}."
  end

  def item_lost(player_name, item_name)
    puts "#{player_name} verliert #{item_name}."
  end

  def win(name)
    puts "#{name} GEWINNT!"
    exit # THE END
  end

  # ask player for a name
  # returns String with newline stripped
  def get_name
    gets.strip
  end

  # ask the player a question from character, return which id they chose (0-indexed)
  def question(player_name, character_name, question, options)
    puts "Frage von #{character_name} an #{player_name}: #{question}"
    options.each_with_index { |option,i| puts "(#{i+1}) => #{option}" }

    loop do
      print "Auswahl: "
      selection = gets.chomp.to_i - 1
      return selection if (0...options.count).include? selection
    end
  end
end

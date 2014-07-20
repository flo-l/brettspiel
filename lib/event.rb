# The Event class is the base class of all events.
# Every Event should sublass Event.

# Every Event has possibility points. The percentage with which the event occurs is calculated
# with these possibility points.

# Every Event is assigned to a character and has a text. Later eventually also sound etc may be added.

# An Event might occur just if the player visits the location in "investigate" or "hidden" mode. Also both are possible.
# The possible? method does whatever it want's to check if the event can occur and returns a boolean.

# The occur! method gets called if the event takes place.
# It can execute arbitrary code

# Sometimes it's necessary that an event occurs, in that case the necessary method of the event
# can be overridden to return true and it will be preferred to normal events

class Event
  attr_reader   :character, :text, :investigate, :hidden
  attr_accessor :probability_points

  def initialize
    #this code should never get called
    raise NotImplementedError, "The initialize method of Event MUST be overridden!"
  end

  # called once after instantiation
  def setup!(game)
    # exchange the character name (string) with the actual character object
    character = game.characters.values.find { |character| character.name == @character }
    @character = character
    @character_id = @character.id
  end

  #any event can happen by default, may be overridden
  def possible?(game); true; end

  #no event is necessary by default, may be overridden
  def necessary?(game); false end

  # this is the only way an event can occur, in the future
  # probably more setup will be necessary
  def prepare_and_occur!(game)
    @game = game
    @game.message_character(@character.id, @text)
    occur!
  end

  private

  #sometimes nothing happens, may be overridden
  def occur!; nil; end

  # This takes a hash, whose values are options (strings) and asks the
  # player which they choose. It returns the key of the selected option
  def ask(player, question, options)
    raise ProgrammerError unless player.is_a? Player and options.is_a? Hash

    if @game.answers_buffer.empty?
      # ask for the answer if we don't have it
      @game.message_question(player.id, @character_id, question, options)
    else
      # return the answer if we have it
      @game.answers_buffer.shift
    end
  end

  # Replaces the normal puts with a puts that sends a message from 
  # the character of the event to the player
  def puts(*args)
    args.each do |obj|
      @game.message_character(@character_id, obj.to_s)
    end
  end
end

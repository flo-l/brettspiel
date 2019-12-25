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
  @@events = []

  # Adds event to all locations
  def self.add_to_all
    @all = true
    @@events << self
  end

  # Adds event to all locations
  def self.add_to(*locations)
    @all = false
    @locations = locations
    @@events << self
  end

  def self.events   ; @@events  ; end
  def self.all?     ; @all      ; end
  def self.locations; @locations; end

  attr_reader   :character, :text, :active, :passive
  attr_accessor :probability_points

  def initialize
    #this code should never get called
    raise NotImplementedError, "The initialize method of Event MUST be overridden!"
  end

  # called once after instantiation
  def setup!(game)
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
    @game.message_question(player.id, @character.id, question, options)
  end

  # Replaces the normal puts with a puts that sends a message from
  # the character of the event to the player
  def puts(*args)
    args.each do |obj|
      @game.message_character(@character.id, obj.to_s)
    end
  end
end

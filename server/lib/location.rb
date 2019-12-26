# A location represents a location on the map
# It hasen't been decided yet how to represent the linkage of the map. One proposal is to store the
# ids of all neighbours of a location in an array per location

# Every Location has a human readable name (name) and an id (id, a symbol similar to the name)

# Each location has many events stored in the events array. See Event doc for more info on events

# A player visits a location by executing the visited_by method with itself as the argument

class Location
  # mutable
  attr_accessor :events

  # immutable
  attr_reader :name, :id, :neighbour_ids

  def initialize(id, name, neighbour_ids)
    @name = name
    @id = id
    @neighbour_ids = neighbour_ids

    @events = []
  end

  def select_event(game, mode)
    #find events corresponding to the mode
    events = @events.select { |event| event.send(mode) }

    #calculate probabilities
    sum_probability_points = events.map{ |event| event.probability_points }.inject(:+)
    raise NotImplementedError if sum_probability_points <= 0
    probabilities = events.map{ |event| event.probability_points.to_f / sum_probability_points }

    #find a random number between 1 and 0
    random_number = Kernel.rand

    #decide which event should happen and return it
    index = 0 #index of the event to happen
    probabilities.inject do |sum, probability|
      if probability > random_number
        index += 1 #it must be one of the next events
      else
        break #we've got it
      end
      sum + probability
    end

    #return the selected event
    events[index]
  end

  def to_s
    @name
  end
end

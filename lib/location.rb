# A location represents a location on the map
# It hasen't been decided yet how to represent the linkage of the map. One proposal is to store the
# ids of all neighbours of a location in an array per location

# Every Location has a human readable name (name) and an id (id, a symbol similar to the name)

# Each location has many events stored in the events array. See Event doc for more info on events

# A player visits a location by executing the visited_by method with itself as the argument

class Location
  attr_reader :name, :id, :neighbour_ids
  attr_accessor :events

  def initialize(name, id, neighbour_ids)
    @name = name
    @id = id
    @neighbour_ids = neighbour_ids

    @events = []
  end

  # This lets the @current_player of game visit the location
  # and some event occur
  def get_visited(game, mode)
    event = select_event(game, mode)
    event.prepare_and_occur!(game)
  end

  def select_event(game, mode)
    #find events corresponding to the mode
    events = @events.select { |event| event.send(mode) }

    #find possible events
    events = events.select { |event| event.possible?(game) }

    #find possible and necessary events
    necessary_events = events.select { |event| event.necessary?(game) }

    if necessary_events.empty?
      #calculate probabilities
      sum_probability_points = events.map{ |event| event.probability_points }.inject(:+)
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
    else
      necessary_events.last #return the most recent necessary event if one exists
    end
  end
  private :select_event

  def to_s
    @name
  end
end

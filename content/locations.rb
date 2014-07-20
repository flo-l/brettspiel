# key is id, value: [name, neighbour_ids, events]
LOCATIONS_HASH = {
  1 => ["HQ",    [2,3]],
  2 => ["Markt", [1,3]],
  3 => ["Arena", [1,2]]
}

# add an empty ary for events
LOCATIONS_HASH.each_value { |ary| ary << [] }

# Adds event to all locations
def LOCATIONS_HASH.add_to_all(event)
  raise ArgumentError unless event.is_a?(Class)
  each_value { |ary| ary[2] << event }
end

# Adds event to all locations with names
def LOCATIONS_HASH.add_to(names, event)
  raise ArgumentError unless event.is_a?(Class) && names.is_a?(Array)

  values = each_value.select { |ary| names.member?(ary[0]) }
  values.each { |ary| ary[2] << event }

  # probably a typo in the names ary
  raise ProgrammerError if values.count != names.count
end

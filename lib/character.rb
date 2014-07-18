# A Character is somebody or something that can meet a player through an event.
# Each Character has a name and an id assigned to them and may also store additional information in the object

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

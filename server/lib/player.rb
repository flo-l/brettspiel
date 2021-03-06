# A Player represents a player in real life
# Each Player has a name, maybe customizable
# A Player has honor and up to three items he has to collect
# The Player Object also keeps track of the amount of resources (aka carsd) a player has

# The Cards are wrapped in a CardHand object which offers an own API
# eg: player.cards.count  # overall amount of cards a player has
#     player.cards.swords # amount of swords

# It's possible to add an item to a player like so:
# player.add_item(item)
# There are convenience methods available, that check for the presence
# of one or all of the three items a player should collect

class Player
  attr_reader :name, :id, :current_location, :honor, :cards
  attr_accessor :game

  def initialize(name, game)
    @name = name
    @id = game.players.count

    # The Player needs to know the Game instance he's playing in
    @game = game

    @current_location = game.locations.values.first
    @honor = 0

    @cards = CardHand.new(self)
    @items = []
  end

  # This should prepare the players and set things like start cards etc.
  def prepare!
    @cards.change(swords: 1, shields: 1, supply: 1)
  end

  # This sets current_location to its argument and fires a move message
  def current_location=(location)
    @game.message_move(@id, location.id)
    @current_location = location
  end

  # This fires an event if the honor of a player is changed
  def honor=(n)
    amount = n - @honor
    @game.message_honor(@id, amount)
    @honor = n
  end

  # No items should be added to or removed from the items ary,
  # thus others only get a freezed copy. Items inside can still be modified.
  def items
    @items.dup.freeze
  end

  def add_item(item)
    @game.message_item_gained(@id, item.name)
    @items << item
  end

  def remove_item(item)
    @game.message_item_lost(@id, item.name)
    @items.delete(item)
  end

  def to_s
    @name
  end
end

class CardHand
  attr_reader :swords, :shields, :supply

  def initialize(player)
    @player = player

    @swords = 0
    @shields = 0
    @supply = 0
  end

  # Adds the given amount of cards to the hand and fires a card_change message
  def change(swords: 0, shields: 0, supply: 0)
    @swords += swords
    @shields += shields
    @supply += supply

    @player.game.message_change_cards(@player.id, swords, shields, supply)
  end

  def count
    @swords + @shields + @supply
  end
end

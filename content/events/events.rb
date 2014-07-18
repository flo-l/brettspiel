# sample event #1
class P0wn < Event
  def initialize
    @character = "FloMaster"

    @investigate  = true
    @hidden       = true

    @necessary    = false

    @probability_points = 10

    @text = "Woop, you've been p0wnd!"
  end

  def occur!
    @game.current_player.honor += 50
    #@game.current_player.current_location = @game.locations.values.sample
    #@game.next_player = @game.players.first
  end
end

LOCATIONS_HASH.add_to_all(P0wn)

# sample event #2
class Offer < Event
  def initialize
    @character = "Händler"

    @investigate  = true
    @hidden       = true

    @necessary    = true

    @probability_points = 20

    @text = "Lutscherstange, Lutscherstange.."
  end

  def occur!
    puts (ask(@game.current_player, "Wer will eine Lutscherstange?", { "Well, then you're gay.." => "Ja, absolut!", "Good choice Mister!" => "Definitiv nicht!" }))

    @necessary = false
  end
end

#LOCATIONS_HASH.add_to(["Markt"], Offer)

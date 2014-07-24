# sample event #1
class P0wn < Event
  def initialize
    @character = "FloMaster"

    @active  = true
    @passive = true

    @necessary    = false

    @probability_points = 10

    @text = "Woop, you've been p0wnd!"
  end

  def occur!
    @game.current_player.honor += 50
    @game.current_player.current_location = @game.locations.values.sample
    @game.next_player = @game.players.first
  end
end

P0wn.add_to_all

# sample event #2
class Offer < Event
  def initialize
    @character = "Händler"

    @active  = true
    @passive = true

    @necessary    = true

    @probability_points = 20

    @text = "Lutscherstange, Lutscherstange.."
  end

  def occur!
    yes = ask(@game.current_player, "Wer will eine Lutscherstange?", { true => "Ja, absolut!", false => "Nope!" })

    if yes
      puts "Well, then you're gay.."
    else
      puts ask(@game.current_player, "Wirrklich nicht?", { "Well, then you're gay.." => "Ok doch!", "Good choice Mister!" => "Definitiv nicht!" })
    end

    @necessary = false
  end
end

Offer.add_to "Markt"
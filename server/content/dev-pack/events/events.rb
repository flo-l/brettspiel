# sample event #1
class P0wn < Event
  def initialize
    @character = FloMaster

    @active  = true
    @passive = true

    @probability_points = 10
  end

  def occur!
    puts "Woop, you've been p0wnd!"
    @game.current_player.honor += 50
    @game.current_player.current_location = @game.locations.values.sample
    @game.next_player = @game.players.first
  end
end

P0wn.add_to MainStage

# sample event #2
class Offer < Event
  def initialize
    @character = Händler

    @active  = true
    @passive = true

    @probability_points = 10
  end

  def occur!
    puts "Lutscherstange, Lutscherstange.."

    yes = ask(@game.current_player, "Wer will eine Lutscherstange?", { true => "Ja, absolut!", false => "Nope!" })

    if yes
      puts "Well, then you're gay.."
    else
      puts ask(@game.current_player, "Wirrklich nicht?", { "Well, then you're gay.." => "Ok doch!", "Good choice Mister!" => "Definitiv nicht!" })
    end

    @necessary = false
  end
end

Offer.add_to Markt

class FiberTest < Event
  def initialize
    @character = Hexe

    @active  = true
    @passive = true

    @probability_points = 10
  end

  def occur!
    puts "Test Event!"

    r = rand
    puts r
    x = ask(@game.current_player, "ready?", {'YES' => 'yes', 'NO' => 'no'})
    puts r
    puts x
  end
end

FiberTest.add_to HexenHaus

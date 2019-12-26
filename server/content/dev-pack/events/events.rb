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

    yes = ask(@game.current_player, "Wer will eine Lutscherstange?", { "Ja, absolut!" => true, "Nope!" => false })

    if yes
      puts "Well, then you're gay.."
    else
      puts ask(@game.current_player, "Wirrklich nicht?", { "Ok doch!" => "Well, then you're gay..", "Definitiv nicht!" => "Good choice Mister!" })
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
    puts "Test Event!", character: FloMaster

    r = rand
    puts r
    x = ask(@game.current_player, "ready?", {'yes' => 'YES', 'no' => 'NO'})
    puts r
    puts x
  end
end

FiberTest.add_to HexenHaus

class FiberTest2 < Event
  def initialize
    @character = Hexe

    @active  = true
    @passive = true

    @probability_points = 0
  end

  def occur!
    puts "Not good!", character: FloMaster
  end
end

FiberTest.add_to HexenHaus

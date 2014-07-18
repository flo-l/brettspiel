require 'em-websocket-client'
require 'json'

require_relative 'client_helpers.rb'
require_relative './lib/message.rb'

# This is a simple frontend for the game.
# Should be replayced by something pretty.

# The game client class
class GameClient
  include TCPMessages # lib for message processing

  def initialize(port=2012)
    # Load the content
    # Characters:
    require_relative 'content/characters.rb'
    @characters = {}
    CHARACTERS_HASH.each { |id, name| @characters[id] = Character.new(name, id) }

    # Locations:
    require_relative 'content/locations.rb'
    @locations = {}
    LOCATIONS_HASH.each do |id, ary|
      # instantiate location object with name, id and neighbour_ids
      location = Location.new(ary[0], id, ary[1])

      # store the location object
      @locations[id] = location
    end

    @port = port
    @players = []
    @player_names = [] #dirty hack, fuck it..
  end

  def start!
    greeting

    EM.run do
      # Open up the connection
      #@socket = EventMachine::WebSocketClient.connect("ws://localhost:#{@port}")
      @socket = EventMachine::WebSocketClient.connect("ws://localhost:2012")
      #@socket = EventMachine::WebSocketClient.connect("ws://192.168.0.12:2012")

      # An alias of send_msg
      def @socket.puts(msg)
        send_msg msg
      end

      @socket.callback do
        # Start a new game
        @socket.puts({:type => "new_game"}.to_json)
      end

      @socket.stream do |msg|
        puts "<#{msg}>"
        process Message.new(msg.data)
      end

      @socket.disconnect do
        puts "Disconnected"
        EM::stop_event_loop
      end

      @socket.errback do |e|
        puts "Got error: #{e}"
      end
    end
  end

  def greeting
    puts "WELCOME TO THE GREATEST GAME OF THE WORLD!"
    puts "#############################################"
  end

  # Lets players join, returns if they wish to start
  def join_player
    puts "Bitte geben Sie Ihren Namen ein, um teilzunehmen."
    puts "Um das Spiel zu starten bitte Enter drücken."

    print "Name: "
    name = gets.chomp!

    if name.empty?
      # Let's start the game!
      response = {"type" => "start", "game_id" => @game_id}
      @socket.puts response.to_json
    else
      # Ask the server to register the player
      response = {"type" => "join", "game_id" => @game_id, "name" => name}
      @socket.puts response.to_json

      # Store the player name until the registered msg comes back
      @player_names << name
    end
  end

  # asks the player for next location's id
  def ask_next_location_id
    puts "Was soll das nächste Ziel sein?"

    puts "(#{@current_player.current_location.id}) => #{@current_player.current_location.name}"
    @current_player.current_location.neighbours(@locations).each do |location|
      puts "(#{location.id}) => #{location.name}"
    end

    print "Auswahl: "
    gets.to_i
  end
end

GameClient.new(3000).start!

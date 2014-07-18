require 'thin'
require 'em-websocket'

require_relative 'message.rb'
require_relative 'game.rb'

# decorate send to perform logging etc.
class EventMachine::WebSocket::Connection
  alias :old_send :send

  def send(msg)
    puts "Sent: #{msg}"
    old_send msg
  end
end

class Server
  def initialize
    # Hash with game_ids as keys and arrays of websocket_connections, aka clients as values
    @clients = {}
  end

  # send all messages in res_ary to all clients
  def send_to_all(game_id, res_ary)
    @clients[game_id].each do |client|
      res_ary.each do |msg|
        client.send msg
      end
    end
  end

  # This handles an incoming message and returns an ary with min 1 response
  def handle(msg_str, client)
    msg = Message.new(msg_str)

    # Create a new game if needed and send the response back immediatly returning early
    if msg.type.to_sym == :new_game
      return client.send new_game
    end

    # Register a client if they want to join a game, associating each connection with a game id
    if msg.type.to_sym == :join
      if @clients[msg.game_id]
        @clients[msg.game_id] << client unless @clients[msg.game_id].include? client
      elsif
        @clients[msg.game_id] = [client]
      end
    end

    # Load the game
    if msg.respond_to?(:game_id)
      game = Game.load(msg.game_id)
    else
      raise StandardError, "Invalid Message, lacking game_id. msg: #{msg_str}"
    end

    # Handle the request with the game object
    res_ary = game.handle(msg)

    # Save the game
    game.save!

    # Deliver the response(s) to all concerned clients
    send_to_all(game.id, res_ary)
  end

  # Creates a new game
  def new_game
    game = Game.new
    game.save!
    {:type => "game_created", :game_id => game.id}.to_json
  end

  # Start the Server and go!
  def start!
    puts "started"

    EM.run do
      EM::WebSocket.start(:host => '0.0.0.0', :port => '2012') do |ws|
        ws.onopen do |obj|
          puts "somebody connected!"
        end

        ws.onmessage do |msg|
          puts "Recieved: #{msg}"
          handle(msg, ws)
        end

        ws.onclose do |reason|
          puts "somebody has disconnected"
          @clients.each { |_,clients| clients.delete(ws) }
        end
      end
    end
  end
end

Server.new.start!

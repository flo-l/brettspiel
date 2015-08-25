require 'thin'
require 'em-websocket'

require_relative 'packer.rb'
require_relative 'message.rb'
require_relative 'game.rb'

# Exceptions
class ServerError < StandardError; nil; end

class InvalidMessageError < ServerError; nil; end
class LackingGameIdError < InvalidMessageError; nil; end
class MessageTypeUnknownError < InvalidMessageError; nil; end

class UserInputError < ServerError; nil; end
class GameIdWrongError < UserInputError; nil; end
class PlayerNameTakenError < UserInputError; nil; end
class WrongPlayerMoveError < UserInputError; nil; end
class ImpossibleResponseError < UserInputError; nil; end
class LocationNotPresentError < UserInputError; nil; end
class LocationNotReachableError < UserInputError; nil; end

class GameAlreadyStartedError < ServerError; nil; end
class GameNotStartedError < ServerError; nil; end
class NoPlayerError < ServerError; nil; end

class ProgrammerError < ServerError; nil; end

# decorate send to perform logging etc.
class EventMachine::WebSocket::Connection
  alias :old_send :send

  def send(msg)
    puts "Sent: #{msg}"
    old_send msg
  end
end

class Server
  # directory with content packs
  PACK_DIR = Dir.open(File.expand_path 'content')
  
  def initialize
    # Hash with game_ids as keys and arrays of websocket_connections, aka clients as values
    @clients = {}
    
    # find available content-packs
    @available_packs = PACK_DIR.each.reject{ |x| ['.', '..'].include? x }.to_a
    
    puts "packing content:", @available_packs.map { |pack| "  => #{pack}" }, ""
    
    # build all content packs on server start
    @available_packs.each { |pack| Packer.pack(pack) }
  end

  private
  
  # send all messages in res_ary to all clients
  def send_to_all(game_id, res_ary)
    @clients[game_id].each do |client|
      res_ary.each do |msg|
        client.send msg
      end
    end
  end

  # This handles an incoming message and returns an ary with min 1 response(s)
  def handle(msg_str, client)
    msg = Message.new(msg_str)

    # Create a new game if needed and send the response back immediatly returning early
    if msg.type.to_sym == :new_game
      return client.send new_game
    end

    # Load the game
    if msg.respond_to?(:game_id)
      game = Game.load(msg.game_id)
    else
      raise LackingGameIdError
    end

    # Register a client if they want to join the game, associating each connection with a game id
    if msg.type.to_sym == :join
      if @clients[msg.game_id]
        @clients[msg.game_id] << client unless @clients[msg.game_id].include? client
      else
        @clients[msg.game_id] = [client]
      end
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
    game = Game.new(@available_packs.first)
    game.save!
    {:type => "game_created", :game_id => game.id}.to_json
  end

  public
  
  # Start the Server and go!
  def start!(port=2012)
    puts "listening on localhost:#{port}"

    EM.run do
      EM::WebSocket.start(:host => '0.0.0.0', :port => port) do |ws|
        ws.onopen do |obj|
          puts "somebody connected!"
        end

        ws.onmessage do |msg|
          puts "Recieved: #{msg}"

          begin
            handle(msg, ws)
          rescue ServerError => error
            puts "SERVER ERROR: " + error.class.to_s
          rescue => e
            puts "GENERIC ERROR:"
            puts "#{e.backtrace.shift}: #{e.message} (#{e.class})", e.backtrace.map { |s| "        from " << s }
          end
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

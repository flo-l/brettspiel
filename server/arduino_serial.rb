require 'rubyserial'

# this class abstracts communication with the arduino
class Arduino
  BAUDRATE = 9600
  PORTNAME = Dir["/dev/ttyACM*"].first

  # TODO: fix buttons ary size (zB 32)
  def initialize
    @port = Serial.new(PORTNAME, BAUDRATE)
  end

  # this blocks until data from the arduino could be read and then
  # returns an ary of bools, where true means button at index is pressed
  def buttons_changed
    data = ""
    loop do
      begin
        data = @port.read(8)
        next if data == ""
        bits = data.unpack("b*").first
        return bits.chars.map do |c|
          c == '1'
        end
      rescue RubySerial::Exception => e
        puts e
      end
    end
  end
end

# arduino replica for debug
class FakeArduino
  BUTTON_NUM = 32

  def initialize
    @buttons = Array.new(BUTTON_NUM, false)
  end

  def buttons_changed
    loop do
      print "Toggle: "
      input = gets
      if input == "\n"
        puts @buttons.each_with_index.select{ |s,i| s }.map{|_,i| i}.join
        next
      end
      n = input.to_i
      next unless n
      @buttons[n] = !@buttons[n]
      @buttons.map! { |x| x.nil? ? false : x }
      return @buttons.dup
    end
  end
end

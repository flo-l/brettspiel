require 'java'
require "/usr/share/java/jna.jar"
require "/usr/share/java/jna/jna-platform.jar"

java_import 'com.sun.jna.Native'
java_import 'com.sun.jna.NativeLong'
java_import 'com.sun.jna.platform.unix.X11'
java_import 'java.awt.Color'
java_import 'java.awt.Font'
java_import 'java.awt.BorderLayout'
java_import 'java.awt.Toolkit'
java_import 'java.awt.font.TextLayout'
java_import 'java.awt.Rectangle'

java_import 'javax.swing.JFrame'
java_import 'javax.swing.BorderFactory'
java_import 'javax.swing.JPanel'
java_import 'javax.swing.JLabel'
java_import 'javax.swing.SwingConstants'

class X11FullscreenHelper
  NET_WM_STATE_REMOVE = 0
  NET_WM_STATE_ADD    = 1

  attr_reader :isFullScreenMode

  def initialize
    @isFullScreenMode = false
  end

  def setFullScreenWindow(w, fullScreen)
    x = X11.INSTANCE
    display = nil
    begin
      display = x.XOpenDisplay(nil)

      result = sendClientMessage(
        display,
        Native.getWindowID(w),
        "_NET_WM_STATE",
        [
          NativeLong.new(fullScreen ? NET_WM_STATE_ADD : NET_WM_STATE_REMOVE),
          x.XInternAtom(display, "_NET_WM_STATE_FULLSCREEN", false),
          x.XInternAtom(display, "_NET_WM_STATE_ABOVE", false),
          NativeLong.new(0),
          NativeLong.new(0)
        ].to_java(NativeLong)
      )
      @isFullScreenMode = (result != 0) && fullScreen;
      result != 0
    ensure
      x.XCloseDisplay(display) if display
    end
  end

  def sendClientMessage(display, wid, msg, data)
    raise ArgumentException unless data.length == 5
    x = X11.INSTANCE
    event = X11::XEvent.new
    event.type = X11::ClientMessage
    event.setType(X11::XClientMessageEvent.java_class)
    event.xclient.type = X11::ClientMessage
    event.xclient.serial = NativeLong.new(0)
    event.xclient.send_event = 1
    event.xclient.message_type = x.XInternAtom(display, msg, false)
    event.xclient.window = X11::Window.new(wid)
    event.xclient.format = 32
    event.xclient.data.setType([].to_java(NativeLong).java_class)
    event.xclient.data.l = data

    mask = NativeLong.new(X11.SubstructureRedirectMask | X11.SubstructureNotifyMask)
    result = x.XSendEvent(display, x.XDefaultRootWindow(display), 0, mask, event)
    x.XFlush(display)
    result
  end
end

import javax.swing.JFrame

class ArduinoDisplay < JFrame
  BORDER_SIZE = 10

  def initialize
    super "GUI"
    self.initUI
  end

  def initUI
    screen_size = Toolkit.getDefaultToolkit.getScreenSize
    @x,@y = *[screen_size.width, screen_size.height]

    panel = JPanel.new
    panel.setLayout BorderLayout.new 0, 0
    panel.setBackground Color::BLACK

    @label = JLabel.new "", SwingConstants::CENTER
    @label.setFont Font.new "Arial", Font::BOLD, 14
    @label.setForeground Color::WHITE

    panel.add @label, BorderLayout::CENTER
    panel.setBorder BorderFactory.createEmptyBorder *[BORDER_SIZE]*4
    self.add panel
    self.pack

    self.setDefaultCloseOperation JFrame::EXIT_ON_CLOSE
    self.setLocationRelativeTo nil
    self.setExtendedState(JFrame::MAXIMIZED_BOTH)
    self.setVisible true
    helper = X11FullscreenHelper.new
    helper.setFullScreenWindow(self, true)
  end

  def render_text(str)
    html = ""
    words = str.split(' ')
    font = @label.font
    max_width = @x-20
    max_height = @y-20
    upper = @x
    lower = 0
    size = font.size

    loop do
      html = ""
      metrics = @label.getFontMetrics(font)
      spacings = words.map{|word| metrics.string_width(word)}
      space = metrics.string_width(' ')
      line_height = metrics.height
      height = line_height
      width = 0

      spacings.zip(words).each do |w, word|
        width += w
        if width > max_width
          width = w + space
          html << '<br>' + word + ' '
          height += line_height
        else
          width += space
          html << word + ' '
        end
      end

      if height < max_height
        lower = size
        size += (upper-lower)/2
      else
        upper = size
        size -= (upper-lower)/2
      end

      font = Font.new(font.name, Font::PLAIN, size)
      break if upper - lower <= 1
    end

    html = "<html><div style='text-align: center;'>#{html}</html>"
    @label.font = font
    
    @label.text = html
  end

  # just output s as plain text
  def text(text)
    render_text text
  end

  # output what character says
  def character(name, text)
    puts "#{name}: #{text}"
  end

  # visualize player has moved (maybe unnecessary)
  def move(player_name, location_name)
    puts "#{player_name} => #{location_name}"
  end

  # display who's turn it is
  def next_player(name)
    puts "Nächster Spieler: #{name}"
  end

  def honor(name, amount)
    amount = "+#{amount}" if amount >= 0
    puts "#{name}: #{amount} Ehre"
  end

  # TODO replace with other items
  def change_cards(name, swords, shields, supply)
    puts "#{name} bekommt: #{swords} sword(s), #{shields} shield(s) and #{supply} supply"
  end

  def item_gained(player_name, item_name)
    puts "#{player_name} erhält #{item_name}."
  end

  def item_lost(player_name, item_name)
    puts "#{player_name} verliert #{item_name}."
  end

  def win(name)
    puts "#{name} GEWINNT!"
    exit # THE END
  end

  # ask player for a name
  # returns String with newline stripped
  def get_name
    gets.strip
  end

  # ask the player a question from character, return which id they chose (0-indexed)
  def question(player_name, character_name, question, options)
    puts "Frage von #{character_name} an #{player_name}: #{question}"
    options.each_with_index { |option,i| puts "(#{i+1}) => #{option}" }

    loop do
      print "Auswahl: "
      selection = gets.chomp.to_i - 1
      return selection if (0...options.count).include? selection
    end
  end
end

a = ArduinoDisplay.new
loop do
a.text "Hallo Gregor.."
sleep 2
a.text "BLabla BLa"
sleep 2
end

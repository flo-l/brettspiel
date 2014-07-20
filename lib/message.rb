require 'json'

# A Message, used to parse a message, sending doesn't require this class,
# because it's so easy
class Message
  def initialize(msg_str)
    # A hash with all arguments of the message
    @hash = JSON.parse(msg_str)

    metaclass = class << self; self; end

    # Make all fields available
    @hash.keys.each do |key|
      metaclass.send(:define_method, key) do
        @hash[key]
      end
    end
  end

  # In case some fields are missing
  def method_missing(*args)
    raise InvalidMessageError
  end
end

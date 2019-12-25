  def buttons_changed(new_buttons)
    # read new button state


    # replace all unchanged values with nil and all changed ones with the id + new state
    changed = []
    @buttons.zip(new_buttons).each_with_index do |(x,y),i|
      if x != y
        # [id, new_state]
        changed << [i,y]
      end
    end

    @buttons = new_buttons
    changed
  end

@buttons    = [false, false]
p buttons_changed [true, false]
p buttons_changed [true, true]
p buttons_changed [true, false]

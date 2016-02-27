using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel;
using Client.Frame;

namespace Client
{
  class MainWindowViewModel : ViewModelBase
  {
    public ConnectionControlViewModel VM { get; set; }

    public MainWindowViewModel()
    {
      VM = new ConnectionControlViewModel(); //DEBUG
    }
  }
}

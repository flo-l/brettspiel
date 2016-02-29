using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ViewModel;

namespace Client.Frame
{
  class ConnectionControlViewModel : ViewModelBase
  {
    #region Control Properties
    public string Host { get; set; }
    public string PlayerName { get; set; }
    public string GameID { get; set; }
    public List<string> Themes { get; set; }
    public string SelectedTheme { get; set; }

    string Status_;
    public string Status
    {
      get { return Status_; }
      set
      {
        Status_ = value;
        OnPropertyChanged("Status");
      }
    }

    #region Button Enable

    bool ConnectEnabled_;
    public bool ConnectEnabled
    {
      get { return ConnectEnabled_; }
      set
      {
        ConnectEnabled_ = value;
        OnPropertyChanged("ConnectEnabled");
      }
    }

    bool JoinEnabled_;
    public bool JoinEnabled
    {
      get { return JoinEnabled_; }
      set
      {
        JoinEnabled_ = value;
        OnPropertyChanged("JoinEnabled");
      }
    }

    bool CreateEnabled_;
    public bool CreateEnabled
    {
      get { return CreateEnabled_; }
      set
      {
        CreateEnabled_ = value;
        OnPropertyChanged("CreateEnabled");
      }
    }
    bool AddEnabled_;
    public bool AddEnabled
    {
      get { return AddEnabled_; }
      set
      {
        AddEnabled_ = value;
        OnPropertyChanged("AddEnabled");
      }
    }

    bool StartEnabled_;
    public bool StartEnabled
    {
      get { return StartEnabled_; }
      set
      {
        StartEnabled_ = value;
        OnPropertyChanged("StartEnabled");
      }
    }
    #endregion

    string ConnectText_;
    public string ConnectText
    {
      get { return ConnectText_; }
      private set
      {
        ConnectText_ = value;
        OnPropertyChanged("ConnectText");
      }
    }
    #endregion

    SocketModel Connection;

    bool IsHostUsed = false;

    #region Commands
    void ConnectCommand_()
    {
      if (!IsHostUsed)
      {
        try
        {
          Host = "ws://127.0.0.1:2012"; //DEBUG
          Connection = new SocketModel(Host);
          IsHostUsed = true;
          ConnectText = "Abort";
          Status = "Connecting...";

          Connection.Opened += Socket_Opened;
        }
        catch
        {
          Status = "Connection failed";
        }
      }

      else
      {
        IsHostUsed = false;
        ConnectText = "Connect";
        Status = "Connection aborted";

        Connection.Opened -= Socket_Opened;

        Connection.Dispose();
      }
    }
    void Socket_Opened(object sender, EventArgs e)
    {
      ConnectEnabled = false;
      CreateEnabled = true;
      JoinEnabled = true;

      Status = "Connection established";

//      Connection.AddEvent(typeof(Message), regCommand);
//      Connection.AddEvent(typeof(Message), gameCommand);
//      Connection.AddEvent("next", nextCommand);
//      Connection.AddEvent("pack_list", packCommand);

      Connection.Send(new RequestPackListMessage());
    }

    void regCommand(Message msg)
    {

    }
    void gameCommand(Message msg)
    {
 //     GameID = msg.Attributes["game_id"];

    }
    void nextCommand(InGameMessage msg)
    {

    }
    void packCommand(Message msg)
    {
 //     string asdf = msg.Attributes["packs"];
    }

    public DelegateCommand ConnectCommand { get; private set; }
    void JoinCommand_()
    {

    }
    public DelegateCommand JoinCommand { get; private set; }
    void CreateCommand_()
    {

    }
    public DelegateCommand CreateCommand { get; private set; }
    void AddCommand_()
    {

    }
    public DelegateCommand AddCommand { get; private set; }
    void StartCommand_()
    {

    }
    public DelegateCommand StartCommand { get; private set; }

    #endregion

    public ConnectionControlViewModel()
    {
      ConnectCommand = new DelegateCommand(ConnectCommand_);
      JoinCommand = new DelegateCommand(JoinCommand_);
      CreateCommand = new DelegateCommand(CreateCommand_);
      AddCommand = new DelegateCommand(AddCommand_);
      StartCommand = new DelegateCommand(StartCommand_);

      ConnectText = "Connect";

      ConnectEnabled = true;
      JoinEnabled = false;
      CreateEnabled = false;
      AddEnabled = false;
      StartEnabled = false;
    }
  }
}

﻿using System;
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
    public string GameID 
    { 
      get
      {
        if (Connection != null)
          return Connection.GameID;
        return "";
      }
      set
      {
        if (Connection != null)
          Connection.GameID = value;
      }
    }

    List<string> Themes_ = new List<string>();
    public List<string> Themes
    {
      get
      {
        return Themes_;
      }
      private set
      {
        Themes_ = value;
        OnPropertyChanged("Themes");

        if (Themes_ != null && Themes_.Count > 0)
          SelectedTheme = Themes_[0];

      }
    }
    string SelectedTheme_;

    public string SelectedTheme
    {
      get { return SelectedTheme_; }
      set
      {
        SelectedTheme_ = value;
        OnPropertyChanged("SelectedTheme");
      }
    }

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

    //TODO Check flow of registration commands
    #region Commands
    void Socket_Opened(object sender, EventArgs e)
    {
      ConnectEnabled = false;
      CreateEnabled = true;
      JoinEnabled = true;

      Status = "Connection established";

      Connection.AddEvent<PackListMessage>(packCommand);

      Connection.Send(new RequestPackListMessage());
    }

    void regCommand(RegisteredMessage msg)
    {
      Status = msg.PlayerName + " joined the game";
    }
    void gameCreatedCommand(GameCreatedMessage msg)
    {
      GameID = msg.GameID;

      JoinEnabled = false;
      CreateEnabled = false;
      AddEnabled = true;
      StartEnabled = true;

      Connection.RemoveEvent<GameCreatedMessage>(gameCreatedCommand);
      Connection.AddEvent<RegisteredMessage>(regCommand);
    }
    void nextCommand(NextMessage msg)
    {
      
    }
    void packCommand(PackListMessage msg)
    {
      Themes = msg.Packs;
      Connection.RemoveEvent<PackListMessage>(packCommand);
    }

    void ConnectCommand_()
    {
      if (!IsHostUsed)
      {
        try
        {
          Host = "ws://127.0.0.1:2012"; //DEBUG
          Connection = new SocketModel(Host);
          IsHostUsed = true;

          Connection.Opened += Socket_Opened;
          Connection.Open();
          ConnectText = "Abort";
          Status = "Connecting...";
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
    public DelegateCommand ConnectCommand { get; private set; }
    void JoinCommand_()
    {
      CreateEnabled = false;
      JoinEnabled = false;
      AddEnabled = true;

      Connection.AddEvent<RegisteredMessage>(regCommand);
      Connection.AddEvent<NextMessage>(nextCommand);
    }
    public DelegateCommand JoinCommand { get; private set; }
    void CreateCommand_()
    {
      Connection.AddEvent<GameCreatedMessage>(gameCreatedCommand);

      NewGameMessage msg = new NewGameMessage();
      msg.ContentPack = SelectedTheme;
      Connection.Send(msg);
    }
    public DelegateCommand CreateCommand { get; private set; }
    void AddCommand_()
    {
      JoinMessage msg = new JoinMessage();
      msg.PlayerName = PlayerName;
      Connection.Send(msg);
    }
    public DelegateCommand AddCommand { get; private set; }
    void StartCommand_()
    {
      Connection.Send(new StartMessage());
      Connection.AddEvent<NextMessage>(nextCommand);
      Connection.RemoveEvent<RegisteredMessage>(regCommand);
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

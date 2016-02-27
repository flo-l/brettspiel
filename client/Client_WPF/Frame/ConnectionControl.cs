using System;
using SuperSocket;
using WebSocket4Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.IO;

namespace Client
{
  [DataContract]
  public class Message
  {
    [DataMember]
    public string Type { get; private set; }

    [DataMember]
    public Dictionary<string, string> Attributes { get; private set; }

    public Message(string type)
    {
      Type = type;
      Attributes = new Dictionary<string, string>();
    }

    public void Add(string key, string value)
    {
      Attributes.Add(key, value);
    }
  }

  public class SocketModel : IDisposable
  {
    WebSocket Socket;

    #region general fields and properties

    private string Host_ = "ws://127.0.0.1:2012";
    public string Host
    {
      get
      {
        return Host_;
      }
    }
    public string ContentPackHost
    {
      get
      {
        return Host_.Replace("2012", "2013").Replace("ws", "http");
      }
    }

    private List<int> ClientIDs_ = new List<int>();
    private bool ClientIDsSet = false;
    public List<int> ClientIDs
    {
      get
      {
        return ClientIDs_;
      }
      set
      {
        if (!ClientIDsSet)
        {
          ClientIDsSet = true;
          ClientIDs_ = value;
        }
      }
    }

    private string GameID_; // The Game ID at the server
    private bool GameIDSet = false;
    public string GameID
    {
      get
      {
        return GameID_;
      }
      set
      {
        if (!GameIDSet)
        {
          GameIDSet = true;
          GameID_ = value;
        }
      }
    }
    #endregion

    #region ctor & disposal
    public SocketModel(string uri)
    {
      Host_ = uri;
      Socket = new WebSocket(Host);
      Socket.Open();

      MessageRecievedActions = new Dictionary<string, List<Action<Message>>>();
      Socket.MessageReceived += Socket_MessageReceived;
    }

    public void Dispose()
    {
      Socket.Dispose();
      MessageRecievedActions = new Dictionary<string, List<Action<Message>>>();
    }
    #endregion

    #region events & wrappers

    Dictionary<string, List<Action<Message>>> MessageRecievedActions;
    void Socket_MessageReceived(object sender, MessageReceivedEventArgs e)
    {
      Message msg;
      using (Stream s = StringToStream(e.Message))
      {
        DataContractJsonSerializer deser = new DataContractJsonSerializer(typeof(Message));
        try
        {
          msg = deser.ReadObject(s) as Message;
        }
        catch
        {
          Console.WriteLine("Couldn't deserialize: " + e.Message); //DEBUG
          return;
        }
      }

      if (!MessageRecievedActions.ContainsKey(msg.Type))
        return;

      foreach (Action<Message> a in MessageRecievedActions[msg.Type])
      {
        a.Invoke(msg);
      }
    }


    public void AddEvent(string type, Action<Message> action)
    {
      if (!MessageRecievedActions.ContainsKey(type))
        MessageRecievedActions.Add(type, new List<Action<Message>>());

      MessageRecievedActions[type].Add(action);
    }

    public void RemoveEvent(string type, Action<Message> action)
    {
      if (!MessageRecievedActions.ContainsKey(type))
        return;
      MessageRecievedActions[type].Remove(action);
    }

    public event EventHandler Opened 
    { 
      add { Socket.Opened += value; } 
      remove { Socket.Opened -= value; } 
    }
    public event EventHandler Closed 
    {
      add { Socket.Closed += value; } 
      remove { Socket.Closed -= value; } 
    }

    #endregion

    public void Send(Message message)
    {
      DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Message));
      MemoryStream stream = new MemoryStream();

      if (message.Type != "new_game" && message.Type != "theme" && GameIDSet)
        message.Add("game_id", GameID);

      ser.WriteObject(stream, message);

      stream.Position = 0;
      StreamReader sr = new StreamReader(stream);

      Socket.Send(sr.ReadToEnd());
    }
    Stream StringToStream(string s)
    {
      MemoryStream stream = new MemoryStream();
      StreamWriter writer = new StreamWriter(stream);
      writer.Write(s);
      writer.Flush();
      stream.Position = 0;
      return stream;
    }
  }
}
using System;
using SuperSocket;
using WebSocket4Net;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SimpleJson;
using System.Reflection;
using System.Collections;

namespace Client
{

  #region helpers
  [AttributeUsage(AttributeTargets.Property)]
  public class JsonExportAttribute : Attribute
  {
    public string name;

    public JsonExportAttribute(string name)
    {
      this.name = name;
    }
    public static JsonObject GetJson(object obj)
    {
      PropertyInfo[] PropInf = obj.GetType().GetProperties();
      JsonObject jobj = new JsonObject();

      foreach (PropertyInfo p in PropInf)
      {
        JsonExportAttribute attr = (JsonExportAttribute)Attribute.GetCustomAttribute(p, typeof(JsonExportAttribute));
        if (attr == null)
          continue;
        jobj.Add(attr.name, p.GetValue(obj));
      }
      return jobj;
    }
  }

  public static class ReflectiveEnumerator
  {
    static ReflectiveEnumerator() { }

    public static IEnumerable<Type> GetListOfType<T>() where T : class
    {
      return Assembly.GetAssembly(typeof(T)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))); 
    }
  }

  class ActionDictionary
  {
    Dictionary<Type, List<dynamic>> dict = new Dictionary<Type, List<dynamic>>();

    public void Add<T>(Action<T> action)
    {
      Type type = typeof(T);
      if (!dict.ContainsKey(type))
      {
        dict.Add(type, new List<dynamic>());
      }

      dict[type].Add(action);
    }

    public void Remove<T>(Action<T> action)
    {
      Type type = typeof(T);
      if (!dict.ContainsKey(type))
        return;
      dict[type].Remove(action);
      if (dict[type].Count == 0)
        dict.Remove(type);
    }

    public void Invoke(object args)
    {
      if (dict.ContainsKey(args.GetType()))
      {
        foreach (dynamic action in dict[args.GetType()])
        {
          action.Invoke((dynamic)args);
          if (!dict.ContainsKey(args.GetType()))
            break;
        }
      }
    }
  }
#endregion

  #region messages

  #region Base
  public abstract class Message
  {
    [JsonExport("type")]
    public abstract string Type { get; }

    //TODO maybe put this in a separate attribute with class as target
    //TODO replace try catch with proper errorhandling / exceptions
    //TODO implement nested classes / handle json objs and json arrays
    public static bool Deserialize(JsonObject obj, out Message msg)
    {
      Init();

      msg = null;

      string MessageType;
      try
      {
        MessageType = (string)obj["type"];
      }
      catch
      {
        //DEBUG
        Console.WriteLine("Couldn't deserialize, because type was missing");
        Console.WriteLine(obj);
        return false;
      }

      foreach (Type t in MessageTypes)
      {
        Message Output = (Message)Activator.CreateInstance(t);

        if (Output.Type == (string)obj["type"])
        {
          foreach (PropertyInfo p in t.GetRuntimeProperties())
          {
            JsonExportAttribute attr = (JsonExportAttribute)Attribute.GetCustomAttribute(p, typeof(JsonExportAttribute));
            if (attr == null || attr.name == "type")
              continue;

            object ObjectBuf;

            try
            {
              ObjectBuf = obj[attr.name];
            }
            catch
            {
              //DEBUG
              Console.WriteLine("Couldn't deserialize to " + t.ToString() + " , because a property was missing");
              Console.WriteLine(obj);
              return false;
            }
            if (typeof(ICollection).IsAssignableFrom(p.PropertyType) && p.PropertyType.IsGenericType && ObjectBuf is JsonArray)
            {
              JsonArray JsonArray = (ObjectBuf as JsonArray);

              if (JsonArray.Count == 0)
              {
                ObjectBuf = Activator.CreateInstance(p.PropertyType);
              }
              else
              {
                Type[] ContainedTypes = p.PropertyType.GetGenericArguments();
                if (ContainedTypes.Length == 1 && typeof(ICollection<>).MakeGenericType(ContainedTypes).IsAssignableFrom(p.PropertyType))
                {
                  if (ContainedTypes[0].IsAssignableFrom(JsonArray[0].GetType()))
                  {
                    dynamic Collection = (dynamic)Activator.CreateInstance(p.PropertyType);
                    foreach (object o in JsonArray)
                    {
                      dynamic added = Convert.ChangeType(o, ContainedTypes[0]);
                      Collection.Add(added);
                    }
                    ObjectBuf = Collection;
                  }
                }
                if (ContainedTypes.Length == 2 && typeof(IDictionary<,>).MakeGenericType(ContainedTypes).IsAssignableFrom(p.PropertyType))
                {
                  if (ContainedTypes[0].IsAssignableFrom((JsonArray[0] as JsonObject)["Key"].GetType()) && 
                      ContainedTypes[1].IsAssignableFrom((JsonArray[0] as JsonObject)["Value"].GetType()))
                  {
                    dynamic Dict = (dynamic)Activator.CreateInstance(p.PropertyType);
                    foreach (object o in JsonArray)
                    {
                      JsonObject JsonObject = o as JsonObject;
                      Dict.Add((dynamic)JsonObject["Key"], (dynamic)JsonObject["Value"]);
                    }
                    ObjectBuf = Dict;
                  }
                }
              }
            }
            try
            {
              p.SetValue(Output, Convert.ChangeType(ObjectBuf, p.PropertyType));
            }
            catch
            {
              //DEBUG
              Console.WriteLine("Couldn't deserialize to " + t.ToString() + " , because conversion from " + ObjectBuf.GetType().ToString() + " to " + p.PropertyType.ToString() + " was not possible");
              Console.WriteLine(obj);
              return false;
            }
          }

          msg = Output;
          return true;
        }
      }
      Console.WriteLine("Couldn't deserialize, because the given type doesn't exist");
      Console.WriteLine(obj);
      return false;
    }

    static List<Type> MessageTypes;

    static bool IsInit = false;
    public static void Init()
    {
      if (IsInit)
        return;
      MessageTypes = new List<Type>(ReflectiveEnumerator.GetListOfType<Message>());
      IsInit = true;
    }
  }

  public abstract class InGameMessage : Message
  {
  }

  #endregion

  #region Server
  public class PackListMessage : Message
  {
    [JsonExport("packs")]
    public List<string> Packs { get; set; }

    public override string Type
    {
      get { return "pack_list"; }
    }
  }
  public class GameCreatedMessage : Message
  {
    public override string Type
    {
      get { return "game_created"; }
    }

    [JsonExport("game_id")]
    public string GameID { get; set; }
  }
  public class RegisteredMessage : Message
  {
    public override string Type
    {
      get { return "registered"; }
    }

    [JsonExport("content_pack")]
    public string ContentPack { get; set; }

    [JsonExport("player_id")]
    public int PlayerID { get; set; }

    [JsonExport("player_name")]
    public string PlayerName { get; set; }
  }
  public class CharacterMessage : Message
  {
    public override string Type
    {
      get { return "characted"; }
    }

    [JsonExport("character_id")]
    public int CharacterID { get; set; }

    [JsonExport("text")]
    public string Text { get; set; }
  }
  public class NextMessage : Message
  {
    public override string Type
    {
      get { return "next"; }
    }

    [JsonExport("player_id")]
    public int PlayerID { get; set; }
  }
  public class MoveMessage : Message
  {
    public override string Type
    {
      get { return "move"; }
    }

    [JsonExport("player_id")]
    public int PlayerID { get; set; }

    [JsonExport("location_id")]
    public int LocationID { get; set; }
  }
  public class QuestionMessage : Message
  {
    public override string Type
    {
      get { return "question"; }
    }

    [JsonExport("player_id")]
    public int PlayerID { get; set; }

    [JsonExport("character_id")]
    public int CharacterID { get; set; }

    [JsonExport("question")]
    public string Question { get; set; }

    [JsonExport("options")]
    public List<string> Options { get; set; }
  }
  public class ItemGainedMessage : Message
  {
    public override string Type
    {
      get { return "item_gained"; }
    }

    [JsonExport("player_id")]
    public int PlayerID { get; set; }

    [JsonExport("item_name")]
    public string ItemName { get; set; }
  }
  public class ItemLostMessage : Message
  {
    public override string Type
    {
      get { return "item_lost"; }
    }

    [JsonExport("player_id")]
    public int PlayerID { get; set; }

    [JsonExport("item_name")]
    public string ItemName { get; set; }
  }
  public class WinMessage : Message
  {
    public override string Type
    {
      get { return "win"; }
    }

    [JsonExport("player_id")]
    public int PlayerID { get; set; }
  }
#endregion

  #region Client
  public class NewGameMessage : Message
  {
    public override string Type
    {
      get { return "new_game"; }
    }

    [JsonExport("content_pack")]
    public string ContentPack { get; set; }
  }
  public class RequestPackListMessage : Message
  {
    public override string Type
    {
      get { return "request_pack_list"; }
    }
  }
  public class JoinMessage : InGameMessage
  {
    public override string Type
    {
      get { return "join"; }
    }

    [JsonExport("name")] //TODO check if JoinMessage is implemented the same way in ruby
    public string PlayerName { get; set; }
  }
  public class StartMessage : InGameMessage
  {
    public override string Type
    {
      get { return "start"; }
    }
  }
  public class AnswerMessage : InGameMessage
  {
    public override string Type
    {
      get { return "answer"; }
    }

    [JsonExport("answer")]
    public int Answer { get; set; }
  }
  public class MoveRequestMessage : InGameMessage
  {
    public override string Type
    {
      get { return "move_request"; }
    }

    [JsonExport("player_id")]
    public int PlayerID { get; set; }

    [JsonExport("location_id")]
    public int LocationID { get; set; }
  }
#endregion

  #endregion

  #region Socket
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

    private string GameID_ = ""; // The Game ID at the server
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

      MessageRecievedActions = new ActionDictionary();
      Socket.MessageReceived += Socket_MessageReceived;
    }

    public void Open()
    {
      Socket.Open();
    }

    public void Dispose()
    {
      Socket.Dispose();
      MessageRecievedActions = new ActionDictionary();
    }
    #endregion

    #region events & wrappers

    ActionDictionary MessageRecievedActions;
    void Socket_MessageReceived(object sender, MessageReceivedEventArgs e)
    {
      object BUF;
      Message msg;
      if (!SimpleJson.SimpleJson.TryDeserializeObject(e.Message, out BUF))
      {
        //DEBUG
        Console.WriteLine("Deserialization to JsonObject failed");
        return;
      }
      if (!Message.Deserialize((JsonObject)BUF, out msg))
      {
        return;
      }

      MessageRecievedActions.Invoke(msg);
    }


    public void AddEvent<T>(Action<T> action) where T : Message
    {
      MessageRecievedActions.Add<T>(action);
    }

    public void RemoveEvent<T>(Action<T> action) where T : Message
    {
      MessageRecievedActions.Remove<T>(action);
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
      JsonObject msg = JsonExportAttribute.GetJson(message);
      if (message is InGameMessage)
        msg.Add("game_id", GameID);

      Socket.Send(SimpleJson.SimpleJson.SerializeObject(msg));
    }
  }
#endregion

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.WebSocket;

namespace SuperWebSocket.Samples.BasicConsole
{
  class Program
  {
    static int player_count = 0;
    static void Main(string[] args)
    {
      var appServer = new SuperSocket.WebSocket.WebSocketServer();

      //Setup the appServer
      if (!appServer.Setup(2012)) //Setup with listening port
      {
        Console.WriteLine("Failed to setup!");
        Console.ReadKey();
        return;
      }

      appServer.NewSessionConnected += new SessionHandler<WebSocketSession>(appServer_NewSessionConnected);
      appServer.SessionClosed += new SessionHandler<WebSocketSession, CloseReason>(appServer_SessionClosed);
      appServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(appServer_NewMessageReceived);
      Console.WriteLine();

      //Try to start the appServer
      if (!appServer.Start())
      {
        Console.WriteLine("Failed to start!");
        Console.ReadKey();
        return;
      }

      Console.WriteLine("The server started successfully, press key 'q' to stop it!");

      JsonObject MSG = new JsonObject();

      while (true)
      {
        MSG = new JsonObject();
        switch (Console.ReadKey().KeyChar)
        {
          case 'q':
            goto quit;
          case 'd':
            //Debug
            break;
          case 'g':
            MSG.Add("type", "change_cards");
            int[] resources = new int[] { 0, 0, 0, 0 };
            for (int i = 0; i < 4; i++)
            {
              Console.Write("r" + i.ToString() + ": ");
              int.TryParse(Console.ReadLine(), out resources[i]);
              MSG.Add("res" + (i + 1).ToString(), resources[i]);
            }
            break;
          case 't':
            MSG.Add("type", "text");
            Console.WriteLine();
            Console.Write("Message: ");
            MSG.Add("text", Console.ReadLine());
            break;
          case 'r':
            MSG.Add("type", "registered");
            int r1 = 0;
            string r2;

            Console.Write("PlayerID: ");
            int.TryParse(Console.ReadLine(), out r1);
            Console.Write("Playername: ");
            r2 = Console.ReadLine();

            MSG.Add("player_id", r1);
            MSG.Add("player_name", r2);
            MSG.Add("content_pack", "dev-pack");
            break;
          case 'n':
            MSG.Add("type", "next");
            int n1 = 0;

            Console.Write("PlayerID: ");
            int.TryParse(Console.ReadLine(), out n1);

            MSG.Add("player_id", n1);

           /* Dictionary<string, string> asdf = new Dictionary<string, string>();
            asdf.Add("key1", "val1");
            asdf.Add("key2", "val2");
            MSG.Add("DEBUG", asdf);*/
            break;
          case 'm':
            MSG.Add("type", "move");
            int m1 = 0;
            int m2 = 0;

            Console.Write("PlayerID: ");
            int.TryParse(Console.ReadLine(), out m1);
            Console.Write("LocationID: ");
            int.TryParse(Console.ReadLine(), out m2);

            MSG.Add("player_id", m1);
            MSG.Add("location_id", m2);
            break;
          case 'a':
            MSG.Add("type", "question");
            int a1 = 0;
            int a2 = 0;
            string a3 = "";
            string[] a4 = new string[0];

            Console.Write("PlayerID: ");
            int.TryParse(Console.ReadLine(), out a1);

            Console.Write("CharacterID: ");
            int.TryParse(Console.ReadLine(), out a2);

            Console.Write("Question: ");
            a3 = Console.ReadLine();

          addOption:
            Array.Resize(ref a4, a4.Length + 1);
            Console.Write("Option " + a4.Length.ToString() + ": ");
            a4[a4.Length - 1] = Console.ReadLine();
            if (Console.ReadKey().KeyChar == '+')
              goto addOption;

            MSG.Add("player_id", a1);
            MSG.Add("character_id", a2);
            MSG.Add("question", a3);
            MSG.Add("options", a4);
            break;
          //TODO: add chars for different commands
        }
        if (MSG.ContainsKey("type"))
        {
          foreach (WebSocketSession s in appServer.GetAllSessions())
          {
            s.Send(SimpleJson.SerializeObject(MSG));
          }
          Console.WriteLine("Message sent: " + SimpleJson.SerializeObject(MSG));
        }
        Console.WriteLine();
        continue;
      }
    quit:
      //Stop the appServer
      appServer.Stop();

      Console.WriteLine();
      Console.WriteLine("The server was stopped!");
      Console.ReadKey();
    }

    static void appServer_NewMessageReceived(WebSocketSession session, string message)
    {
      JsonObject Answer = new JsonObject();

      Console.WriteLine(message);

      object BUF;
      if (SimpleJson.TryDeserializeObject(message, out BUF))
      {
        JsonObject Message = (JsonObject)BUF;

        switch (Message["type"].ToString())
        {
          case "join":
            Answer.Add("type", "registered");
            Answer.Add("player_name", Message["name"].ToString());
            //int j1 = 0;
            //Console.Write("PlayerID: ");
            //int.TryParse(Console.ReadLine(), out j1);
            //Answer.Add("player_id",j1);
            Answer.Add("player_id", player_count++);
            Answer.Add("content_pack", "dev-pack");
            break;
          case "new_game":
            Answer.Add("type", "game_created");
            Answer.Add("game_id", "bullshit");
            break;
          case "request_pack_list":
            Answer.Add("type", "pack_list");
            Answer.Add("packs", new string[] { "dev-pack" });
            break;
          case "start":
            Answer.Add("type", "next");
            Answer.Add("player_id", 0);
            break;
          case "move_request":
            Answer.Add("type", "move");
            Answer.Add("player_id", Convert.ToInt32(Message["player_id"]));
            Answer.Add("location_id", Convert.ToInt32(Message["location_id"]));
            break;
          default:
            break;
        }
      }
      Console.WriteLine(SimpleJson.SerializeObject(Answer));
      session.Send(SimpleJson.SerializeObject(Answer));
    }
    static void appServer_SessionClosed(WebSocketSession session, CloseReason reason)
    {
      Console.WriteLine("Somebody left " + reason.ToString());
    }
    static void appServer_NewSessionConnected(WebSocketSession session)
    {
      Console.WriteLine("Somebody joined " + session.Path);
    }
  }
}

/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 12.07.2014
 * Time: 11:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using SimpleJson;
using WebSocket4Net;
using System.IO;
using System.Net;

namespace Client
{
	/// <summary>
	/// Description of StartForm.
	/// </summary>
	public class JoinCreateForm : Form
	{
		Label StatLabel = new Label();
		Button AddPlayerButton = new Button();
		Button StartButton = new Button();
		
		private string PlayerName;
		public int[] myIDs = new int[0];
		public string[] PlayerNames = new string[0];
		
		Connector Connection;
		
		Form SubInitForm;
		
		private bool PleaseCloseForm;
		Timer CloseTimer = new Timer();

    public void Connection_Socket_MessageReceived(object send, MessageReceivedEventArgs e)
    {
      object Obj;

      if (SimpleJson.SimpleJson.TryDeserializeObject(e.Message, out Obj))
      {
        JsonObject Message = (JsonObject)Obj;
        if (!Message.ContainsKey("type"))
          return;
        switch (Message["type"].ToString())
        {
          case "registered":
            int id = Convert.ToInt32(Message["player_id"]);
            if (id >= PlayerNames.Length)
              Array.Resize(ref PlayerNames, id + 1);
            PlayerNames[id] = Message["player_name"].ToString();
            if (Message["player_name"].ToString() == PlayerName)
            {
              Array.Resize(ref myIDs, myIDs.Length + 1);
              myIDs[myIDs.Length - 1] = id;
            }

            Invoke((MethodInvoker)delegate
                   {
                     StatLabel.Text = PlayerNames[PlayerNames.Length - 1] + " joined. Waiting for start signal";
                   });
            string ContentPackDir = Message["content_pack"].ToString();
            MainForm.Theme = ContentPackDir;
            //if (!Directory.Exists(@".\" + ContentPackDir))
            //{
            //    string Host = Connection.ContentPackHost + "/" + ContentPackDir + ".zip";
            //    WebClient c = new WebClient();
            //    c.DownloadFile(Host, @".\" + ContentPackDir + ".zip");
            //    System.IO.Compression.ZipFile.ExtractToDirectory(@".\" + ContentPackDir + ".zip", @".\" + ContentPackDir);
            //    File.Delete(@".\" + ContentPackDir + ".zip");
            //}
            break;
          case "game_created":
            Connection.GameID = Message["game_id"].ToString();

            Invoke((MethodInvoker)delegate
                   {
                     StartButton.Enabled = true;
                   });
            break;
          case "next":
            PleaseCloseForm = true;
            break;
          case "pack_list":
            JsonArray Themes = (JsonArray)Message["packs"];
            SubInitForm = new ThemeForm(Themes);
            SubInitForm.ShowDialog();

            JsonObject Answer = new JsonObject();
            Answer.Add("type", "new_game");
            Answer.Add("content_pack", (SubInitForm as ThemeForm).Theme);

            Connection.Send(Answer);
            break;
        }
      }
    }
		
		public JoinCreateForm(Connector c, string action)
		{
			this.StartPosition = FormStartPosition.Manual;
			this.Location = MainForm.StartPos;
			
			Size = new Size(290,105);
			
			PleaseCloseForm = false;
			CloseTimer.Interval = 10;
			CloseTimer.Enabled = true;
			CloseTimer.Tick += delegate 
			{ 
				if (PleaseCloseForm)
				{
					PleaseCloseForm = false;
					c.Socket.MessageReceived -= Connection_Socket_MessageReceived;
					Close();
				}
			};
			
			Connection = c;
			Connection.Socket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(Connection_Socket_MessageReceived);
			
			Controls.AddRange(new Control[2] {StatLabel, AddPlayerButton});
			
			StatLabel.Text = "Waiting for registration";
			StatLabel.Location = new Point(10, 10);
			StatLabel.Size = new Size(250,20);
			
			AddPlayerButton.Text = "Add Player";
			AddPlayerButton.Location = new Point(10,35);
			AddPlayerButton.Click += delegate(object sender, EventArgs e) 
			{ 
				SubInitForm = new AddPlayerForm();
				SubInitForm.ShowDialog();
				PlayerName = (SubInitForm as AddPlayerForm).Playername;
				
				JsonObject Message = new JsonObject();
				
				Message.Add("type","join");
				Message.Add("name",PlayerName);
				Connection.Send(Message);
				
			};
			
			JsonObject message = new JsonObject();
			if (action == "join")
			{
				AddPlayerButton.Width = 250;
			}
			if (action == "create_new")
			{
				message.Add("type","request_pack_list");
				Connection.Send(message);
				
				AddPlayerButton.Width = 120;
				
				Controls.Add(StartButton);
				StartButton.Width = 120;
				StartButton.Location = new Point(140,35);
				StartButton.Text = "Start";
				StartButton.Enabled = false;
				
				StartButton.Click += delegate(object button, EventArgs e) 
				{
					if (Connection.Socket.State == WebSocketState.Open)
					{
						JsonObject Message = new JsonObject();
						Message.Add("type","start");
						Connection.Send(Message);
					}
				};
				
			}
			
		}
	}
}

/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 02.04.2014
 * Time: 19:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using WebSocket4Net;
using SimpleJson;
using System.Linq;

namespace Client
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent(); // The InitializeComponent() call is required for Windows Forms designer support.
			InitDisplay(); //Set se Resolution
			EstablishConnection();
			
			Map = new Map();
			SideMenu = new SideMenu();
			
			//Map
			this.Controls.Add(Map.FrameLabel);
			Map.FrameLabel.BringToFront();
			Map.FrameLabel.MouseUp += OnMoveMap;
			Map.FrameLabel.MouseDown += OnMoveMap;
			Map.RedrawImage();
			//Places
			Places = new Locations(SendMoveRequest);
			InitControlGroup(Places.Menu);
			foreach(Place p in Places.AllPlaces.Values)
			{
				this.Controls.Add(p.ClickLabel);
				p.ClickLabel.Parent = Map.FrameLabel;
				p.ClickLabel.BringToFront();
			}
			OnMoveMap(Map.FrameLabel, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
			Map.FrameLabel.MouseDown += delegate 
			{
				foreach (Place p in Places.AllPlaces.Values) 
					p.ClickLabel.Visible = false;
			};
			
			//Players
			UpdatePlayerLocation();
			
			//Sidemenu
			InitControlGroup(SideMenu);
			SideMenu.Visible = true;
			foreach (RelativeLabel c in SideMenu.Controls)
				c.Click += SideMenuButtonClick;
			InitControlGroup(SideMenu.LogMenu);
			
			//FIXME: debug... REMOVE!
			Quests[0] = new Quest("Quest 1", new int[2] {1,2},new Resources(2,3,1,4),2,3,new Item[0]);
			Quests[1] = new Quest("Quest 1", new int[2] {1,2},new Resources(2,3,1,4),2,3,new Item[0]);
			Quests[2] = new Quest("Quest 1", new int[2] {1,2},new Resources(2,3,1,4),2,3,new Item[0]);
			//END
		}
		
		Connector Connection;
		
		int CurrentPlayer = 0;
		int ClientID = 0;
		Player[] Players;
		
		Quest[] Quests = new Quest[3];		
		
		ActingOptionMenu ActingOptionMenu;
		Locations Places;
		Map Map;
		SideMenu SideMenu;
		
		public static string Theme = "Default";
		
		public static Point StartPos = new Point(100,100);
		public static Size ClientResolution = new Size(1366,768); //Adjustable
		
		void MainFormResizeEnd(object sender, EventArgs e)
		{
			this.Width =ClientResolution.Width;
			this.Height=ClientResolution.Height;
		}
		
		void SideMenuButtonClick (object sender, EventArgs e)
		{
			//UNDONE: Sidemenu Buttons			
			Control Sender = Array.Find(SideMenu.Controls, entry => entry.Equals(sender));
			SideMenu.SubGroups[0].Dispose();
			switch (Sender.Name)
			{
				case "TasksButton":
					SideMenu.SubGroups[0] = new QuestMenu(Quests, Players[ClientID]);
					InitControlGroup(SideMenu.SubGroups[0]);
					SideMenu.Visible=true;
					break;
				case "StatsButton":
					break;
				case "OptionsButton":
					break;
				case "LogButton":
					SideMenu.LogMenu.Visible = !SideMenu.LogMenu.Visible;
						
//					if (SideMenu.SubGroups[1].Visible)
//					{
//						SideMenu.SubGroups[1].Dispose();
//						SideMenu.SubGroups[1].Visible = false;
//					}
//					else
//					{
//						SideMenu.SubGroups[1] = new LogMenu();
//						InitControlGroup(SideMenu.SubGroups[1]);
//						SideMenu.Visible = true;
//					}
					break;
				case "ExitButton":
					Close();
					break;
			}
		}
		
		void InitControlGroup(ControlGroup c) //Add the controls of a ControlGroup
		{
			foreach (ControlGroup c2 in c.SubGroups)
			{
				InitControlGroup(c2);
			}
			this.Controls.AddRange(c.Controls);
			c.Visible = c.Visible;
			c.Location = c.Location;
			try
			{
				c.GetControl("BackgroundLabel").SendToBack();
			}
			catch{}
		}
		
		void OnMoveMap(object sender, MouseEventArgs e) // Recalculates the Position of the Places on Mapmovement
		{
			foreach(Place p in Places.AllPlaces.Values)
			{
				p.CheckVisibility(Map.GetInnerbounds());
			}
			Place[] ps = Array.FindAll(Places.AllPlaces.Values.ToArray(), entry => entry.Visible == true);
			foreach(Place p in ps)
			{
				p.ClickLabel.Location = Point.Add(p.ClickLabel.RelativeLocation, (Size)Map.MapPos);
			}
			
			Places.Menu.Visible = false;
			SideMenu.SubGroups[0].Dispose();
		}
		
		public void UpdatePlayerLocation(Player player, int newLocationID)
		{
//			if (player.LocationID != 0)
//				Places.AllPlaces[player.LocationID].Players --;
//			Places.AllPlaces[newLocationID].Players ++;
			
			player.LocationID = newLocationID;
			UpdatePlayerLocation();
		}
		
		public void UpdatePlayerLocation()
		{
			SortedList<int,int> PpL = new SortedList<int, int>();
			foreach (Place p in Places.AllPlaces.Values)
			{
				PpL.Add(p.ID,0);
			}
			
			foreach(Player p in Players)
			{
				PpL[p.LocationID] ++;
			}
			
			SortedList<int,int> PpL_left = new SortedList<int, int>(PpL);
			
			GraphicElement[] PlayerGraphics = new GraphicElement[Players.Length];
			
			int locID;
			double angle;
			double startAngle = Math.PI/2;
			double length = 50;
			
			Point pos;
			
			for (int i = 0; i < Players.Length; i++)
			{
				locID = Players[i].LocationID;
				angle = 2*Math.PI/(double)PpL[locID];
				
				if (angle == 2*Math.PI)
				{
					pos = Places.AllPlaces[locID].Location;
				}
				else
				{
					pos = new Point((int)(Math.Sin(PpL_left[locID] * angle + startAngle)*length),(int)(Math.Cos(PpL_left[locID] * angle + startAngle)*length));
					pos.X += Places.AllPlaces[locID].Location.X;
					pos.Y += Places.AllPlaces[locID].Location.Y;
//					Point.Add(pos,new Size(Places.AllPlaces[locID].Location));
					PpL_left[locID] --;
				}
				
				pos = Point.Subtract(pos, new Size(Players[i].Image.Width/2, Players[i].Image.Height));
				PlayerGraphics[i] = new GraphicElement(Players[i].Image, pos);
			}
			
			Map.ResetBackground();
			
			Map.AddGraphic(PlayerGraphics);
			
			Map.RedrawImage();
		}
		
		public void InitDisplay()
		{
			DisplayManager DisplayManager = new DisplayManager();
			DisplayManager.ShowDialog();
			ClientResolution = DisplayManager.Resolution;
			
			if (DisplayManager.ResolutionChanged)
			{
				Closed += delegate(object sender, EventArgs e) { DisplayManager.SetOriginalDEVMODE(); };
			}
			
			FormBorderStyle = FormBorderStyle.None;
			this.WindowState = FormWindowState.Maximized;
		}
		
		public void AddLogEntry (string entry)
		{
			LogMenu.AddLogEntry(entry);
			SideMenu.LogMenu.UpdateLabel();
		}
		
		//################################################################################################################### Stuff
		
		public static string StructureString (string Unstructured, char[] Separators)
		{
			string[] Buf = Unstructured.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
			return StructureString(Buf);
		}
		
		public static string StructureString (string[] Unstructured)
		{
			string Out = "";
			for (int i = 0; i<Unstructured.Length; i++)
			{
				Out += Unstructured[i];
				if (i < Unstructured.Length - 2)
					Out += ", ";
				if (i == Unstructured.Length - 2)
					Out += " and ";
			}
			return Out;
		}
		
		#region Websocket
		public bool SendMoveRequest (int location_id, string mode)
		{
			if (Array.Exists(Connection.ClientIDs, entry => entry == CurrentPlayer))
			{
				JsonObject Message = new JsonObject();
				Message.Add("type","move_request");
				Message.Add("location_id",location_id);
				Message.Add("player_id",CurrentPlayer);
				Message.Add("mode",mode);
				Connection.Send(Message);
				return true;
			}
			return false;
		}
		
		public bool SendResponse (int answer)
		{
			if (Array.Exists(Connection.ClientIDs, entry => entry == CurrentPlayer))
			{
				JsonObject Message = new JsonObject();
				Message.Add("type","answer");
				Message.Add("answer",answer);
				Connection.Send(Message);
				return true;
			}
			return false;
		}
		
		public void HandleMessageWrapper (MessageReceivedEventArgs e)
		{
			try
			{
				Action DOIT = () => HandleMessage(e);
				if (InvokeRequired)
					Invoke(DOIT);
				else
					HandleMessage(e);
			}
			catch (KeyNotFoundException)
			{
				if (!e.Message.Contains("game_created") && !e.Message.Contains("next") && !e.Message.Contains("registered") && !e.Message.Contains("card"))
					MessageBox.Show("Bad Message: "+e.Message); // TODO: Destroy Debug (Messaging)
			}
		}
		
		public bool HandleMessage (MessageReceivedEventArgs e)
		{
			object Obj;
			JsonObject Message;
			
			if (SimpleJson.SimpleJson.TryDeserializeObject(e.Message, out Obj))
				Message = (JsonObject)Obj;
			else
				return false;
			
			if (!Message.ContainsKey("type"))
				return false;
			
			int p_id = 0;
			int c_id = 0;
			int l_id = 0;
			
			if (Message.ContainsKey("player_id"))
				p_id = Convert.ToInt32(Message["player_id"]);
			if (Message.ContainsKey("character_id"))
				c_id = Convert.ToInt32(Message["character_id"]);
			if (Message.ContainsKey("location_id"))
				l_id = Convert.ToInt32(Message["location_id"]);
			
			switch (Message["type"].ToString())
			{
				case "text": //HANDLED
					AddLogEntry(Message["text"].ToString());
					return true;
				case "character": //HANDLED
					AddLogEntry(Character.GetCharacter(c_id).Name + ": " + Message["text"].ToString());
					return true;
				case "move": //HANDLED
					UpdatePlayerLocation(Players[p_id], l_id);
					AddLogEntry(Players[p_id].Name + " moved to " + Places.AllPlaces[l_id].Name);
					return true;
				case "next": //HANDLED
					CurrentPlayer = p_id;
					try
					{
						AddLogEntry("It is " + Players[p_id].Name + "'s turn");
					}
					catch {	}
					return true;
				case "question": //HANDLED
					JsonArray optionsBuf = (JsonArray)Message["options"];
					string[] options = new string[optionsBuf.Count];
					AddLogEntry(Players[p_id].Name + " has to decide on the following: " +  Message["question"].ToString() +" These options are available for selection:");
					for (int i = 0; i < optionsBuf.Count; i++)
					{
						options[i] = optionsBuf[i].ToString();
						AddLogEntry(options[i]);
					}
					if (Array.Exists(Connection.ClientIDs, entry => entry == p_id))
					{
						ActingOptionMenu = new ActingOptionMenu(p_id, c_id, Message["question"].ToString(), options, SendResponse);
						InitControlGroup(ActingOptionMenu);
						ActingOptionMenu.Visible = true;
					}
					return true;
				case "change_cards": //HANDLED // TODO: Replace Keys
					Resources ResourcesBUF = new Resources(
						Convert.ToInt32(Message["res1"]),
						Convert.ToInt32(Message["res2"]),
						Convert.ToInt32(Message["res3"]),
						Convert.ToInt32(Message["res4"]));
					Players[p_id].Resources += ResourcesBUF;
					AddLogEntry(Players[p_id].Name + " " + ResourcesBUF.ToString());
					return true;
				case "item_gained": //HANDLED
					Players[p_id].AddItem(new Item(Message["item_name"].ToString()));
					AddLogEntry(Players[p_id].Name + " gained " + Message["item_name"].ToString());
					return true;
				case "item_lost": //HANDLED
					Players[p_id].DeleteItem(new Item(Message["item_name"].ToString()));
					AddLogEntry(Players[p_id].Name + " lost " + Message["item_name"].ToString());
					return true;
				case "win":
					AddLogEntry(Players[p_id].Name + " won.");
					return true;
				default:
					return false;
			}
		}
		
		public void SendMessage (JsonObject message)
		{
			Connection.Send(message);
		}
		
		public void EstablishConnection()
		{
			
			ConnectionForm InitForm1;
			WaitForConnectionForm InitForm2;
			ActionForm InitForm3;
			JoinCreateForm InitForm4;
		init:
			InitForm1 = new ConnectionForm();
			InitForm1.ShowDialog();
			
			try 
			{
				Connection = new Connector(InitForm1.Host);
				Connection.Socket.MessageReceived += delegate (object sender, MessageReceivedEventArgs e) {HandleMessageWrapper(e);};
			} 
			catch (ArgumentException) 
			{
				goto init;
			}
			
			InitForm2 = new WaitForConnectionForm(Connection);
			InitForm2.ShowDialog();
			
			if (Connection.Socket.State != WebSocketState.Open)
				goto init;
			
		startgame:
			InitForm3 = new ActionForm();
			InitForm3.ShowDialog();
			if (InitForm3.Action == "")
				goto startgame;
			if (InitForm3.Action == "join")
				Connection.GameID = InitForm3.GameID;
			
			InitForm4 = new JoinCreateForm(Connection, InitForm3.Action);
			InitForm4.ShowDialog();
			if (InitForm4.myIDs.Length == 0)
				goto startgame;
			
			Connection.ClientIDs = InitForm4.myIDs;
			Players = new Player[InitForm4.PlayerNames.Length];
			for (int i = 0; i<InitForm4.PlayerNames.Length; i++)
			{
				if (InitForm4.PlayerNames[i] == null) //DEBUG??
					InitForm4.PlayerNames[i] = "Player"+i.ToString();
				Players[i] = new Player(i, InitForm4.PlayerNames[i]);
			}
		}
		#endregion
	}
}

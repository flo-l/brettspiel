/*
 * Created by SharpDevelop.
 * User: bberg
 * Date: 24.04.2014
 * Time: 15:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Client
{
	/// <summary>
	/// Description of QuestDisplay.
	/// </summary>
	public class QuestMenu:ControlGroup
	{
		public static int MaxDisplayed = 5;
		private int CurrentStartID = 0;
		
		private int Offset = 10;
		private int Height = 100;
		private int Width = 310;
		private int ButtonHeight = 20;
		private int NameLabelHeight = 68;
		
		public QuestMenu(Quest[] quests, Player player)
		{
//			Location = new Point(MainForm.res.Width-Width,Map.StartPoint.Y);//Adjustable
			Location = new Point(0,SideMenu.EndPoint.Y);//Adjustable
			
			CurrentStartID = 0;
			
			if (MaxDisplayed>=quests.Length)
			{	
				Add(new RelativeLabel(new Point (0,0), new Size(Width,Offset+quests.Length*(Height+Offset)+NameLabelHeight+Offset)), "BackgroundLabel");
				for (int i=0;i<quests.Length;i++)
				{
					Add(CreateQuestGroupControl(quests[i],player,i,0));
				}
			}
			else
			{
				Add(new RelativeLabel(new Point (0,0), new Size(Width,Offset+MaxDisplayed*(Height+Offset)+2*(ButtonHeight+Offset)+NameLabelHeight+Offset)), "BackgroundLabel");
				for (int i=CurrentStartID;i<CurrentStartID+MaxDisplayed;i++)
				{
					Add(CreateQuestGroupControl(quests[i],player,i,Offset+ButtonHeight));
				}
				Add(new RelativeLabel(new Point(Offset,NameLabelHeight+2*Offset), new Size(Width-2*Offset,ButtonHeight)),"UpButton");
				GetControl("UpButton").Text = "^";
				GetControl("UpButton").Click += delegate 
				{
					if(CurrentStartID>0) 
						CurrentStartID--;
					UpdateVisibleQuests(quests,player);
				};
				Add(new RelativeLabel(new Point(Offset, SubGroups[MaxDisplayed-1].Location.Y+Offset+Height-SideMenu.EndPoint.Y), new Size(Width-2*Offset,ButtonHeight)),"DownButton");
				GetControl("DownButton").Text = "v";
				GetControl("DownButton").Click += delegate 
				{ 
					if(CurrentStartID+MaxDisplayed<quests.Length)
						CurrentStartID++; 
					UpdateVisibleQuests(quests,player);
				};
			}
			GetControl("BackgroundLabel").BackColor = Color.Lime;
			
			Add(new RelativeLabel(new Point(Offset,Offset), new Size(Width - 2*Offset,NameLabelHeight)), "NameLabel");
			GetControl("NameLabel").Text = "Quests";
		}
		
		public void UpdateVisibleQuests(Quest[] quests, Player player)
		{
			for (int i=0; i<MaxDisplayed;i++)
			{
				string [] TextBuf=QuestText(quests[i+CurrentStartID],player);
				SubGroups[i].GetControl("TextLabel").Text = TextBuf[0];
				SubGroups[i].GetControl("RequirementsLabel").Text = TextBuf[1];
				SubGroups[i].GetControl("ImageLabel").BackgroundImage = Character.GetCharacter(i+CurrentStartID).CharacterImage;
			}
		}
		
		public string[] QuestText(Quest quest, Player player)
		{
			string TextBuf2 = Character.GetCharacter(0).Name+" "+"Charakter "+quest.CharacterID.ToString()+": "+quest.Text;
			
			string TextBuf = "";
			TextBuf += "Location: " + player.LocationID + " =? " + quest.LocationID + " ";
			
			TextBuf += "Resources: ";
			if (quest.ResourcesNeeded > new Resources())
				for (int i = 0; i<quest.ResourcesNeeded.All.Length;i++)
					TextBuf += player.Resources.All[i].ToString() + "/" + quest.ResourcesNeeded.All[i].ToString() +" ";
			else
				TextBuf += "none ";
			
			TextBuf += "Players: ";
			if (quest.PlayerIDs.Length == 0) 
				TextBuf += "all ";
			else
				foreach (int i in quest.PlayerIDs)
					TextBuf += i.ToString() + " ";
			
			TextBuf += "Items: ";
			if (quest.NeededItems.Length == 0)
				TextBuf += "none ";
			else
				foreach (Item i in quest.NeededItems)
					TextBuf += i.Name + " ";
			
			return new string[2] {TextBuf2, TextBuf};
		}
		
		public ControlGroup CreateQuestGroupControl(Quest quest, Player player, int SlotNum, int SpecOffset)
		{
			ControlGroup c = new ControlGroup(new Point(Location.X,SlotNum*(Height+Offset)+SideMenu.EndPoint.Y+2*Offset+NameLabelHeight+SpecOffset));
			
			c.Add(new RelativeLabel(new Point(Offset,0), new Size(Height,Height)), "ImageLabel");
			c.Add(new RelativeLabel(new Point(Offset+Height+Offset,0),new Size(Width-3*Offset-Height,50)),"TextLabel");
			c.Add(new RelativeLabel(new Point(Offset+Height+Offset,50),new Size(Width-3*Offset-Height,50)),"RequirementsLabel");
			
			string[] TextBuf = QuestText(quest,player);
			
			c.GetControl("TextLabel").Text = TextBuf[0];
			c.GetControl("RequirementsLabel").Text = TextBuf[1];
			c.GetControl("ImageLabel").BackgroundImage = Character.GetCharacter(quest.CharacterID).CharacterImage;
			
			return c;
		}
	}
}

/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 15.07.2014
 * Time: 13:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using WebSocket4Net;
using System.Linq;

namespace Client
{
	/// <summary>
	/// Description of OptionMenu.
	/// </summary>
	public class ActingOptionMenu : ControlGroup
	{
		public Size GroupSize
		{
			get
			{
				return new Size(300, AllOptions.Length * (Offset + OptionHeight) + 2*Offset + QuestionTextHeight);
			}
		}
		
		public Point StartPoint
		{
			get
			{
				return new Point(Map.EndPoint.X-GroupSize.Width, MainForm.ClientResolution.Height-GroupSize.Height);
			}
		}
		
		private static int Offset = 10;
		private static int OptionHeight = 50;
		private static int QuestionTextHeight = 100;
		
		private static ResourceReader resources = new ResourceReader("options");
		private static SortedList<string, Image> ButtonImages = resources.ReadAllImages();
		
		public int AskedPlayerID;
		public int CharacterID;
		public string Question;
		public string[] AllOptions;
		
		public ActingOptionMenu(int pID, int cID, string q, string[] o, Func<int,bool> ResponseFunc)
		{
			AskedPlayerID = pID;
			CharacterID = cID;
			Question = q;
			AllOptions = o;
			
			Location = StartPoint;
			Add(new RelativeLabel(new Point(0,0),GroupSize), "BackgroundLabel");
			GetControl("BackgroundLabel").BackColor = Color.Lime;
			
			Add(new RelativeLabel(new Point(2*Offset+QuestionTextHeight,Offset), new Size(GroupSize.Width-3*Offset-QuestionTextHeight,QuestionTextHeight)),"QuestionTextLabel");
			Add(new RelativeLabel(new Point(Offset,Offset), new Size(QuestionTextHeight,QuestionTextHeight)),"QuestionCharacterLabel");
			GetControl("QuestionCharacterLabel").BackgroundImage = Character.GetCharacter(CharacterID).CharacterImage;
			GetControl("QuestionTextLabel").Text = Question;
			
			for (int i = 0; i < AllOptions.Length; i++)
			{
				Add(CreateOptionGroupControl(i, ResponseFunc));
			}
		}
		
		public ControlGroup CreateOptionGroupControl(int ind, Func<int,bool> ResponseFunc)
		{
			ControlGroup c = new ControlGroup(new Point(Location.X,Location.Y + Offset + QuestionTextHeight + Offset + ind * (OptionHeight+Offset) ));
			
			c.Add(new RelativeLabel(new Point(Offset,0), new Size(OptionHeight,OptionHeight)), "ButtonLabel");
			c.Add(new RelativeLabel(new Point(Offset+OptionHeight+Offset,0),new Size(GroupSize.Width-3*Offset-OptionHeight,OptionHeight)),"TextLabel");
			
			c.GetControl("TextLabel").Text = AllOptions[ind];
			c.GetControl("ButtonLabel").Click += delegate(object sender, EventArgs e) 
			{
				ResponseFunc(ind);
				Dispose();
			};
			
			bool KeywordFound = false;
			string[] Keywords = ButtonImages.Keys.ToArray();
			foreach (string kw in Keywords)
			{
				if (KeywordFound = AllOptions[ind].Contains(kw))
				{
					c.GetControl("ButtonLabel").BackgroundImage = ButtonImages[kw];
					break;
				}
			}
			if (!KeywordFound)
				c.GetControl("ButtonLabel").BackgroundImage = ButtonImages["misc"];
			
			return c;
		}
	}
}

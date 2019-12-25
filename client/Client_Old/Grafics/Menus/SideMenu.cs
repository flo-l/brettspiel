/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 16.04.2014
 * Time: 16:20
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
	/// Description of SideMenu.
	/// </summary>
	public class SideMenu:ControlGroup
	{
		static private System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SideMenu));
		
		static private string[] label_names = new string[5] {"TasksButton", "StatsButton", "OptionsButton", "LogButton", "ExitButton"};
		
		static public Size GroupSize
		{
			get
			{
//				return new Size(2*Offset.Width+ButtonSize.Width, Map.Size.Height);
				if (Sideways)
					return new Size(Offset.Width*(label_names.Length+1) + ButtonSize.Width*(label_names.Length), 2*Offset.Height+ButtonSize.Height);
				else
					return new Size(2*Offset.Width+ButtonSize.Width, Offset.Height*(label_names.Length+1) + ButtonSize.Height*(label_names.Length));
			}
		}
		
		static public Point EndPoint
		{
			get
			{
				return Point.Add(StartPoint,GroupSize);
			}
		}
		
		static public Point StartPoint
		{
			get
			{
				return new Point(0,Map.StartPoint.Y); //Adjustabele
			}
		}
		
		static private Size Offset = new Size(10,10); //Adjustabele
		static private Size ButtonSize = new Size(50,50); //Adjustabele
		
		static private bool Sideways = true;
		
		public LogMenu LogMenu = new LogMenu();
		
		public SideMenu()
		{
			Location = StartPoint;
			
			Add(new RelativeLabel(new Point(0,0), GroupSize),"BackgroundLabel");
			GetControl("BackgroundLabel").BackColor = Color.Lime;
			
			for (int i = 0; i < label_names.Length; i++)
			{
				if (Sideways)
					Add(new RelativeLabel(new Point (Offset.Width+i*(Offset.Width+ButtonSize.Width), Offset.Height),
					                      new Size(ButtonSize.Width,ButtonSize.Height)), label_names[i]);
				else
					Add(new RelativeLabel(new Point (Offset.Width,Offset.Height+i*(Offset.Height+ButtonSize.Height)),
					                      new Size(ButtonSize.Width,ButtonSize.Height)), label_names[i]);
				GetControl(label_names[i]).BackgroundImage = (Image)(resources.GetObject(label_names[i]+".Image"));
			}
			
			foreach (Control c in Controls)
			{
				if (c.Name.Contains("Button"))
					c.BackColor = Color.Green;
			}
			
			Add(new ControlGroup(new Point(0,0)));
		}
	}
}

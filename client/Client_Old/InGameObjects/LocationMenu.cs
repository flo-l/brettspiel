/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 16.04.2014
 * Time: 12:15
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
	/// OBSOLETE ControlGroup is cooler
	/// </summary>
	public class LocationMenu:ControlGroup
	{
		private static int Offset = 10;
		private static int Width = 300;
		private static int Height = 480;
		
		public int CurrentLocation;
		
		public LocationMenu()
		{
			Location = new Point(0,60);
			
			Add(new RelativeLabel(new Point (0,0), new Size(Width,Height)), "BackgroundLabel"); 
			GetControl("BackgroundLabel").BackColor = Color.Lime;
			Add(new RelativeLabel(new Point(Offset,Offset), new Size(Width-2*Offset,30)), "NameLabel");
			Add(new RelativeLabel(new Point(Offset,50), new Size(Width-2*Offset,380)), "TextLabel");
			Add(new RelativeLabel(new Point(Offset,440), new Size((Width-3*Offset)/2,30)), "ActiveLabel");
			Add(new RelativeLabel(new Point(2*Offset + (Width-3*Offset)/2,440), new Size((Width-3*Offset)/2,30)), "PassiveLabel");
			
			GetControl("ActiveLabel").Text = "Investigate";
			GetControl("PassiveLabel").Text = "Hide";
		}
		
		public void CallMenu(Place p)
		{
			GetControl("TextLabel").Text = p.Description;
			GetControl("NameLabel").Text = p.Name;
			
			int XCoord = p.ClickLabel.Midpoint.X;
			int YCoord = p.ClickLabel.Midpoint.Y;
			
			bool ifLeft = (Map.Midpoint.X) > p.ClickLabel.Midpoint.X + Map.StartPoint.X;
			bool ifUp = (Map.Midpoint.Y) > p.ClickLabel.Midpoint.Y + Map.StartPoint.Y;
			
			if (ifLeft)
				XCoord += p.ClickLabel.Width / 2 + Offset +  Map.StartPoint.X;
			else
				XCoord -= p.ClickLabel.Width / 2 + Offset + GetControl("BackgroundLabel").Width -  Map.StartPoint.X;
			if (ifUp)
				YCoord = Map.StartPoint.Y + Offset;
			else
				YCoord = Map.StartPoint.Y + Map.Size.Height - Offset - GetControl("BackgroundLabel").Height;
			Location = new Point(XCoord,YCoord);
			Visible = true;
		}
	}
}

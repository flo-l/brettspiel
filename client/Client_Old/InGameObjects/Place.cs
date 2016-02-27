/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 16.04.2014
 * Time: 11:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Client
{
	/// <summary>
	/// Description of Place.
	/// </summary>
	public class Place
	{
		public int Players = 0;
		
		private string _Name;
		private string _Description;
		private int _ID;
		private Point _Location;
		
		public int ID
		{
			get
			{
				return _ID;
			}
		}
		
		public string Name
		{
			get
			{
				return _Name;
			}
		}
		public string Description
		{
			get
			{
				return _Description;
			}
		}
		public Point Location
		{
			get
			{
				return _Location;
			}
		}
		
		public RelativeLabel ClickLabel;
		public bool Visible;
		
		public static Size Size = new Size(100, 100);	//Adjustable
		
		public Place(int id, string name, Point relatLoc, string description)
		{
			_Location = relatLoc;
			_Name = name;
			_Description = description;
			_ID = id;
			
			ClickLabel = new RelativeLabel(new Point (relatLoc.X - Size.Width/2, relatLoc.Y - Size.Height/2), Size);
			
			ClickLabel.Size = Size;
			ClickLabel.BackColor = Color.Transparent;
		}
		
		public void CheckVisibility(Rectangle rect)//wip merge with map?? WORKS QUITE FINE EITHER
		{
			if(rect.Contains(new Rectangle(ClickLabel.RelativeLocation, Size)))
			{
				Visible=true;
				ClickLabel.BringToFront();
			}
			else
			{
				Visible=false;
				ClickLabel.SendToBack();
			}
			ClickLabel.Visible=Visible;
		}
	}
}

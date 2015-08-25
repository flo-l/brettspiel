/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 15.04.2014
 * Time: 22:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Linq;

namespace Client
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	
	public class Locations
	{
		public SortedList<int,Place> AllPlaces = new SortedList<int, Place>();
		
		private static ResourceReader resources = new ResourceReader("locations");
		private static SortedList<int, SortedList<string,string>> CSV = resources.ReadCSV("locations.csv");
		
		public LocationMenu Menu = new LocationMenu();
		
		public Locations (Func<int,string,bool> sendMoveRequest)
		{
			foreach (SortedList<string,string> LocationSpecs in CSV.Values)
			{
				int id = Convert.ToInt32(LocationSpecs["id"]);
				Point l = new Point (Convert.ToInt32(LocationSpecs["x"]),Convert.ToInt32(LocationSpecs["y"]));
				AllPlaces.Add(id ,new Place(id,LocationSpecs["name"],l,LocationSpecs["description"]));
				AllPlaces[id].ClickLabel.Click += new EventHandler(Locations_Click);
			}
			Menu.GetControl("ActiveLabel").Click += delegate(object sender, EventArgs e) 
			{
				sendMoveRequest(Menu.CurrentLocation,"active"); 
			};
			Menu.GetControl("PassiveLabel").Click += delegate(object sender, EventArgs e) 
			{
				sendMoveRequest(Menu.CurrentLocation,"passive"); 
			};
		}

		void Locations_Click(object sender, EventArgs e)
		{
			Place p = Array.Find(AllPlaces.Values.ToArray(), entry => entry.ClickLabel.Equals(sender));
			Menu.CallMenu(p);
			Menu.CurrentLocation = p.ID;
		}
	}
	
}

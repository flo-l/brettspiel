/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 15.04.2014
 * Time: 21:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Drawing;

namespace Client
{
	/// <summary>
	/// Description of player.
	/// </summary>
	/// 
	
	public class Player
	{
		public string Name = "Sir Blah the debugging soldier";
		public Item[] Items = new Item[0];
		public Resources Resources = new Resources(); //3 Base resources + Honor // SPLIT IT?
		
		private static ResourceReader resources = new ResourceReader("players");
		public Bitmap Image;
		
		public int LocationID = 1;	//ID of the Players current Position. 0 may equal the start point

		public Player(int i, string name)
		{
			Name = name;
			Image = (Bitmap) resources.ReadImage("player"+i.ToString() + ".png");
		}
		
		public void AddItem(Item item) //Adds an item
		{
			Array.Resize(ref Items, Items.Length+1);
			Items[Items.Length-1]=item;
		}
		
		public void DeleteItem(Item item) //Deletes an item
		{
			int ind = Array.FindLastIndex(Items, entry => entry.Name == item.Name);
			if (!int.Equals(ind,null))
			{
				Array.Copy(Items,ind+1,Items,ind,Items.Length-ind-1);
				Array.Resize(ref Items,Items.Length-1);
			}
		}
	}
}

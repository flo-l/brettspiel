/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 23.04.2014
 * Time: 18:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Client
{
	/// <summary>
	/// Description of Quest.
	/// </summary>
	public class Quest
	{
		public string Text = "";
		public int[] PlayerIDs = new int[0];
		public Resources ResourcesNeeded = new Resources();
		public int LocationID = 0;
		public int CharacterID = 0;
		public Item[] NeededItems = new Item[0];
		
		public Quest(string txt, int[] players, Resources resources, int locationID, int charID, Item[] items)
		{
			Text=txt;
			PlayerIDs=players;
			ResourcesNeeded = resources;
			LocationID=locationID;
			CharacterID = charID;
			NeededItems = items;
		}
	}
}

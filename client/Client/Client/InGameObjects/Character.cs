/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 15.07.2014
 * Time: 18:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Client
{
	/// <summary>
	/// Description of Character.
	/// </summary>
	public class Character
	{
		public string Name;
		public Image CharacterImage;
		public int ID;
		
		private static ResourceReader resources = new ResourceReader("characters");
		
		private static SortedList<int, SortedList<string,string>> CSV = resources.ReadCSV("characters.csv");
		private static SortedList<string, Image> AllImages = resources.ReadAllImages();
		
		public Character(int id)
		{
			ID = id;
			
			try
			{
				SortedList<string,string> CharSpecs = CSV[ID];
				Name = CharSpecs["name"];
				CharacterImage = (Image)AllImages[ID.ToString()];
			}
			catch
			{
				MessageBox.Show("Character load failed " + ID.ToString());
			}
		}
		
		public static Character GetCharacter(int id)
		{
			return new Character(id);
		}
	}
}

/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 23.04.2014
 * Time: 17:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;

namespace Client
{
	/// <summary>
	/// Description of Item.
	/// </summary>
	public class Item
	{
		private string _Name = "";
		private Image _Picture = new Bitmap(0,0);
		
		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
			}
		}
		public Image Picture
		{
			get
			{
				return _Picture;
			}
			set
			{
				_Picture = value;
			}
		}
		
		public Item(string name, Image pic)
		{
			Name = name;
			Picture = pic;
		}
		
		public Item(string name)
		{
			Name = name;
		}
	}
}

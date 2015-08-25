/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 23.07.2014
 * Time: 13:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace Client
{
	/// <summary>
	/// Description of ResourceReader.
	/// </summary>
	public class ResourceReader
	{
		private string Path;
		
		private static string ImageExtension = ".jpg";
		
		public ResourceReader(string path)
		{
			Path = @".\"+MainForm.Theme+@"\"+path+@"\";
		}
		
		public ResourceReader()
		{
			Path = @".\"+MainForm.Theme+@"\";
		}
		
		public SortedList<int, SortedList<string,string>> ReadCSV(string pathEnding)
		{
			if (!pathEnding.EndsWith(".csv") || !File.Exists(Path+pathEnding))
				return new SortedList<int, SortedList<string,string>>();
			
			string[] AllLines;
			string[] LineBuf;
			SortedList<int, SortedList<string,string>> Out = new SortedList<int, SortedList<string,string>>();
			SortedList<string,string> LineList = new SortedList<string,string>();
			
			string[] Keys;
			
			AllLines = File.ReadAllLines(Path+pathEnding);
			Keys = AllLines[0].Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
			
			for (int y = 1; y < AllLines.Length; y++)
			{
				LineBuf = AllLines[y].Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
				LineList = new SortedList<string,string>();
				for (int x = 0; x < Keys.Length; x++)
				{
					LineList.Add(Keys[x],LineBuf[x]);
				}
				Out.Add(Convert.ToInt32(LineBuf[0]), LineList);
			}
			
			return Out;
		}
		
		public Image ReadImage(string pathEnding)
		{
//			if (!pathEnding.EndsWith(ImageExtension) || !File.Exists(Path+pathEnding))
			if (!File.Exists(Path+pathEnding))
				return new Bitmap(0,0);
			return (Image)(new Bitmap(Path+pathEnding));
		}
		
		public SortedList<string,Image> ReadAllImages()
		{
			string[] ImagePaths = Array.FindAll(Directory.GetFiles(Path), entry => entry.EndsWith(ImageExtension));
			
			SortedList<string,Image> AllImages = new SortedList<string,Image>();
			
			foreach (string path in ImagePaths)
			{
				AllImages.Add(path.Replace(Path,"").Replace(ImageExtension,""),(Image)(new Bitmap(path)));
			}
			return AllImages;
		}
	}
}

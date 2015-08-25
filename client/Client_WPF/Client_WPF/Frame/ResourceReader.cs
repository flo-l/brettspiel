using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Client
{
	public class ResourceReader
	{
		private string Path;
		
		private static string ImageExtension = ".jpg";
		
		public ResourceReader(string theme, string path)
		{
			Path = @".\"+theme+@"\"+path+@"\";
		}

    public ResourceReader(string theme)
		{
			Path = @".\"+theme+@"\";
		}
		
		public SortedList<int, SortedList<string,string>> ReadCSV(string fileName)
		{
			if (!File.Exists(Path+fileName))
        throw new FileNotFoundException("Resourcemanger could not find " + fileName);
			
			string[] AllLines;
			string[] LineBuf;
			SortedList<int, SortedList<string,string>> Out = new SortedList<int, SortedList<string,string>>();
			SortedList<string,string> LineList = new SortedList<string,string>();
			
			string[] Keys;
			
			AllLines = File.ReadAllLines(Path+fileName);
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
		
		public WriteableBitmap ReadImage(string fileName)
		{
      if (!File.Exists(Path + fileName))
        throw new FileNotFoundException("Resourcemanger could not find " + fileName);
			return new WriteableBitmap(new BitmapImage(new Uri(Path+fileName)));
		}
		
		public SortedList<string,WriteableBitmap> ReadAllImages()
		{
			string[] ImagePaths = Array.FindAll(Directory.GetFiles(Path), entry => entry.EndsWith(ImageExtension));
			
			SortedList<string,WriteableBitmap> AllImages = new SortedList<string,WriteableBitmap>();
			
			foreach (string path in ImagePaths)
			{
				AllImages.Add(
                    path.Replace(Path,"").Replace(ImageExtension,""),
                    new WriteableBitmap(new BitmapImage(new Uri(path))));
			}
			return AllImages;
		}
	}
}

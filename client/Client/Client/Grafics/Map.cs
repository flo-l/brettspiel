/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 15.04.2014
 * Time: 17:57
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
	/// Description of map.
	/// </summary>
	public class Map
	{
		public Label FrameLabel = new Label();
		public Point MapPos = new Point(0,0);
		
		private static ResourceReader resources = new ResourceReader();
		private static Bitmap MapImageBase = (Bitmap)resources.ReadImage("map.png");
		private static Bitmap MapImage = new Bitmap(MapImageBase);
		
		public static Point StartPoint 
		{
			get
			{
//				return new Point(SideMenu.EndPoint.X,SideMenu.StartPoint.Y);
				return new Point(0,0);
			}
		}
		
		public static Size Size
		{
			get
			{
//				return new Size(1000,600);	//Adjustable
				return MainForm.ClientResolution;	//Adjustable
			}
		}
		
		public static Point EndPoint
		{
			get
			{
				return Point.Add(StartPoint, Size);
			}
		}
		
		public static Point Midpoint
		{
			get
			{
				return new Point (StartPoint.X+Size.Width/2,StartPoint.Y+Size.Height/2);
			}
		}
		
		//MouseTrack
		private Size MouseStart;
		private Size Delta;
		private bool MouseActive = false;
		
		public Map()
		{
			FrameLabel.Location = StartPoint;
			FrameLabel.Location = new Point(0,0);
			FrameLabel.Size = Size;
			FrameLabel.Image = (System.Drawing.Image)(MapImage);
			
			FrameLabel.MouseDown += delegate(object sender, MouseEventArgs e) 
			{ 
				MouseActive = true;
				MouseStart = (Size)e.Location;
				Delta = new Size(0,0);
			};
			
			FrameLabel.MouseMove += delegate(object sender, MouseEventArgs e) 
			{ 
				if (MouseActive)
				{
					Delta = (Size)Point.Subtract(e.Location, MouseStart);
					MapPos = Point.Add(MapPos,Delta);
					RedrawImage();
					MapPos = Point.Subtract(MapPos,Delta);
				}
			};
			
			FrameLabel.MouseUp += delegate(object sender, MouseEventArgs e) 
			{
				MapPos = Point.Add(MapPos,Delta);
				RedrawImage();
				MouseActive = false;
			};
		}
		
		unsafe public void RedrawImage () 
		{
			CheckPosition();
			Rectangle SubPicRect = new Rectangle(-MapPos.X,-MapPos.Y,Size.Width,Size.Height);
			BitmapData ImageData = MapImage.LockBits(SubPicRect, ImageLockMode.ReadWrite, MapImage.PixelFormat);
			Bitmap newImage = new Bitmap(Size.Width,Size.Height,ImageData.Stride,MapImage.PixelFormat,ImageData.Scan0);
			BitmapData SubPicData = newImage.LockBits(new Rectangle(0,0,Size.Width,Size.Height), ImageLockMode.ReadOnly, MapImage.PixelFormat);
			
			//newImage.UnlockBits(ImageData);
			//BitmapData SubPicData = ImageData;
			//SubPicData = ImageData;
			
			newImage.UnlockBits(ImageData);
			MapImage.UnlockBits(ImageData);
			
			FrameLabel.Image = (System.Drawing.Image)newImage;
		}
		
		public static void ResetBackground()
		{
			MapImage = new Bitmap(MapImageBase);
		}
		
		static public void AddGraphic(GraphicElement[] gs)
		{
			Array.Sort(gs, GraphicElement.CompareGraphics);
			foreach (GraphicElement g in gs)
				AddGraphic(g);
		}
		
		static public void AddGraphic(GraphicElement g)
		{
			AddGraphic(g.Image, g.Location);
		}
		
		unsafe static public void AddGraphic(Bitmap Graphic, Point Location)
		{
			try
			{
				Rectangle SubPicRect = new Rectangle(Location.X,Location.Y,Graphic.Width,Graphic.Height);
				BitmapData BmD = MapImage.LockBits(SubPicRect, ImageLockMode.ReadWrite, MapImage.PixelFormat);
				BitmapData  AddedBmD = Graphic.LockBits(new Rectangle(0,0,Graphic.Width,Graphic.Height), ImageLockMode.ReadOnly, MapImage.PixelFormat);
				
				byte* Row;
				byte* AddedRow;
				
				int PixelSize = 4;
				int AddedPixelSize = 4;
				
				for(int y=0; y<AddedBmD.Height; y++)
				{
					Row=(byte *)BmD.Scan0+(y*BmD.Stride);
					AddedRow=(byte *)AddedBmD.Scan0+(y*AddedBmD.Stride);
				    for(int x=0; x<AddedBmD.Width; x++)
				    {
				    	if (AddedRow[x*PixelSize+3] > 0)
					   	{
				    		Row[x*PixelSize+0]=System.Convert.ToByte((int)(Row[x*PixelSize+0]*(1-(float)AddedRow[x*AddedPixelSize+3]/(float)255))
				    		                    +(int)(AddedRow[x*AddedPixelSize+0]*((float)AddedRow[x*AddedPixelSize+3]/(float)255)));
				    		Row[x*PixelSize+1]=System.Convert.ToByte((int)(Row[x*PixelSize+1]*(1-(float)AddedRow[x*AddedPixelSize+3]/(float)255))
				    		                    +(int)(AddedRow[x*AddedPixelSize+1]*((float)AddedRow[x*AddedPixelSize+3]/(float)255)));
				    		Row[x*PixelSize+2]=System.Convert.ToByte((int)(Row[x*PixelSize+2]*(1-(float)AddedRow[x*AddedPixelSize+3]/(float)255))
				    		                    +(int)(AddedRow[x*AddedPixelSize+2]*((float)AddedRow[x*AddedPixelSize+3]/(float)255)));
				    		Row[x*PixelSize+3]=AddedRow[x*AddedPixelSize+3];
				    	}
				    }
				}
				MapImage.UnlockBits(BmD);
				Graphic.UnlockBits(AddedBmD);
			}
			catch{}
		}
		
		private void CheckPosition()
		{
			Point nP = MapPos;
			int h = MapImage.Height;
			int w = MapImage.Width;
			if (nP.X>0) nP.X = 0;
			if (nP.Y>0) nP.Y = 0;
			if (nP.X+w<Size.Width) nP.X = Size.Width - w;
			if (nP.Y+h<Size.Height) nP.Y = Size.Height - h;
			MapPos = nP;
		}
		
		public Rectangle GetInnerbounds()
		{
			return new Rectangle(-MapPos.X,-MapPos.Y,Size.Width,Size.Height);
		}
	}
}

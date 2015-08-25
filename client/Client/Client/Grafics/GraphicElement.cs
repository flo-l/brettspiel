/*
 * Created by SharpDevelop.
 * User: lackner
 * Date: 27.07.2014
 * Time: 16:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;

namespace Client
{
	/// <summary>
	/// Description of GraphicElement.
	/// </summary>
	public class GraphicElement
	{
		public Bitmap Image;
		public Point Location;
		
		public GraphicElement(Bitmap img, Point loc)
		{
			Image = img;
			Location = loc;
		}
		public GraphicElement(Image img, Point loc)
		{
			Image = (Bitmap)img;
			Location = loc;
		}
		
		public static int CompareGraphics(GraphicElement g1, GraphicElement g2)
		{
			if (g1.Location.Y > g2.Location.Y)
				return 1;
			if (g1.Location.Y == g2.Location.Y)
				return 0;
			if (g1.Location.Y < g2.Location.Y)
				return -1;
			return 0;
		}
	}
}

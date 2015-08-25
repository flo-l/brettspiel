/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 16.04.2014
 * Time: 13:20
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
	/// Description of RelativeLabel.
	/// </summary>
	public class RelativeLabel:Label
	{
		private Point _RelativeLocation = new Point(0,0);
		public Point RelativeLocation
		{
			get
			{
				return _RelativeLocation;
			}
			set
			{
				_RelativeLocation = value;
			}
		}
		
		public Point Midpoint
		{
			get
			{
				return new Point(Left+Width/2,Top+Height/2);
			}
			set
			{
				Location = new Point(value.X - Size.Width/2, value.Y - Size.Height/2);
			}
		}
		
		public RelativeLabel(Point relPos, Size size)
		{
			RelativeLocation = relPos;
			Size = size;
			BackgroundImageLayout = ImageLayout.Stretch;
		}
	}
}

/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 21.04.2014
 * Time: 20:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Client
{
	/// <summary>
	/// Description of ControlGroup.
	/// </summary>
	public class ControlGroup
	{
		private ControlGroup[] _SubGroups = new ControlGroup[0];
		public ControlGroup[] SubGroups
		{
			get
			{
				return _SubGroups;
			}
			set
			{
				_SubGroups = value;
				Location=Location;
			}
		}
		
		private Control[] _Controls = new Control[0];
		public Control[] Controls
		{
			get
			{
				return _Controls;
			}
			set
			{
				_Controls = value;
				Location=Location;
			}
		}
		
		public Rectangle Bounds
		{
			get
			{
				int x = 0;
				int y = 0;
				
				int xBUF;
				int yBUF;
				
				for (int i=0; i<_Controls.Length; i++)
				{
					Control c = _Controls[i];
					xBUF = c.Location.X + c.Size.Width - _Location.X;
					yBUF = c.Location.Y + c.Size.Height - _Location.Y;
					
					if (xBUF > x) x=xBUF;
					if (yBUF > y) y=yBUF;
				}
				
				return new Rectangle(_Location,new Size(x,y));
			}
		}
		
		private Point _Location = new Point(0,0);
		public Point Location
		{
			get
			{
				return _Location;
			}
			set
			{
				_Location = value;
				Control[] rLabels = Array.FindAll(_Controls, entry => entry.GetType() == (new RelativeLabel(new Point(), new Size())).GetType());
				foreach (RelativeLabel c in rLabels)
				{
					c.Location = Point.Add(c.RelativeLocation, (Size)_Location);
				}
				foreach (ControlGroup g in _SubGroups)
				{
					g.Location=g.Location;
				}
			}
		}
		
		private bool _Visible = false;
		public bool Visible
		{
			get
			{
				return _Visible;
			}
			set
			{
				_Visible = value;
				foreach (Control c in _Controls)
				{
					c.Visible = _Visible;
				}
					
				foreach (ControlGroup c in _SubGroups)
				{
					c.Visible = _Visible;
				}
				
				if (_Visible) BringToFront();
				else SendToBack();
			}
		}
		
		public ControlGroup(Point loc)
		{
			Location = loc;
			Visible = false;
		}
		public ControlGroup()
		{
			Visible = false; 
		}
		
		public void Add(Control c)
		{
			Array.Resize(ref _Controls, _Controls.Length+1);
			_Controls[_Controls.Length-1] = c;
		}
		public void Add(Control c, string name)
		{
			c.Name = name;
			Add(c);
		}
		public void Add(ControlGroup c)
		{
			Array.Resize(ref _SubGroups,_SubGroups.Length+1);
			_SubGroups[_SubGroups.Length-1] = c;
		}
		
		public Control GetControl(string name)
		{
			return Array.Find(_Controls, entry => entry.Name == name);
		}
		
		public int GetIndex(string name)
		{
			return Array.FindIndex(_Controls, entry => entry.Name == name);
		}
		
		public void BringToFront()
		{
			foreach (Control c in _Controls)
			{
				c.BringToFront();
			}
			foreach (ControlGroup c in _SubGroups)
			{
				c.BringToFront();
			}
		}
		
		public void SendToBack()
		{
			foreach (Control c in _Controls)
			{
				c.SendToBack();
			}
			foreach (ControlGroup c in _SubGroups)
			{
				c.SendToBack();
			}
		}
		
		public void Swap(int i1, int i2)
		{
			if (i1 < _Controls.Length && i2 < _Controls.Length && i1 >= 0 && i2 >= 0)
			{
				Control BUF = _Controls[i1];
				_Controls[i1] = _Controls[i2];
				_Controls[i2] = BUF;
				
				BringToFront();
			}
		}
		
		public void Swap(string n1, string n2)
		{
			int i1 = GetIndex(n1);
			int i2 = GetIndex(n2);
			
			if (!int.Equals(i1,null) && !int.Equals(i2,null))
			{
				Swap(i1, i2);
			}
		}
		
		public void SetControlPos (int oldIndex, int newIndex)
		{
			if (oldIndex >= 0 && oldIndex < _Controls.Length && newIndex >=0 && newIndex < _Controls.Length)
			{
				Control BUF = _Controls[oldIndex];
				int length = Math.Abs(oldIndex-newIndex);
				if (oldIndex < newIndex)
					Array.Copy(_Controls,oldIndex+1,_Controls,oldIndex,length);
				if (oldIndex > newIndex)
					Array.Copy(_Controls,newIndex+1,_Controls,newIndex,length);
				Controls[newIndex]=BUF;
			}
		}
		
		public void SetControlPos (string name, int newIndex)
		{
			int id = Array.FindIndex(_Controls, entry => entry.Name == name);
			if (!int.Equals(id,null))
				SetControlPos(id, newIndex);
		}
		
		private bool _Disposable = true;
		public void Dispose()
		{
			if (_Disposable)
			{
				_Disposable = false;
				foreach (ControlGroup c in SubGroups)
				{
					c.Dispose();
				}
				foreach (Control c in Controls)
				{
					c.Dispose();
				}
			}
		}
		
	}
}

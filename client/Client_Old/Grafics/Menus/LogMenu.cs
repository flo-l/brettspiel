/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 15.07.2014
 * Time: 22:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;


namespace Client
{
	/// <summary>
	/// Description of LogMenu.
	/// </summary>
	
	public class LogMenu:ControlGroup
	{
		public static List<string> LogEntries = new List<string>();
		
		public static void AddLogEntry(string newLogEntry)
		{
			LogEntries.Add(newLogEntry);
		}
		
		public static void ResetLog()
		{
			Lines = new string[0];
			LogEntries = new List<string>();
		}
		
		public List<string> GetBufferContent()
		{
			List<string> LinesBuf = new List<string>();
			
			string stringBuf = "";
			
			int currentPos = 0;
			
			int yCurrent = 0;
			int yLine = BufferBox.GetPositionFromCharIndex(0).Y;
			
			for (int i = 0; i < BufferBox.Lines.Length; i++)
			{
				for (int j = 0; j < BufferBox.Lines[i].Length; j++)
				{
					yCurrent = BufferBox.GetPositionFromCharIndex(currentPos).Y;
					if (yCurrent == yLine)
					{
						stringBuf += BufferBox.Text[currentPos];
					}
					else
					{
						LinesBuf.Add(stringBuf.Replace("\r\n",""));
						yLine = yCurrent;
						stringBuf = BufferBox.Text[currentPos].ToString();
					}
					currentPos++;
				}
				LinesBuf.Add(stringBuf.Replace("\r\n",""));
				yLine = yCurrent+LineHeight;
				stringBuf = "";
				currentPos += 2;
			}
			return LinesBuf;
		}
		
		public void AddLastLogEntry()
		{
			if (LogEntries.Count > 0)
				BufferBox.Text = LogEntries[LogEntries.Count-1];
			
			List<string> LinesBuf = GetBufferContent();
			
			Array.Resize(ref Lines, Lines.Length + LinesBuf.Count);
			for (int i = Lines.Length - LinesBuf.Count; i < Lines.Length; i++)
			{
				Lines[i] = LinesBuf[i - (Lines.Length - LinesBuf.Count)];
			}
		}
		
		public void GetAllLogEntries()
		{
//			BufferBox.Text += LogEntries[LogEntries.Count-1];
			string FullText = "";
			foreach (string strng in LogEntries)
				FullText += strng+"\r\n";
			BufferBox.Text = FullText;
			
			List<string> LinesBuf = GetBufferContent();
			
			Lines = LinesBuf.ToArray();
		}
		
		public void UpdateLabel()
		{
			AddLastLogEntry();
			ScrollPos = Lines.Length - MaxLines;
		}
		
		public void DisplayVisible()
		{
			string ShownText = "";
			for (int i = ScrollPos; i < ScrollPos + MaxLines; i++)
			{
				if (i < Lines.Length)
					ShownText += Lines[i].Replace("\r\n","").Trim() + "\r\n";
			}
			GetControl("TextLabel").Text = ShownText;
		}
		
		private TextBox BufferBox = new TextBox();
		
		Size ScrollLabelSize = new Size(40,20);
		
		private static int Width = 300;
		private static int Height = MainForm.ClientResolution.Height/2;
		private static int Offset = 10;
		
		private static int SafetyOffset = 2; // Used because of the padding differences between the bufferbox and the displaylabel
		
		private static string[] Lines = new string[0];
		
		public int _ScrollPos = 0;
		public int ScrollPos
		{
			get 
			{
				return _ScrollPos;
			}
			set
			{
				_ScrollPos = value;
				if (value > Lines.Length - MaxLines)
					_ScrollPos = Lines.Length - MaxLines;
				if (_ScrollPos < 0)
					_ScrollPos = 0;
				
				double quot = (Lines.Length - MaxLines);
				if (quot <= 0)
					quot = 1;
				GetControl("ScrollLabel").Top = Location.Y + Offset + (int)((double)(BufferBox.Height * ScrollPos)/quot)-ScrollLabelSize.Height/2;
				
				DisplayVisible();
			}
		}
		
		private int MaxLines
		{
			get
			{
				return BufferBox.Height / LineHeight;
			}
		}
		
		private int LineCount
		{
			get
			{
				int Count = BufferBox.Lines.Length;
				
				int currentPos = 0;
				int yFirst;
				int yLast;
				
				for (int i = 0; i < BufferBox.Lines.Length; i++)
				{
					yFirst = BufferBox.GetPositionFromCharIndex(currentPos).Y;
					currentPos += BufferBox.Lines[i].Length;
					yLast = BufferBox.GetPositionFromCharIndex(currentPos).Y;
					currentPos += 2;
					
					Count += (yLast - yFirst)/LineHeight;
				}
				return Count;
			}
		}
		
		private int LineHeight
		{
			get
			{
				return BufferBox.Font.Height;
			}
		}
		
		public LogMenu() //FIXME: Bug causing empty Lines
		{
			Location = new System.Drawing.Point(MainForm.ClientResolution.Width-Width,0);
			
			Add(new RelativeLabel(new System.Drawing.Point(0,0),new System.Drawing.Size(Width,Height)),"BackgroundLabel");
			GetControl("BackgroundLabel").BackColor = Color.Lime;
			
			BufferBox.Multiline  = true;
			BufferBox.ReadOnly = true;
			BufferBox.Cursor = Cursors.Default;
//			BufferBox.BackColor = SystemColors.Control;
//			BufferBox.BorderStyle = BorderStyle.None;
			BufferBox.Location = Point.Add(Location, new Size(Offset,Offset));
			BufferBox.Size = new Size(Width-2*Offset-SafetyOffset,Height-2*Offset);
			Add(BufferBox);
			
			Add(new RelativeLabel(new Point(2*Offset-ScrollLabelSize.Width,Offset-ScrollLabelSize.Height/2), ScrollLabelSize),"ScrollLabel");
			GetControl("ScrollLabel").BackColor = Color.Yellow;
			
			#region RelativeLable version
			RelativeLabel c = new RelativeLabel (new Point(Offset,Offset), new Size(Width-2*Offset,Height-2*Offset));
			c.BorderStyle = BorderStyle.None;
			c.BackColor = Color.Transparent;
			c.Parent = BufferBox;
			Add(c, "TextLabel");
			
			c.MouseEnter += delegate(object sender, EventArgs e) { c.Focus(); };
			c.MouseWheel += delegate(object sender, MouseEventArgs e)
			{ 
				ScrollPos -= 5*e.Delta/120;
			};
			#endregion
			
			#region Scroll-control
//			GetControl("ScrollLabel").MouseDown += delegate(object sender, MouseEventArgs e) 
//			{ 
//				ScrollDrag = true;
//			};
//			GetControl("ScrollLabel").MouseUp += delegate(object sender, MouseEventArgs e) 
//			{ 
//				ScrollDrag = false;
//			};
//			GetControl("ScrollLabel").MouseMove += delegate(object sender, MouseEventArgs e) 
//			{ 
//				if (ScrollDrag && (Location.Y + Offset - SLS.Height/2 < e.Y) && (e.Y < Location.Y + Offset + Height - SLS.Height/2))
//				{
//					GetControl("ScrollLabel").Top = e.Y;
//				}
//			};
			#endregion
			
			BringToFront();
		}
	}
}

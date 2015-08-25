/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 12.07.2014
 * Time: 11:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using WebSocket4Net;

namespace Client
{
	/// <summary>
	/// Description of StartForm.
	/// </summary>
	public class WaitForConnectionForm : Form
	{
		Button AbortButton = new Button();
		Timer WaitTimer = new Timer();
		
		public WaitForConnectionForm(Connector c)
		{
			this.StartPosition = FormStartPosition.Manual;
			this.Location = MainForm.StartPos;
			
			Controls.AddRange(new Control[1] {AbortButton});
			
			AbortButton.Text = "Abort";
			
			AbortButton.Location = new Point(10, 10);
			AbortButton.Width = 250;
			AbortButton.Click += delegate 
			{
				Close();
			};
			
			WaitTimer.Interval = 10;
			WaitTimer.Enabled = true;
			
			WaitTimer.Tick += delegate 
			{ 
				if (c.Socket.State == WebSocketState.Open)
				{
					this.Close(); 
				}
			};
			
			Size = new Size(290,80);
		}
	}
}

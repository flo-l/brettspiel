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

namespace Client
{
	/// <summary>
	/// Description of StartForm.
	/// </summary>
	public class ConnectionForm : Form
	{
		Label HostLabel = new Label();
		TextBox HostBox = new TextBox();
		Button ConfirmButton = new Button();
		
		
		private string _Host = "ws://127.0.0.1:2012";
		public string Host
		{
			get
			{
				return _Host;
			}
		}
		
		public ConnectionForm()
		{
			this.StartPosition = FormStartPosition.Manual;
			this.Location = MainForm.StartPos;
			
			Controls.AddRange(new Control[3] {HostLabel,HostBox,ConfirmButton});
			
			HostLabel.Text = "Host: ";
			ConfirmButton.Text = "Confirm";
			
			HostLabel.Location = new Point(10, 10);
			HostLabel.Size = new Size(50,20);
			HostBox.Location = new Point(60, 10);
			HostBox.Size = new Size(200,20);
			HostBox.Text = "ws://127.0.0.1:2012";
			HostBox.Text = "ws://10.0.0.4:2012";
			HostBox.Text = "ws://192.168.0.18:2012";
				
			
			ConfirmButton.Location = new Point(10, 35);
			ConfirmButton.Width = 250;
			ConfirmButton.Click += delegate 
			{
				_Host = HostBox.Text;
				Close();
			};
			
			Size = new Size(290,105);
		}
	}
}

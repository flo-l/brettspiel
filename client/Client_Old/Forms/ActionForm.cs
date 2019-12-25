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
	public class ActionForm : Form
	{
		Button JoinButton = new Button();
		Button CreateNewButton = new Button();
		
		public string Action = "";
		public string GameID;
		
		public ActionForm()
		{
			this.StartPosition = FormStartPosition.Manual;
			this.Location = MainForm.StartPos;
			
			Controls.AddRange(new Control[2] {JoinButton,CreateNewButton});
			
			JoinButton.Text = "Join";
			CreateNewButton.Text = "Create new";
			
			JoinButton.Location = new Point(10, 10);
			JoinButton.Width = 120;
			JoinButton.Click += delegate 
			{
				GameIDForm SubInitForm = new GameIDForm();
				SubInitForm.ShowDialog();
				GameID = SubInitForm.GameID;
				
				Action = "join";
				
				Close();
			};
			
			CreateNewButton.Location = new Point(140,10);
			CreateNewButton.Width = 120;
			CreateNewButton.Click += delegate 
			{
				Action = "create_new";
				
				Close();
			};
			
			Size = new Size(290,75);
		}
	}
}

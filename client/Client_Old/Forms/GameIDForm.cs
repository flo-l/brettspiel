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
	public class GameIDForm : Form
	{
		Label GameIDLabel = new Label();
		TextBox GameIDBox = new TextBox();
		Button ConfirmButton = new Button();
		
		public string GameID = "";
		
		public GameIDForm()
		{
			this.StartPosition = FormStartPosition.Manual;
			this.Location = MainForm.StartPos;
			
			Controls.AddRange(new Control[3] {GameIDLabel,GameIDBox,ConfirmButton});
			
			GameIDLabel.Text = "GameID: ";
			ConfirmButton.Text = "Confirm";
			
			GameIDLabel.Location = new Point(10, 10);
			GameIDLabel.Size = new Size(50,20);
			GameIDBox.Location = new Point(60, 10);
			GameIDBox.Size = new Size(200,20);
			GameIDBox.Text = "0";
			
			ConfirmButton.Location = new Point(10, 35);
			ConfirmButton.Width = 250;
			ConfirmButton.Click += delegate 
			{
				GameID = GameIDBox.Text;
				Close();
			};
			
			Size = new Size(290,105);
		}
	}
}

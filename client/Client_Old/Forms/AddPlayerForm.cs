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
	public class AddPlayerForm : Form
	{
		Label PlayerLabel = new Label();
		TextBox PlayerBox = new TextBox();
		Button ConfirmButton = new Button();
		
		public string Playername = "";
		
		public AddPlayerForm()
		{
			this.StartPosition = FormStartPosition.Manual;
			this.Location = MainForm.StartPos;
			
			Controls.AddRange(new Control[3] {PlayerLabel,PlayerBox,ConfirmButton});
			
			PlayerLabel.Text = "Playername: ";
			ConfirmButton.Text = "Confirm";
			
			PlayerLabel.Location = new Point(10, 10);
			PlayerLabel.Size = new Size(50,20);
			PlayerBox.Location = new Point(60, 10);
			PlayerBox.Size = new Size(200,20);
			PlayerBox.Text = "XOR";
			
			ConfirmButton.Location = new Point(10, 35);
			ConfirmButton.Width = 250;
			ConfirmButton.Click += delegate 
			{
				Playername = PlayerBox.Text;
				Close();
			};
			
			Size = new Size(290,105);
		}
	}
}

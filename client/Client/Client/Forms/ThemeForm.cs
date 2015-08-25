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
using SimpleJson;

namespace Client
{
	/// <summary>
	/// Description of StartForm.
	/// </summary>
	public class ThemeForm : Form
	{
		Label ThemeLabel = new Label();
		ComboBox ThemeBox = new ComboBox();
		Button ConfirmButton = new Button();
		
		public string Theme = "Default";
		
		public ThemeForm(JsonArray themes)
		{
			this.StartPosition = FormStartPosition.Manual;
			this.Location = MainForm.StartPos;
			
			Controls.AddRange(new Control[3] {ThemeLabel,ThemeBox,ConfirmButton});
			
			ThemeLabel.Text = "Theme: ";
			for (int i = 0; i < themes.Count; i++)
			{
				ThemeBox.Items.Add(themes[i].ToString());
			}
			ThemeBox.SelectedItem = ThemeBox.Items[0];
			ConfirmButton.Text = "Confirm";
			
			ThemeLabel.Location = new Point(10, 10);
			ThemeLabel.Size = new Size(50,20);
			ThemeBox.Location = new Point(60, 10);
			ThemeBox.Size = new Size(200,20);
			ThemeBox.Text = "0";
			
			ConfirmButton.Location = new Point(10, 35);
			ConfirmButton.Width = 250;
			ConfirmButton.Click += delegate 
			{
				Theme = ThemeBox.SelectedItem.ToString();
				Close();
			};
			
			Size = new Size(290,105);
		}
	}
}

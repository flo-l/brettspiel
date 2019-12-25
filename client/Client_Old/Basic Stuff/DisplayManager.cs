/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 25.07.2014
 * Time: 11:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Client
{
	public class DisplayManager:Form
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct DISPLAY_DEVICE
		{
			public int cb;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string DeviceName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceString;
			public int StateFlags;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceID;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=128)]
			public string DeviceKey;
 
			public DISPLAY_DEVICE(int flags)
			{
				cb = 0;
				StateFlags = flags;
				DeviceName = new string((char)32, 32);
				DeviceString = new string((char)32, 128);
				DeviceID = new string((char)32, 128);
				DeviceKey = new string((char)32, 128);
				cb = Marshal.SizeOf(this);
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DEVMODE
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string dmDeviceName;
			public short dmSpecVersion;
			public short dmDriverVersion;
			public short dmSize;
			public short dmDriverExtra;
			public int dmFields;
			public short dmOrientation;
			public short dmPaperSize;
			public short dmPaperLength;
			public short dmPaperWidth;
			public short dmScale;
			public short dmCopies;
			public short dmDefaultSource;
			public short dmPrintQuality;
			public short dmColor;
			public short dmDuplex;
			public short dmYResolution;
			public short dmTTOption;
			public short dmCollate;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string dmFormName;
			public short dmUnusedPadding;
			public short dmBitsPerPel;
			public int dmPelsWidth;
			public int dmPelsHeight;
			public int dmDisplayFlags;
			public int dmDisplayFrequency;
		}
	 
		internal static string GetDeviceName(int devNum)
		{
			DISPLAY_DEVICE d = new DISPLAY_DEVICE(0);
			bool result = EnumDisplayDevices(IntPtr.Zero, 
				devNum, ref d, 0);
			return (result ? d.DeviceName.Trim() : "#error#");
		}
	
		internal static bool MainDevice(int devNum)
		{ //whether the specified device is the main device
			DISPLAY_DEVICE d = new DISPLAY_DEVICE(0);
			if (EnumDisplayDevices(IntPtr.Zero, devNum, ref d, 0))
			{
				return ((d.StateFlags & 4) != 0);
			} 
			return false;
		}
	
		[DllImport("User32.dll")]
		private static extern bool EnumDisplayDevices(
			IntPtr lpDevice, int iDevNum,
			ref DISPLAY_DEVICE lpDisplayDevice, int dwFlags);
	
		[DllImport("User32.dll")]
		private static extern bool EnumDisplaySettings(
			string devName, int modeNum, ref DEVMODE devMode);
	
		[DllImport("user32.dll")]
		public static extern int ChangeDisplaySettings(
			ref DEVMODE devMode, int flags);
		
		private static DEVMODE CurrentDEVMODE
		{
			get
			{
				DEVMODE mode = new DEVMODE();
				EnumDisplaySettings(null, -1, ref mode);
				return mode;
			}
		}
		public static Size Resolution
		{
			get
			{
				DEVMODE devMode = CurrentDEVMODE;
				return new Size(devMode.dmPelsWidth,devMode.dmPelsHeight);
			}
		}
		
		private static DEVMODE OriginalDEVMODE = new DEVMODE();
		private static bool OriginalDEVMODE_Set = false;
		
		public static bool ResolutionChanged
		{
			get
			{
				return OriginalDEVMODE_Set;
			}
		}
		
		public static void GetOriginalDEVMODE()
		{
			if (!OriginalDEVMODE_Set)
			{
				OriginalDEVMODE_Set = EnumDisplaySettings(null, -1, ref OriginalDEVMODE);
			}
		}
		
		public static void SetOriginalDEVMODE()
		{
			if(OriginalDEVMODE_Set)
			{
				ChangeDisplaySettings(ref OriginalDEVMODE, 0);
			}
		}
		
		ListBox listDevices = new ListBox();
		ListBox listSettings = new ListBox();
		
		Label SettingsLabel = new Label();
		Label DeviceLabel = new Label();
		
		Label InfoLabel = new Label();
		
		Button SetButton = new Button();
		Button CloseButton = new Button();
		
	 	public DisplayManager()
		{
			this.StartPosition = FormStartPosition.Manual;
			this.Location = MainForm.StartPos;
			Size = new Size(290,510);
			
			Controls.AddRange(new Control[] {listDevices,listSettings,SettingsLabel,DeviceLabel,InfoLabel,SetButton,CloseButton});
			
			InfoLabel.Location = new Point(10,10);
			InfoLabel.Size = new Size(250,40);
			InfoLabel.Text = "This game is optimised for a resolution of xxxx * yyyy. You can change the resolution now or continue without adjustments.";
			
			DeviceLabel.Location = new Point(10,60);
			DeviceLabel.Size = new Size(250,20);
			DeviceLabel.Text = "Devices: ";
			
			listDevices.Location = new Point(10,90);
			listDevices.Size = new Size(250,150);
			
			SettingsLabel.Location = new Point(10,250);
			SettingsLabel.Size = new Size(250,20);
			SettingsLabel.Text = "Settings: ";
			
			listSettings.Location = new Point(10,280);
			listSettings.Size = new Size(250,150);
			
			SetButton.Location = new Point(10,440);
			SetButton.Width = 120;
			SetButton.Text = "Set";
			
			CloseButton.Location = new Point(140,440);
			CloseButton.Width = 120;
			CloseButton.Text = "Continue";
			
			listDevices.SelectedIndexChanged += listDevices_SelectedIndexChanged;
			SetButton.Click += SetButton_Click;
			CloseButton.Click += CloseButton_Click;
			
			EnumDevices();
		}
		
		private void listDevices_SelectedIndexChanged(object sender, EventArgs e)
		{
			int devNum = listDevices.SelectedIndex;
			bool isMain = MainDevice(devNum);
			SetButton.Enabled = isMain; // enable only for the main device
			EnumModes(devNum);
		}
		
		private void CloseButton_Click(object sender, EventArgs e)
		{
			Close();
		}
	 	
		private void SetButton_Click(object sender, EventArgs e)
		{ //set selected display mode
			GetOriginalDEVMODE();
			int devNum = listDevices.SelectedIndex;
			int modeNum = listSettings.SelectedIndex;
			DEVMODE d = GetDevmode(devNum, modeNum);
			if (d.dmBitsPerPel != 0 & d.dmPelsWidth != 0 
				& d.dmPelsHeight != 0)
			{
        d.dmPelsHeight = 1200;
        d.dmPelsWidth = 1920;
				ChangeDisplaySettings(ref d, 0);
			}
			Close();
		}
	 	
		private void EnumModes(int devNum)
		{
			listSettings.Items.Clear();
			string devName = GetDeviceName(devNum);
			DEVMODE devMode = new DEVMODE();
			int modeNum = 0;
			bool result = true;
			do
			{
				result = EnumDisplaySettings(devName, 
					modeNum, ref devMode);

				if (result)
				{
					string item = DevmodeToString(devMode);
					listSettings.Items.Add(item);
				}
				modeNum++;
			} while (result);
 
			if (listSettings.Items.Count > 0)
			{
				DEVMODE current = GetDevmode(devNum, -1);
				int selected = listSettings.FindString(
					DevmodeToString(current));
				if (selected >= 0)
				listSettings.SetSelected(selected, true);
			}
		}
 
		private DEVMODE GetDevmode(int devNum, int modeNum)
		{ //populates DEVMODE for the specified device and mode
			DEVMODE devMode = new DEVMODE();
			string devName = GetDeviceName(devNum);
			EnumDisplaySettings(devName, modeNum, ref devMode);
			return devMode;
		}
 
		private string DevmodeToString(DEVMODE devMode)
		{
			return devMode.dmPelsWidth.ToString() +
				" x " + devMode.dmPelsHeight.ToString() +
				", " + devMode.dmBitsPerPel.ToString() + 
				" bits, " + 
				devMode.dmDisplayFrequency.ToString() + " Hz";
		}
 
		private void EnumDevices()
		{ //populates Display Devices list
			this.listDevices.Items.Clear();
			DISPLAY_DEVICE d = new DISPLAY_DEVICE(0);
 
			int devNum = 0;
			bool result;
			do
			{
				result = EnumDisplayDevices(IntPtr.Zero, 
					devNum, ref d, 0);
 
				if (result)
				{
					string item = devNum.ToString() + 
					". " + d.DeviceString.Trim();
					if ((d.StateFlags & 4) != 0) item += " - main";
					this.listDevices.Items.Add(item);
				}
				devNum++;
			} while (result);
		}
	}
}

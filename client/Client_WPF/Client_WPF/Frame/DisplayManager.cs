using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Collections.Generic;

namespace Client
{
  public class DisplayManager
  {
    #region deep shit
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
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
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
    #endregion

    public static List<DEVMODE> Settings;
    public static List<DISPLAY_DEVICE> Devices;

    public static DEVMODE CurrentDEVMODE
    {
      get
      {
        DEVMODE mode = new DEVMODE();
        EnumDisplaySettings(null, -1, ref mode);
        return mode;
      }
      set
      {
        ChangeDisplaySettings(ref OriginalDEVMODE, 0);
      }
    }
    public static Size Resolution
    {
      get
      {
        DEVMODE devMode = CurrentDEVMODE;
        return new Size(devMode.dmPelsWidth, devMode.dmPelsHeight);
      }
    }

    private static DEVMODE OriginalDEVMODE = CurrentDEVMODE;

    public static void RestoreOriginalDEVMODE()
    {
      CurrentDEVMODE = OriginalDEVMODE;
    }

    public DisplayManager()
    {
      EnumDevices();
    }

    private static void EnumModes(int devNum)
    {
      Settings = new List<DEVMODE>();
      string devName = GetDeviceName(devNum);
      DEVMODE devMode = new DEVMODE();

      int modeNum = 0;
      while (EnumDisplaySettings(devName, modeNum, ref devMode))
      {
        Settings.Add(devMode);
        modeNum++;
      }
    }

    private static DEVMODE GetDevmode(int devNum, int modeNum)
    { //populates DEVMODE for the specified device and mode
      DEVMODE devMode = new DEVMODE();
      string devName = GetDeviceName(devNum);
      EnumDisplaySettings(devName, modeNum, ref devMode);
      return devMode;
    }

    public static string DevmodeToString(DEVMODE devMode)
    {
      return devMode.dmPelsWidth.ToString() +
        " x " + devMode.dmPelsHeight.ToString() +
        ", " + devMode.dmBitsPerPel.ToString() +
        " bits, " +
        devMode.dmDisplayFrequency.ToString() + " Hz";
    }
    public static string DeviceToString(DISPLAY_DEVICE device)
    {
      string item = device.DeviceString.Trim();
      if ((device.StateFlags & 4) != 0) item += " - main";
      return item;
    }

    private static void EnumDevices()
    { //populates Display Devices list
      Devices = new List<DISPLAY_DEVICE>();
      DISPLAY_DEVICE d = new DISPLAY_DEVICE(0);

      int devNum = 0;
      bool result;
      while (result = EnumDisplayDevices(IntPtr.Zero, devNum, ref d, 0))
      {
        Devices.Add(d);
        devNum++;
      }
    }
  }
}

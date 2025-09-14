using System;
using System.Management;
using System.Runtime.InteropServices;

namespace YAMDCC.Common
{
    public static class DisplayUtils
    {
        // P/Invoke for EnumDisplaySettings and ChangeDisplaySettings
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);

        public static void SetRefreshRate(int hz)
        {
            DEVMODE dm = new DEVMODE();
            dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
            if (EnumDisplaySettings(null, -1, ref dm) != 0)
            {
                dm.dmDisplayFrequency = hz;
                dm.dmFields = 0x400000; // DM_DISPLAYFREQUENCY
                ChangeDisplaySettings(ref dm, 0);
            }
        }

        public static int GetCurrentRefreshRate()
        {
            DEVMODE dm = new DEVMODE();
            dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
            if (EnumDisplaySettings(null, -1, ref dm) != 0)
            {
                return dm.dmDisplayFrequency;
            }
            return -1;
        }

        public static bool IsOnBattery()
        {
            System.Windows.Forms.PowerStatus ps = System.Windows.Forms.SystemInformation.PowerStatus;
            return ps.PowerLineStatus == System.Windows.Forms.PowerLineStatus.Offline;
        }
    }
}

using System;
using System.Runtime.InteropServices;

static class MonitorPosition {
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DEVMODE {
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
        public short dmBitsPerPel;
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
    };

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hWnd, int dwFlags, IntPtr lParam);


    public const int CDS_UPDATEREGISTRY = 0x01;

    const int DM_POSITION = 0x20;

    public static void PrintMonitors() {
        for (uint i = 0; true; i++) {
            var device = new User32Wrapper.DISPLAY_DEVICE();
            device.cb = Marshal.SizeOf(device);

            bool success = User32Wrapper.EnumDisplayDevices(null, i, ref device, 0);
            if (!success) break;

            Console.WriteLine(
                string.Format(
                    "Monitor {0}:\n\tName - {1}\n\tString - {2}\n\tFlags - {3}\n\tID - {4}\n\tKey - {5}",
                    i,
                    device.DeviceName,
                    device.DeviceString,
                    device.StateFlags,
                    device.DeviceID,
                    device.DeviceKey
                )
            );
        }
    }

    public static void SetMonitorPosition(uint displayIndex, int x, int y) {
        string deviceName = "\\\\.\\DISPLAY" + (displayIndex + 1);

        DEVMODE newMode = new();

        if (!EnumDisplaySettings(deviceName, -1, ref newMode)) {
            throw new Exception($"Failed to query display {deviceName}!");
        }

        newMode.dmFields = DM_POSITION;
        newMode.dmPositionY = y;
        newMode.dmPositionX = x;

        var result = ChangeDisplaySettingsEx(deviceName, ref newMode, IntPtr.Zero, CDS_UPDATEREGISTRY, IntPtr.Zero);
        if (result != 0) {
            throw new Exception($"Failed to change display {deviceName}!");
        }
    }
}

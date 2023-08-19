using System;
using System.Runtime.InteropServices;

static class MonitorPosition {
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

        var newMode = new User32Wrapper.DEVMODE();
        newMode.dmSize = (short) Marshal.SizeOf(typeof(User32Wrapper.DEVMODE));

        User32Wrapper.EnumDisplaySettings(deviceName, 0, ref newMode);

        newMode.dmFields |= (User32Wrapper.DM) 0x20L;
        newMode.dmPosition.y = y;
        newMode.dmPosition.x = x;

        User32Wrapper.ChangeDisplaySettingsEx(deviceName, ref newMode, IntPtr.Zero, User32Wrapper.ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY, IntPtr.Zero);
    }
}

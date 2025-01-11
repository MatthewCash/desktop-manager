using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

class RecalibrateWheel : Keybinds.IKeybindHandler {
    public void KeyDown(object sender, KeyEventArgs e) {
        if (!Keybinds.TrackedKeyStates[Keys.CapsLock]) return;

        if (e.KeyCode != Keys.F1) return;
        e.Handled = true;

        ThreadStart work = Recalibrate;
        Thread thread = new Thread(work);
        thread.Start();
    }

    static void Recalibrate() {
        string wheelDevId = Config.GetConfig().WheelDevId;

        try {
            ChangeDeviceState(wheelDevId, false);
            ChangeDeviceState(wheelDevId, true);
        } catch {
            Console.WriteLine("Calibration failed, trying again...");
            ChangeDeviceState(wheelDevId, false);
            ChangeDeviceState(wheelDevId, true);
        }
    }

    static void ChangeDeviceState(string instanceId, bool enable) {
        Guid classGuid = Guid.Empty;
        IntPtr deviceInfoSet = SetupDiGetClassDevs(ref classGuid, null, IntPtr.Zero, DIGCF_ALLCLASSES | DIGCF_PRESENT);
        if (deviceInfoSet == IntPtr.Zero)
            throw new Exception("Failed to get device info set.");

        try {
            SP_DEVINFO_DATA deviceInfoData = new SP_DEVINFO_DATA();
            deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);

            for (int i = 0; SetupDiEnumDeviceInfo(deviceInfoSet, i, ref deviceInfoData); i++) {
                string deviceId = GetDeviceInstanceId(deviceInfoSet, ref deviceInfoData);
                if (deviceId == instanceId) {
                    ChangeState(deviceInfoSet, ref deviceInfoData, enable);
                    break;
                }
            }
        } finally {
            SetupDiDestroyDeviceInfoList(deviceInfoSet);
        }
    }

    static string GetDeviceInstanceId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData) {
        uint requiredSize = 0;
        SetupDiGetDeviceInstanceId(deviceInfoSet, ref deviceInfoData, null, 0, ref requiredSize);

        StringBuilder buffer = new((int) requiredSize);

        if (SetupDiGetDeviceInstanceId(deviceInfoSet, ref deviceInfoData, buffer, (uint) buffer.Capacity, ref requiredSize)) {
            return buffer.ToString();
        }

        throw new Exception("Failed to get device instance ID.");
    }

    static void ChangeState(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, bool enable) {
        SP_PROPCHANGE_PARAMS propChangeParams = new SP_PROPCHANGE_PARAMS {
            ClassInstallHeader = new SP_CLASSINSTALL_HEADER {
                cbSize = Marshal.SizeOf(typeof(SP_CLASSINSTALL_HEADER)),
                InstallFunction = DIF_PROPERTYCHANGE
            },
            StateChange = enable ? DICS_ENABLE : DICS_DISABLE,
            Scope = DICS_FLAG_GLOBAL,
            HwProfile = 0
        };

        IntPtr propChangeParamsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(propChangeParams));
        Marshal.StructureToPtr(propChangeParams, propChangeParamsPtr, true);

        try {
            if (!SetupDiSetClassInstallParams(deviceInfoSet, ref deviceInfoData, ref propChangeParams, Marshal.SizeOf(propChangeParams))) {
                throw new Exception("Failed to set class install params.");
            }

            if (!SetupDiCallClassInstaller(DIF_PROPERTYCHANGE, deviceInfoSet, ref deviceInfoData)) {
                throw new Exception($"Failed to {(enable ? "enable" : "disable")} device.");
            }
        } finally {
            Marshal.FreeHGlobal(propChangeParamsPtr);
        }
    }

    private const uint DIGCF_PRESENT = 0x02;
    private const uint DIGCF_ALLCLASSES = 0x04;
    private const uint DICS_ENABLE = 1;
    private const uint DICS_DISABLE = 2;
    private const uint DICS_FLAG_GLOBAL = 1;
    private const uint DIF_PROPERTYCHANGE = 0x12;

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, string enumerator, IntPtr hwndParent, uint flags);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiEnumDeviceInfo(IntPtr deviceInfoSet, int memberIndex, ref SP_DEVINFO_DATA deviceInfoData);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder deviceInstanceId, uint deviceInstanceIdSize, ref uint requiredSize);

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool SetupDiSetClassInstallParams(
      IntPtr DeviceInfoSet,
      ref SP_DEVINFO_DATA DeviceInfoData,
      ref SP_PROPCHANGE_PARAMS ClassInstallParams,
      int ClassInstallParamsSize);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiCallClassInstaller(uint installFunction, IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData);

    [StructLayout(LayoutKind.Sequential)]
    private struct SP_DEVINFO_DATA {
        public int cbSize;
        public Guid ClassGuid;
        public uint DevInst;
        public IntPtr Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SP_CLASSINSTALL_HEADER {
        public int cbSize;
        public uint InstallFunction;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SP_PROPCHANGE_PARAMS {
        public SP_CLASSINSTALL_HEADER ClassInstallHeader;
        public uint StateChange;
        public uint Scope;
        public uint HwProfile;
    }
}

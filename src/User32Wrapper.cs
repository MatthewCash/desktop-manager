using System;
using System.Runtime.InteropServices;

public static class User32Wrapper {
    [DllImport("user32.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    public static extern bool GetMessage(ref MSG message, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    public static extern bool TranslateMessage(ref MSG message);

    [DllImport("user32.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    public static extern long DispatchMessage(ref MSG message);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT {
        long x;
        long y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG {
        IntPtr hwnd;
        public uint message;
        UIntPtr wParam;
        IntPtr lParam;
        uint time;
        POINT pt;
    }

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr hWndChildAfter, string className, string windowTitle);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    public enum DISP_CHANGE : int {
        Successful = 0,
        Restart = 1,
        Failed = -1,
        BadMode = -2,
        NotUpdated = -3,
        BadFlags = -4,
        BadParam = -5,
        BadDualView = -6
    }

    public struct POINTL {
        public Int32 x;
        public Int32 y;
    }

    public enum DM : short {
        DMDUP_UNKNOWN = 0,
        DMDUP_SIMPLEX = 1,
        DMDUP_VERTICAL = 2,
        DMDUP_HORIZONTAL = 3,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DEVMODE {
        public const int CCHDEVICENAME = 32;
        public const int CCHFORMNAME = 32;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string dmDeviceName;
        public Int16 dmSpecVersion;
        public Int16 dmDriverVersion;
        public Int16 dmSize;
        public Int16 dmDriverExtra;
        public DM dmFields;

        public POINTL dmPosition;
        public Int32 dmDisplayOrientation;
        public Int32 dmDisplayFixedOutput;

        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
        public string dmFormName;
        public Int16 dmLogPixels;
        public Int32 dmBitsPerPel;
        public Int32 dmPelsWidth;
        public Int32 dmPelsHeight;
        public Int32 dmDisplayFlags;
        public Int32 dmDisplayFrequency;

        public Int32 dmICMMethod;
        public Int32 dmICMIntent;
        public Int32 dmMediaType;
        public Int32 dmDitherType;
        public Int32 dmReserved1;
        public Int32 dmReserved2;
        public Int32 dmPanningWidth;
        public Int32 dmPanningHeight;
    }

    [Flags()]
    public enum ChangeDisplaySettingsFlags : uint {
        CDS_NONE = 0,
        CDS_UPDATEREGISTRY = 0x00000001,
        CDS_TEST = 0x00000002,
        CDS_FULLSCREEN = 0x00000004,
        CDS_GLOBAL = 0x00000008,
        CDS_SET_PRIMARY = 0x00000010,
        CDS_VIDEOPARAMETERS = 0x00000020,
        CDS_ENABLE_UNSAFE_MODES = 0x00000100,
        CDS_DISABLE_UNSAFE_MODES = 0x00000200,
        CDS_RESET = 0x40000000,
        CDS_RESET_EX = 0x20000000,
        CDS_NORESET = 0x10000000
    }

    [DllImport("user32.dll")]
    public static extern DISP_CHANGE ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, ChangeDisplaySettingsFlags dwflags, IntPtr lParam);

    [Flags()]
    public enum DisplayDeviceStateFlags : int {
        /// <summary>The device is part of the desktop.</summary>
        AttachedToDesktop = 0x1,
        MultiDriver = 0x2,
        /// <summary>The device is part of the desktop.</summary>
        PrimaryDevice = 0x4,
        /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
        MirroringDriver = 0x8,
        /// <summary>The device is VGA compatible.</summary>
        VGACompatible = 0x10,
        /// <summary>The device is removable; it cannot be the primary display.</summary>
        Removable = 0x20,
        /// <summary>The device has more display modes than its output devices support.</summary>
        ModesPruned = 0x8000000,
        Remote = 0x4000000,
        Disconnect = 0x2000000
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct DISPLAY_DEVICE {
        [MarshalAs(UnmanagedType.U4)]
        public int cb;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceString;
        [MarshalAs(UnmanagedType.U4)]
        public DisplayDeviceStateFlags StateFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceKey;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

    [DllImport("user32.dll")]
    public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

    [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLongPtr")]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, long dwNewLong);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    public const int GWL_STYLE = -16;
    public const int GWL_EXSTYLE = -20;

    public const Int64 WS_CAPTION = 0x00C00000;
    public const Int64 WS_SYSMENU = 0x00080000;
    public const Int64 WS_BORDER = 0x00800000;
    public const Int64 WS_THICKFRAME = 0x00040000;
    public const Int64 WS_MINIMIZEBOX = 0x00020000;
    public const Int64 WS_MAXIMIZEBOX = 0x00020000;

    public const Int64 WS_EX_LAYERED = 0x00080000;
    public const Int64 WS_EX_DLGMODALFRAME = 0x00000001;
    public const Int64 WS_EX_CLIENTEDGE = 0x00000200;
    public const Int64 WS_EX_STATICEDGE = 0x00020000;

    public const uint LWA_ALPHA = 0x00000002;

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, UInt32 uFlags);

    public const int SWP_FRAMECHANGED = 0x0020;
    public const int SWP_NOMOVE = 0x0002;
    public const int SWP_NOSIZE = 0x0001;
    public const int SWP_NOZORDER = 0x0004;
    public const int SWP_NOOWNERZORDER = 0x0200;

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int GetClassName(IntPtr hWnd, char[] lpClassName, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);
}
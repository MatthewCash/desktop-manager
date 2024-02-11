using System;
using System.Runtime.InteropServices;

static class WindowAccentState {
    [DllImport("user32.dll")]
    static extern int SetWindowCompositionAttribute(IntPtr hWnd, ref WindowCompositionAttributeData data);

    [StructLayout(LayoutKind.Sequential)]
    public struct WindowCompositionAttributeData {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    public enum WindowCompositionAttribute {
        WCA_ACCENT_POLICY = 19
    }

    public enum AccentState {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
        ACCENT_ENABLE_TRANSPARENT = 6,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AccentPolicy {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLongPtr")]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, long dwNewLong);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    public const int GWL_STYLE = -16;
    public const int GWL_EXSTYLE = -20;
    public const uint LWA_ALPHA = 0x00000002;
    public const long WS_CAPTION = 0x00C00000;
    public const long WS_SYSMENU = 0x00080000;
    public const long WS_BORDER = 0x00800000;
    public const long WS_THICKFRAME = 0x00040000;
    public const long WS_MINIMIZEBOX = 0x00020000;
    public const long WS_MAXIMIZEBOX = 0x00020000;

    public const long WS_EX_LAYERED = 0x00080000;
    public const long WS_EX_DLGMODALFRAME = 0x00000001;
    public const long WS_EX_CLIENTEDGE = 0x00000200;
    public const long WS_EX_STATICEDGE = 0x00020000;

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

    public static void SetAccentState(IntPtr hWnd, AccentState accentState) {
        AccentPolicy accent = new() {
            AccentState = accentState
        };

        int accentStructSize = Marshal.SizeOf(accent);

        IntPtr accentPtr = Marshal.AllocHGlobal(accentStructSize);
        Marshal.StructureToPtr(accent, accentPtr, false);

        WindowCompositionAttributeData data = new() {
            Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
            SizeOfData = accentStructSize,
            Data = accentPtr
        };

        _ = SetWindowCompositionAttribute(hWnd, ref data);

        Marshal.FreeHGlobal(accentPtr);
    }

    public static void SetTransparency(IntPtr hWnd, byte alpha) {
        long exStyle = (long) GetWindowLongPtr(hWnd, GWL_EXSTYLE);
        if ((exStyle & WS_EX_LAYERED) == 0) SetWindowLongPtr(hWnd, GWL_EXSTYLE, exStyle | WS_EX_LAYERED);
        SetLayeredWindowAttributes(hWnd, 0, alpha, LWA_ALPHA);
    }
}

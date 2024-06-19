using System;
using System.Runtime.InteropServices;

static class WindowStateManager {
    public enum WindowState {
        Normal = 1,
        Maximized = 3,
        Minimized = 6,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT {
        public int length;
        public int flags;
        public WindowState showCmd;
        public System.Drawing.Point ptMinPosition;
        public System.Drawing.Point ptMaxPosition;
        public System.Drawing.Rectangle rcNormalPosition;
    }

    [DllImport("user32.dll")]
    public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

    [DllImport("user32.dll")]
    public static extern bool ShowWindowAsync(IntPtr hWnd, WindowState nCmdShow);
}

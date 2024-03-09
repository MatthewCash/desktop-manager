using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

static class WindowManager {
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetWindowTextLength(IntPtr hWnd);

    static IntPtr FindWindow(string className, string processName, string title) {
        IntPtr hWndFound = IntPtr.Zero;
        EnumWindows((hWnd, param) => {
            if (className is not null) {
                StringBuilder classBuilder = new(256);
                GetClassName(hWnd, classBuilder, classBuilder.Capacity);

                if (classBuilder.ToString().Trim('\0') != className) return true;
            }

            if (processName is not null) {
                GetWindowThreadProcessId(hWnd, out var processId);
                if (Process.GetProcessById(processId).ProcessName != processName) return true;
            }

            if (title is not null) {
                StringBuilder windowTitle = new(GetWindowTextLength(hWnd) + 1);
                GetWindowText(hWnd, windowTitle, windowTitle.Capacity);

                if (windowTitle.ToString().Trim('\0') != title) return true;
            }

            hWndFound = hWnd;
            return true;
        }, IntPtr.Zero);

        return hWndFound;
    }

    public static void FixAllWindows() {
        foreach (var window in Config.GetConfig().Windows) {
            IntPtr hWnd = FindWindow(window.Class, window.Process, window.Title);
            if (hWnd == IntPtr.Zero) continue;

            if (window.Position is not null) WindowPosition.SetPosition(hWnd, new(window.Position));
            if (window.Transparency is not null) WindowAccentState.SetTransparency(hWnd, (byte) window.Transparency);

            if (window.Minimized) WindowState.Minimize(hWnd);
        }
    }
}

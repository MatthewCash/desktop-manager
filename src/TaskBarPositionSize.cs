using System;
using System.Runtime.InteropServices;

static class TaskBarPositionSize {
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    public static void SetTaskbarPositionSize(IntPtr taskbarHWnd) {
        MoveWindow(taskbarHWnd, 2560, 1391, 1913, 40, true);
    }
}
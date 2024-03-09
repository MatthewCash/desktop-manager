using System;
using System.Runtime.InteropServices;

static class WindowState {

    [DllImport("user32.dll")]
    private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

    const int SW_MAXIMIZE = 3;
    const int SW_MINIMIZE = 6;
    const int SW_NORMAL = 1;

    public static void Minimize(IntPtr hWnd) {
        ShowWindowAsync(hWnd, SW_MINIMIZE);
    }
}

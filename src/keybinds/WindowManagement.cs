using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

class WindowManagement : Keybinds.IKeybindHandler {
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT {
        public int length;
        public int flags;
        public int showCmd;
        public System.Drawing.Point ptMinPosition;
        public System.Drawing.Point ptMaxPosition;
        public System.Drawing.Rectangle rcNormalPosition;
    }


    [DllImport("user32.dll")]
    public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

    const int SW_MAXIMIZE = 3;
    const int SW_MINIMIZE = 6;
    const int SW_NORMAL = 1;

    private static bool ShouldIgnoreWindow(IntPtr hWnd) {
        var classNameBuffer = new char[256];
        _ = User32Wrapper.GetClassName(hWnd, classNameBuffer, classNameBuffer.Length);
        string className = new string(classNameBuffer).Trim('\0');

        return Config.GetConfig().DisabledWindows.Contains(className);
    }

    private static bool IsWindowMaximized(IntPtr hWnd) {
        WINDOWPLACEMENT placement = new() { length = Marshal.SizeOf<WINDOWPLACEMENT>() };
        GetWindowPlacement(hWnd, ref placement);
        return placement.showCmd == SW_MAXIMIZE;
    }

    public void MouseDown(object sender, MouseEventExtArgs e) {
        if (
            !(Keybinds.trackedKeyStates[Keys.LControlKey] ||
            Keybinds.trackedButtonStates[MouseButtons.Middle])
        ) return;

        if (e.Button == MouseButtons.XButton1) {
            e.Handled = true;

            void handler(object sender, MouseEventExtArgs e) {
                Keybinds.trackedButtonStates[e.Button] = false;
                if (e.Button != MouseButtons.XButton1) return;
                e.Handled = true;
                Keybinds.inputHook.MouseUpExt -= handler;
            };
            Keybinds.inputHook.MouseUpExt += handler;

            var hWnd = GetForegroundWindow();
            if (ShouldIgnoreWindow(hWnd)) return;

            if (Keybinds.trackedKeyStates[Keys.LMenu]) {
                // Restore
                ShowWindowAsync(hWnd, SW_NORMAL);
            } else {
                // Minimize
                ShowWindowAsync(hWnd, SW_MINIMIZE);
            }
        }
        if (e.Button == MouseButtons.XButton2) {
            e.Handled = true;

            void handler(object sender, MouseEventExtArgs e) {
                Keybinds.trackedButtonStates[e.Button] = false;
                if (e.Button != MouseButtons.XButton2) return;
                e.Handled = true;
                Keybinds.inputHook.MouseUpExt -= handler;
            };
            Keybinds.inputHook.MouseUpExt += handler;

            var hWnd = GetForegroundWindow();
            if (ShouldIgnoreWindow(hWnd)) return;

            if (IsWindowMaximized(hWnd)) {
                // Restore
                ShowWindowAsync(hWnd, SW_NORMAL);
            } else {
                // Maximize
                ShowWindowAsync(hWnd, SW_MAXIMIZE);
            }
        }
    }
}

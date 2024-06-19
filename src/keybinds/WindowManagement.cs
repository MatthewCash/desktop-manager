using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using static WindowStateManager;

class WindowManagement : Keybinds.IKeybindHandler {
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    private static bool ShouldIgnoreWindow(IntPtr hWnd) {
        var classNameBuffer = new char[256];
        _ = User32Wrapper.GetClassName(hWnd, classNameBuffer, classNameBuffer.Length);
        string className = new string(classNameBuffer).Trim('\0');

        return Config.GetConfig().DisabledWindows.Contains(className);
    }

    private static bool IsWindowMaximized(IntPtr hWnd) {
        WINDOWPLACEMENT placement = new() { length = Marshal.SizeOf<WINDOWPLACEMENT>() };
        GetWindowPlacement(hWnd, ref placement);
        return placement.showCmd == WindowState.Maximized;
    }

    public void MouseDown(object sender, MouseEventExtArgs e) {
        if (
            !(Keybinds.TrackedKeyStates[Keys.LControlKey] ||
            Keybinds.TrackedButtonStates[MouseButtons.Middle])
        ) return;

        if (e.Button == MouseButtons.XButton1) {
            e.Handled = true;

            void handler(object sender, MouseEventExtArgs e) {
                Keybinds.TrackedButtonStates[e.Button] = false;
                if (e.Button != MouseButtons.XButton1) return;
                e.Handled = true;
                Keybinds.InputHook.MouseUpExt -= handler;
            };
            Keybinds.InputHook.MouseUpExt += handler;

            var hWnd = GetForegroundWindow();
            if (ShouldIgnoreWindow(hWnd)) return;

            if (Keybinds.TrackedKeyStates[Keys.LMenu]) {
                // Restore
                ShowWindowAsync(hWnd, WindowState.Normal);
            } else {
                // Minimize
                ShowWindowAsync(hWnd, WindowState.Minimized);
            }
        }
        if (e.Button == MouseButtons.XButton2) {
            e.Handled = true;

            void handler(object sender, MouseEventExtArgs e) {
                Keybinds.TrackedButtonStates[e.Button] = false;
                if (e.Button != MouseButtons.XButton2) return;
                e.Handled = true;
                Keybinds.InputHook.MouseUpExt -= handler;
            };
            Keybinds.InputHook.MouseUpExt += handler;

            var hWnd = GetForegroundWindow();
            if (ShouldIgnoreWindow(hWnd)) return;

            if (IsWindowMaximized(hWnd)) {
                // Restore
                ShowWindowAsync(hWnd, WindowState.Normal);
            } else {
                // Maximize
                ShowWindowAsync(hWnd, WindowState.Maximized);
            }
        }
    }
}

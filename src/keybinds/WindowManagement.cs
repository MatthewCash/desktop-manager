using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

class WindowManagement : Keybinds.IKeybindHandler {
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

    const int SW_MAXIMIZE = 3;
    const int SW_MINIMIZE = 6;
    const int SW_NORMAL = 1;

    private static bool ShouldIgnoreWindow(IntPtr hWnd) {
        var classNameBuffer = new char[256];
        _ = User32Wrapper.GetClassName(hWnd, classNameBuffer, classNameBuffer.Length);
        string className = new string(classNameBuffer).Trim('\0');

        return Config.GetConfig().DisabledWindows.Contains(className);
    }

    public void MouseDown(object sender, MouseEventExtArgs e) {
        if (
            Keybinds.trackedKeyStates[Keys.LControlKey] ||
            Keybinds.trackedButtonStates[MouseButtons.Middle]
        ) {
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

                ShowWindowAsync(hWnd, SW_MAXIMIZE);
            }
        }
    }
}

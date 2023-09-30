using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

static class Keybinds {
    public interface IKeybindHandler {
        void MouseDown(object sender, MouseEventExtArgs e) { }
        void MouseUp(object sender, MouseEventExtArgs e) { }
        void KeyDown(object sender, KeyEventArgs e) { }
        void KeyUp(object sender, KeyEventArgs e) { }
    }

    public static List<IKeybindHandler> handlers = new();

    public static IKeyboardMouseEvents inputHook;

    public static Dictionary<Keys, bool> trackedKeyStates = new();
    public static Dictionary<MouseButtons, bool> trackedButtonStates = new();

    static void KeyDown(object sender, KeyEventArgs e) {
        trackedKeyStates[e.KeyCode] = true;
    }

    static void KeyUp(object sender, KeyEventArgs e) {
        trackedKeyStates[e.KeyCode] = false;
    }

    static void MouseDown(object sender, MouseEventExtArgs e) {
        trackedButtonStates[e.Button] = true;
    }

    static void MouseUp(object sender, MouseEventExtArgs e) {
        trackedButtonStates[e.Button] = false;
    }

    public static void DeregisterKeybinds() {
        inputHook.MouseDownExt -= MouseDown;
        inputHook.MouseUpExt -= MouseUp;
        inputHook.KeyDown -= KeyDown;
        inputHook.KeyUp -= KeyUp;

        inputHook.Dispose();
    }

    public static void RegisterKeybinds() {
        foreach (Keys key in Enum.GetValues(typeof(Keys)))
            trackedKeyStates[key] = false;

        foreach (MouseButtons button in Enum.GetValues(typeof(MouseButtons)))
            trackedButtonStates[button] = false;

        inputHook = Hook.GlobalEvents();

        // Listeners for tracking button/key states
        inputHook.MouseDownExt += MouseDown;
        inputHook.MouseUpExt += MouseUp;
        inputHook.KeyDown += KeyDown;
        inputHook.KeyUp += KeyUp;

        handlers.ForEach(handler => {
            inputHook.MouseDownExt += handler.MouseDown;
            inputHook.MouseUpExt += handler.MouseUp;
            inputHook.KeyDown += handler.KeyDown;
            inputHook.KeyUp += handler.KeyUp;
        });
    }
}

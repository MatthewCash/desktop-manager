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

    public static List<IKeybindHandler> Handlers { get; } = new();

    public static IKeyboardMouseEvents InputHook { get; private set; }

    public static Dictionary<Keys, bool> TrackedKeyStates { get; } = new();
    public static Dictionary<MouseButtons, bool> TrackedButtonStates { get; } = new();

    private static void KeyDown(object sender, KeyEventArgs e) {
        TrackedKeyStates[e.KeyCode] = true;
    }

    private static void KeyUp(object sender, KeyEventArgs e) {
        TrackedKeyStates[e.KeyCode] = false;
    }

    private static void MouseDown(object sender, MouseEventExtArgs e) {
        TrackedButtonStates[e.Button] = true;
    }

    private static void MouseUp(object sender, MouseEventExtArgs e) {
        TrackedButtonStates[e.Button] = false;
    }

    public static void DeregisterKeybinds() {
        InputHook.MouseDownExt -= MouseDown;
        InputHook.MouseUpExt -= MouseUp;
        InputHook.KeyDown -= KeyDown;
        InputHook.KeyUp -= KeyUp;

        InputHook.Dispose();
    }

    public static void RegisterKeybinds() {
        foreach (Keys key in Enum.GetValues(typeof(Keys)))
            TrackedKeyStates[key] = false;

        foreach (MouseButtons button in Enum.GetValues(typeof(MouseButtons)))
            TrackedButtonStates[button] = false;

        InputHook = Hook.GlobalEvents();

        // Listeners for tracking button/key states
        InputHook.MouseDownExt += MouseDown;
        InputHook.MouseUpExt += MouseUp;
        InputHook.KeyDown += KeyDown;
        InputHook.KeyUp += KeyUp;

        Handlers.ForEach(handler => {
            InputHook.MouseDownExt += handler.MouseDown;
            InputHook.MouseUpExt += handler.MouseUp;
            InputHook.KeyDown += handler.KeyDown;
            InputHook.KeyUp += handler.KeyUp;
        });
    }
}

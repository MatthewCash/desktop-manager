using System;
using System.Runtime.InteropServices;

class Taskbar {
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    public IntPtr hWnd;

    TaskbarPosition.TaskbarRect position = null;
    TaskbarAccent.AccentState accentState;

    EventManager eventManager;

    public Taskbar(Boolean primary, TaskbarPosition.TaskbarRect position, TaskbarAccent.AccentState accentState) {
        String taskbarClass = primary ? "Shell_TrayWnd" : "Shell_SecondaryTrayWnd";
        hWnd = FindWindow(taskbarClass, null);

        this.position = position;
        this.accentState = accentState;
    }

    public void SetPosition() {
        TaskbarPosition.SetTaskbarPositionSize(hWnd, position);
    }

    public void SetAccentState() {
        TaskbarAccent.SetAccentState(hWnd, accentState);
    }

    public void FixTaskbar() {
        if (position != null) SetPosition();
        SetAccentState();
    }

    public void RegisterEvents() {
        eventManager = new EventManager(this);
        eventManager.RegisterEvents();
    }
}
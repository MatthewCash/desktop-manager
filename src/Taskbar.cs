using System;

class Taskbar {
    public IntPtr taskbarHandle;
    public IntPtr startButtonHandle;
    public IntPtr clockHandle;
    public IntPtr taskListContainerHandle;
    public IntPtr taskListHandle;
    public IntPtr trayNotifyHandle;

    public bool primary;
    public bool hideStart;
    public bool clockToStart;
    readonly TaskbarPosition.TaskbarRect position = null;
    readonly WindowAccentState.AccentState accentState;

    EventManager eventManager;

    public Taskbar(
        bool primary,
        TaskbarPosition.TaskbarRect position,
        WindowAccentState.AccentState accentState,
        bool hideStart = false,
        bool clockToStart = false
    ) {
        string taskbarClass = primary ? "Shell_TrayWnd" : "Shell_SecondaryTrayWnd";

        this.primary = primary;
        this.position = position;
        this.accentState = accentState;
        this.hideStart = hideStart;
        this.clockToStart = clockToStart;

        taskbarHandle = User32Wrapper.FindWindow(taskbarClass, null);

        startButtonHandle = User32Wrapper.FindWindowEx(taskbarHandle, IntPtr.Zero, "Start", null);
        if (primary) {
            trayNotifyHandle = User32Wrapper.FindWindowEx(taskbarHandle, IntPtr.Zero, "TrayNotifyWnd", null);
            IntPtr reBarHandle = User32Wrapper.FindWindowEx(taskbarHandle, IntPtr.Zero, "ReBarWindow32", null);
            taskListContainerHandle = User32Wrapper.FindWindowEx(reBarHandle, IntPtr.Zero, "MSTaskSwWClass", null);
        } else {
            clockHandle = User32Wrapper.FindWindowEx(taskbarHandle, IntPtr.Zero, "ClockButton", null);
            taskListContainerHandle = User32Wrapper.FindWindowEx(taskbarHandle, IntPtr.Zero, "WorkerW", null);
        }
        taskListHandle = User32Wrapper.FindWindowEx(taskListContainerHandle, IntPtr.Zero, "MSTaskListWClass", null);
    }

    public void SetPosition() {
        TaskbarPosition.SetTaskbarPositionSize(taskbarHandle, position);
    }

    public void SetAccentState() {
        WindowAccentState.SetAccentState(taskbarHandle, accentState);
    }

    public void FixTaskbar() {
        if (position != null) SetPosition();
        SetAccentState();

        if (hideStart) {
            User32Wrapper.ShowWindow(startButtonHandle, 11);
            User32Wrapper.ShowWindow(startButtonHandle, 0);
        }

        if (clockToStart) {
            int clockWidth = 70;
            User32Wrapper.MoveWindow(clockHandle, 0, 0, clockWidth, position.height, true);
            User32Wrapper.MoveWindow(taskListContainerHandle, clockWidth, 0, position.width - clockWidth, position.height, true);
        }
    }

    public void RegisterEvents() {
        eventManager = new EventManager(this);
        eventManager.RegisterEvents();
    }
}

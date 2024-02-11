using System;

class Taskbar {
    public IntPtr taskbarHandle;
    public IntPtr startButtonHandle;
    public IntPtr clockHandle;
    public IntPtr taskListContainerHandle;
    public IntPtr taskListHandle;
    public IntPtr trayNotifyHandle;

    public bool primary;
    public uint monitorIndex;
    public bool hideStart;
    public bool clockToStart;
    readonly WindowPosition.WindowRect position = null;
    readonly WindowAccentState.AccentState accentState;

    EventManager eventManager;

    public Taskbar(
        bool primary,
        uint monitorIndex,
        WindowPosition.WindowRect position,
        WindowAccentState.AccentState accentState,
        bool hideStart = false,
        bool clockToStart = false
    ) {
        string taskbarClass = primary ? "Shell_TrayWnd" : "Shell_SecondaryTrayWnd";

        this.primary = primary;
        this.monitorIndex = monitorIndex;
        this.position = position;
        this.accentState = accentState;
        this.hideStart = hideStart;
        this.clockToStart = clockToStart;

        if (primary) {
            taskbarHandle = User32Wrapper.FindWindow(taskbarClass, null);
        } else {
            var _ = User32Wrapper.EnumWindows((hWnd, lParam) => {
                var classNameBuffer = new char[256];
                User32Wrapper.GetClassName(hWnd, classNameBuffer, classNameBuffer.Length);
                string className = new string(classNameBuffer).Trim('\0');

                if (className != taskbarClass) return true;

                var monitorHandle = User32Wrapper.MonitorFromWindow(hWnd, 0);
                var monitorInfo = new User32Wrapper.MONITORINFOEX();
                User32Wrapper.GetMonitorInfo(monitorHandle, monitorInfo);

                string monitorName = new string(monitorInfo.szDevice).Trim('\0');
                if (monitorName == $"\\\\.\\DISPLAY{monitorIndex + 1}") taskbarHandle = hWnd;

                return true;
            }, 0);
        }

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
        WindowPosition.SetPosition(taskbarHandle, position);
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

    public static void FixAllTaskbars() {
        var taskbarConfigs = Config.GetConfig().Taskbars;

        foreach (var taskbarConfig in taskbarConfigs) {
            WindowPosition.WindowRect position = taskbarConfig.Position is null ? null : new(taskbarConfig.Position);

            Taskbar taskbar = new(
                taskbarConfig.MonitorIndex == -1,
                (uint) taskbarConfig.MonitorIndex,
                position,
                (WindowAccentState.AccentState) taskbarConfig.AccentState,
                taskbarConfig.HideStart,
                taskbarConfig.ClockToStart
            );

            taskbar.FixTaskbar();
            taskbar.RegisterEvents();
        }
    }
}

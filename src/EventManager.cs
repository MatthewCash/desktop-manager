using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

class EventManager {
    [DllImport("user32.dll", SetLastError = true)]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    [DllImport("user32.dll")]
    static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    const uint WM_CUSTOM_EXIT = 0x0400 + 2000;
    const uint WINEVENT_OUTOFCONTEXT = 0;
    const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;
    const uint EVENT_OBJECT_FOCUS = 0x8005;

    static readonly List<WinEventDelegate> eventDelegates = new();
    readonly Taskbar taskbar;

    public EventManager(Taskbar taskbar) {
        this.taskbar = taskbar;
    }

    public void RegisterEvents() {
        GetWindowThreadProcessId(taskbar.taskbarHandle, out uint taskBarProcessId);

        WinEventDelegate locationChangeDelegate = new(OnObjectLocationChange);
        eventDelegates.Add(locationChangeDelegate);
        SetWinEventHook(EVENT_OBJECT_LOCATIONCHANGE, EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, locationChangeDelegate, taskBarProcessId, 0, WINEVENT_OUTOFCONTEXT);

        WinEventDelegate objectFocusDelegate = new(OnObjectFocus);
        eventDelegates.Add(objectFocusDelegate);
        SetWinEventHook(EVENT_OBJECT_FOCUS, EVENT_OBJECT_FOCUS, IntPtr.Zero, objectFocusDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);
    }

    void OnObjectFocus(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) {
        taskbar.FixTaskbar();
    }

    void OnObjectLocationChange(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) {
        if (hWnd != taskbar.taskbarHandle) return;

        taskbar.FixTaskbar();
    }

    public static void StartMessagePump() {
        var msg = new User32Wrapper.MSG();
        while (User32Wrapper.GetMessage(ref msg, IntPtr.Zero, 0, 0)) {
            if (WM_CUSTOM_EXIT == msg.message) break;

            User32Wrapper.TranslateMessage(ref msg);
            User32Wrapper.DispatchMessage(ref msg);
        }
    }
}

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

static class EventManager {
    [DllImport("user32.dll", SetLastError = true)]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    [DllImport("user32.dll")]
    static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    const uint WM_CUSTOM_EXIT = 0x0400 + 2000;
    const uint WINEVENT_OUTOFCONTEXT = 0;
    const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;

    static List<WinEventDelegate> eventDelegates;

    public static void RegisterEvents() {
        eventDelegates = new List<WinEventDelegate>();

        uint taskBarProcessId;
        GetWindowThreadProcessId(TaskBarManager.Program.taskbarHWnd, out taskBarProcessId);

        WinEventDelegate eventDelegate = new WinEventDelegate(WinEventProc);
        eventDelegates.Add(eventDelegate);
        IntPtr m_hhook = SetWinEventHook(EVENT_OBJECT_LOCATIONCHANGE, EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, eventDelegate, taskBarProcessId, 0, WINEVENT_OUTOFCONTEXT);
    }

    public static void StartMessagePump() {
        User32Wrapper.MSG msg = new User32Wrapper.MSG();
        while (User32Wrapper.GetMessage(ref msg, IntPtr.Zero, 0, 0)) {
            if (WM_CUSTOM_EXIT == msg.message) break;

            User32Wrapper.TranslateMessage(ref msg);
            User32Wrapper.DispatchMessage(ref msg);
        }
    }

    static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) {
        IntPtr taskbarHWnd = TaskBarManager.Program.taskbarHWnd;
        if (taskbarHWnd != hwnd) return;

        TaskBarTransparency.SetTaskbarTransparent(taskbarHWnd);
        TaskBarPositionSize.SetTaskbarPositionSize(taskbarHWnd);
    }
}
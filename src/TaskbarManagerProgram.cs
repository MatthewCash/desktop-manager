using System;
using System.Runtime.InteropServices;

namespace TaskBarManager {
    static class Program {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public static IntPtr taskbarHWnd;

        [STAThread]
        static void Main() {
            taskbarHWnd = FindWindow("Shell_SecondaryTrayWnd", null);

            TaskBarTransparency.SetTaskbarTransparent(taskbarHWnd);
            TaskBarPositionSize.SetTaskbarPositionSize(taskbarHWnd);

            EventManager.RegisterEvents();

            Console.WriteLine("Starting message pump");
            EventManager.StartMessagePump();
        }
    }
}
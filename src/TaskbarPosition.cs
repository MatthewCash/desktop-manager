using System;
using System.Runtime.InteropServices;

static class TaskbarPosition {
    public class TaskbarRect {
        public int x;
        public int y;
        public int width;
        public int height;

        public TaskbarRect(int x, int y, int width, int height) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    public static void SetTaskbarPositionSize(IntPtr taskbarHWnd, TaskbarRect position) {
        MoveWindow(taskbarHWnd, position.x, position.y, position.width, position.height, true);
    }
}

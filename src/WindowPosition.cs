using System;
using System.Runtime.InteropServices;

static class WindowPosition {
    public class WindowRect {
        public int x;
        public int y;
        public int width;
        public int height;

        public WindowRect(int x, int y, int width, int height) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public WindowRect(int[] position) {
            x = position[0];
            y = position[1];
            width = position[2];
            height = position[3];
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    public static void SetPosition(IntPtr hWnd, WindowRect position) {
        MoveWindow(hWnd, position.x, position.y, position.width, position.height, true);
    }
}

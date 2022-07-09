using System;
using System.Runtime.InteropServices;

public class User32Wrapper {
    [DllImport(@"user32.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    public static extern bool GetMessage(ref MSG message, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport(@"user32.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    public static extern bool TranslateMessage(ref MSG message);

    [DllImport(@"user32.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    public static extern long DispatchMessage(ref MSG message);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT {
        long x;
        long y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG {
        IntPtr hwnd;
        public uint message;
        UIntPtr wParam;
        IntPtr lParam;
        uint time;
        POINT pt;
    }
}
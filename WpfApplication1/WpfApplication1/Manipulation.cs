using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace WpfApplication1
{
    public class Manipulation
    {
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("User32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("User32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("User32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);


        public void moveCursor(int a, int b)
        {
            SetCursorPos(a, b);
        }

        public void leftClick(int a, int b)
        {
            mouse_event(Manipulation.MOUSEEVENTF_LEFTDOWN, a, b, 0, 0);
            mouse_event(Manipulation.MOUSEEVENTF_LEFTUP, a, b, 0, 0);
        }

        public void moveWindow(int dis)
        {
            IntPtr currentWindow = GetForegroundWindow();
            StringBuilder buff = new StringBuilder(256);
            if (GetWindowText(currentWindow, buff, 256) > 0)
            {
                Debug.Print(buff.ToString());
            }

            MoveWindow(currentWindow, 500 - dis, 500 - dis, 600 + dis, 600 + dis, true);

        }
    }
}
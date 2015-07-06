using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace WpfApplication1
{
    public class MyWindow
    {
        [DllImport("User32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("User32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        
        public static void moveWindow(int dis)
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
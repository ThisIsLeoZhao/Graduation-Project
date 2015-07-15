using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using SingleKinect.MyUtilities;

namespace SingleKinect.Manipulator
{
    public struct RECT
    {
        public int Bottom; // y position of lower-right corner
        public int Left; // x position of upper-left corner
        public int Right; // x position of lower-right corner
        public int Top; // y position of upper-left corner
    }

    public class MyWindow
    {
        //PInvoke
        [DllImport("User32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("User32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("User32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("User32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static void moveWindow(int dis)
        {
            var currentWindow = GetForegroundWindow();

            RECT rct;
            GetWindowRect(currentWindow, out rct);

            var myRect = new Rect();

            var buff = new StringBuilder(256);
            if (GetWindowText(currentWindow, buff, 256) > 0)
            {
                Debug.Print(buff.ToString());
            }

            myRect.X = rct.Left - dis/2 < 0 ? 0 : rct.Left - dis/2;
            myRect.Y = rct.Top - dis/2 < 0 ? 0 : rct.Top - dis/2;

            myRect.Width = rct.Right - myRect.X + 1 + dis > CoordinateConverter.screenWidth
                ? CoordinateConverter.screenWidth
                : rct.Right - myRect.X + 1 + dis;

            myRect.Height = rct.Bottom - myRect.Y + 1 + dis > CoordinateConverter.screenHeight
                ? CoordinateConverter.screenHeight
                : rct.Bottom - myRect.Y + 1 + dis;

            Debug.Print("rct {0}, {1}, {2}, {3}", rct.Left, rct.Top, myRect.Width, myRect.Height);


            MoveWindow(currentWindow, (int) myRect.Left - dis/2, (int) myRect.Top - dis/2,
                (int) myRect.Width + dis, (int) myRect.Height + dis, true);
        }
    }
}
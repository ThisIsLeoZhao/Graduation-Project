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
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
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
        internal static extern bool GetWindowRect(IntPtr hwnd, ref RECT lpRect);

        [DllImport("User32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static void moveWindow(int dis)
        {
            var currentWindow = GetForegroundWindow();

            RECT rct = new RECT
            {
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                Right = 0,
                Left = 0,
                Bottom = 0,
                Top = 0
            };

            GetWindowRect(currentWindow, ref rct);

            var myRect = new Rect();

            var buff = new StringBuilder(256);
            if (GetWindowText(currentWindow, buff, 256) > 0)
            {
                Debug.Print(buff.ToString());
            }

            Debug.Print("rct {0}, {1}, {2}, {3}", rct.Left, rct.Top, rct.Right, rct.Bottom);

            myRect.X = rct.Left - dis/2 < 0 ? 0 : rct.Left - dis/2;
            myRect.Y = rct.Top - dis/2 < 0 ? 0 : rct.Top - dis/2;

            myRect.Width = rct.Right - myRect.X + 1 + dis > CoordinateConverter.screenWidth
                ? CoordinateConverter.screenWidth
                : rct.Right - myRect.X + 1 + dis;

            //Task bar has the height of 50 pixels
            myRect.Height = rct.Bottom - myRect.Y + 1 + dis > CoordinateConverter.screenHeight - 50
                ? CoordinateConverter.screenHeight - 50
                : rct.Bottom - myRect.Y + 1 + dis;

            Debug.Print("myRect {0}, {1}, {2}, {3}", (int) myRect.X, (int) myRect.Y, (int) myRect.Width, (int) myRect.Height);

            MoveWindow(currentWindow, (int) myRect.X, (int) myRect.Y,
                (int) myRect.Width, (int) myRect.Height, true);
        }
    }
}
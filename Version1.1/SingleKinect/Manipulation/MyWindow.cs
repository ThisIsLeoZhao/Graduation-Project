using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using SingleKinect.CoordinateConvert;
using SingleKinect.Manipulation.InputConstants;
using SingleKinect.MyDataStructures;

namespace SingleKinect.Manipulation
{
    public class MyWindow
    {
        private static RECT rct;
        private static RECT incrementRect;
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

        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs,
            [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs,
            int cbSize);

        public static void moveWindow(RECT rect)
        {
            incrementRect = rect;
            Debug.Print("incrementRect {0}, {1}, {2}, {3}", incrementRect.Left, incrementRect.Top,
                incrementRect.Right, incrementRect.Bottom);

            var currentWindow = GetForegroundWindow();

            rct = new RECT
            {
                Right = 0,
                Left = 0,
                Bottom = 0,
                Top = 0
            };

            GetWindowRect(currentWindow, ref rct);
            Debug.Print("rct {0}, {1}, {2}, {3}", rct.Left, rct.Top, rct.Right, rct.Bottom);

            var newRect = new Rect
            {
                X = rct.Left + incrementRect.Left < 0 ? 0 : rct.Left + incrementRect.Left,
                Y = rct.Top + incrementRect.Top < 0 ? 0 : rct.Top + incrementRect.Top
            };

            newRect.Width = updateEdge("Right", newRect.X, CoordinateConverter.SCREEN_WIDTH);
            //Task bar has the height of 50 pixels
            newRect.Height = updateEdge("Bottom", newRect.Y, CoordinateConverter.SCREEN_HEIGHT - 50);

            MoveWindow(currentWindow, (int) newRect.X, (int) newRect.Y,
                (int) newRect.Width, (int) newRect.Height, true);
        }

        private static int updateEdge(string edge, double oppositeEdge, int screenLimit)
        {
            if (rct[edge] + incrementRect[edge] - oppositeEdge < 50)
            {
                return 50;
            }

            var result = (int) (rct[edge] + incrementRect[edge] - oppositeEdge);
            if (result > screenLimit - oppositeEdge)
            {
                return (int) (screenLimit - oppositeEdge);
            }

            return result;
        }
    }
}
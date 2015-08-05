using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using SingleKinect.Manipulator.MyDataStructures;
using SingleKinect.Manipulator.SystemConstants;
using SingleKinect.Manipulator.SystemConstants.Keyboard;
using SingleKinect.MyUtilities;

namespace SingleKinect.Manipulator
{
    public class MyWindow
    {
        //PInvoke
        [DllImport("User32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("User32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        internal static extern int GetSystemMetrics(SystemMetric smIndex);

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

        private static RECT rct;
        private static RECT incrementRect;

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

            int result = (int) (rct[edge] + incrementRect[edge] - oppositeEdge);
            if (result > screenLimit - oppositeEdge)
            {
                return (int) (screenLimit - oppositeEdge);
            }

            return result;
        }

//        public static void scrollWindow(int dis)
//        {
//            // Positive value for scroll right
//
//            INPUT[] input = new INPUT[1];
//            input[0].type = 1;
//
//            var key = dis > 0 ? VirtualKeyShort.RIGHT : VirtualKeyShort.LEFT;
//
//            input[0].U.ki = new KEYBDINPUT
//            {
//                wVk = key,
//                wScan = 0,
//                dwFlags = 0,
//                time = 0,
//                dwExtraInfo = new UIntPtr(0)
//            };
//            Debug.Print("key {0}", key);
//
//            while (dis != 0)
//            {
//                input[0].U.ki.dwFlags = 0;
//                SendInput(1, input, INPUT.Size);
//
//                input[0].U.ki.dwFlags = KEYEVENTF.KEYUP; // KEYEVENTF_KEYUP for key release
//                SendInput(1, input, INPUT.Size);
//
//                dis = dis > 0 ? dis - 1 : dis + 1;
//            }
//            
//        }
        
    }
}
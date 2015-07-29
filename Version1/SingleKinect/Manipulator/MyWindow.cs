using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using SingleKinect.Manipulator.MyDataStructures;
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

        public static void moveWindow(RECT incrementRect)
        {
            Debug.Print("incrementRect {0}, {1}, {2}, {3}", incrementRect.Left, incrementRect.Top,
                incrementRect.Right, incrementRect.Bottom);

            var currentWindow = GetForegroundWindow();

            var rct = new RECT
            {
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

            myRect.X = rct.Left + incrementRect.Left;
            myRect.Y = rct.Top + incrementRect.Top;

            if (myRect.X < 0)
            {
                myRect.X = 0;
            }
            if (myRect.Y < 0)
            {
                myRect.Y = 0;
            }

            if (rct.Right + incrementRect.Right - myRect.X + 1 < 50)
            {
                myRect.Width = 50;
            }
            else
            {
                myRect.Width = rct.Right + incrementRect.Right - myRect.X + 1;
                if (myRect.Width > CoordinateConverter.SCREEN_WIDTH)
                {
                    myRect.Width = CoordinateConverter.SCREEN_WIDTH;
                }
            }

            if (rct.Bottom + incrementRect.Bottom - myRect.Y + 1 < 50)
            {
                myRect.Height = 50;
            }
            else
            {
                myRect.Height = rct.Bottom + incrementRect.Bottom - myRect.Y + 1;
                //Task bar has the height of 50 pixels
                if (myRect.Height > CoordinateConverter.SCREEN_HEIGHT - 50)
                {
                    myRect.Height = CoordinateConverter.SCREEN_HEIGHT - 50;
                }
            }

            Debug.Print("myRect {0}, {1}, {2}, {3}", (int) myRect.X, (int) myRect.Y, (int) myRect.Width,
                (int) myRect.Height);

            MoveWindow(currentWindow, (int) myRect.X, (int) myRect.Y,
                (int) myRect.Width, (int) myRect.Height, true);
        }

        public static void scrollWindow(int dis, bool vertical)
        {
            INPUT[] input = new INPUT[1];
            input[0].type = 1;

            VirtualKeyShort key;
            if (vertical)
            {
                key = dis > 0 ? VirtualKeyShort.UP : VirtualKeyShort.DOWN;
            }
            else
            {
                key = dis > 0 ? VirtualKeyShort.RIGHT : VirtualKeyShort.LEFT;
            }

            input[0].U.ki = new KEYBDINPUT
            {
                wVk = key,
                wScan = 0,
                dwFlags = 0,
                time = 0,
                dwExtraInfo = new UIntPtr(0)
            };
            Debug.Print("key {0}", key);

            while (dis != 0)
            {
                input[0].U.ki.dwFlags = 0;
                SendInput(1, input, INPUT.Size);

                input[0].U.ki.dwFlags = KEYEVENTF.KEYUP; // KEYEVENTF_KEYUP for key release
                SendInput(1, input, INPUT.Size);

                dis = dis > 0 ? dis - 1 : dis + 1;
            }
            
        }

    }
}
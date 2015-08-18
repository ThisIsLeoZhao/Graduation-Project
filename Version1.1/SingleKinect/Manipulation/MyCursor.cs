using System;
using System.Runtime.InteropServices;
using SingleKinect.Manipulation.MyDataStructures;
using SingleKinect.Manipulation.SystemConstants;
using SingleKinect.Manipulation.SystemConstants.Mouse;
using SingleKinect.MyUtilities;

namespace SingleKinect.Manipulation
{
    public class MyCursor
    {
        [DllImport("User32.dll")]
        internal static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        internal static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("User32.dll")]
        internal static extern void mouse_event(MOUSEEVENTF dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        internal static extern int GetSystemMetrics(SystemMetric smIndex);

        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs,
            [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs,
            int cbSize);

        // Positive value for scroll up or right
        public static void scrollWindow(int dis, bool isVerticalScroll)
        {
            POINT cursorPoint;
            GetCursorPos(out cursorPoint);

            MOUSEEVENTF WHEEL = isVerticalScroll ? MOUSEEVENTF.WHEEL : MOUSEEVENTF.HWHEEL;

            INPUT[] input = new INPUT[1];
            input[0].type = 0;

            input[0].U.mi = new MOUSEINPUT
            {
                dwFlags = WHEEL | MOUSEEVENTF.ABSOLUTE,
                dwExtraInfo = new UIntPtr(0),
                dx = (cursorPoint.x * 65536) / GetSystemMetrics(SystemMetric.SM_CXSCREEN),
                dy = (cursorPoint.y * 65536) / GetSystemMetrics(SystemMetric.SM_CYSCREEN),
                mouseData = dis * 120,
                time = 0
            };

            SendInput(1, input, INPUT.Size);
        }

        public static void moveCursor(double xIncrement, double yIncrement)
        {
            POINT cursor;
            GetCursorPos(out cursor);

            cursor.x += CoordinateConverter.movementToScreen(xIncrement, false);
            cursor.y += CoordinateConverter.movementToScreen(yIncrement, true);

            SetCursorPos(cursor.x, cursor.y);
        }
    }
}
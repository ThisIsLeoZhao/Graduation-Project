using System.Runtime.InteropServices;

namespace WpfApplication1
{
    public class MyCursor
    {
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("User32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public static void moveCursor(int a, int b)
        {
            SetCursorPos(a, b);
        }

        public static void leftClick(int a, int b)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, a, b, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, a, b, 0, 0);
        }
    }
}
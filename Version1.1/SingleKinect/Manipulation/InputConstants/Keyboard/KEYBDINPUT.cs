using System;
using System.Runtime.InteropServices;

namespace SingleKinect.Manipulation.InputConstants.Keyboard
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        internal VirtualKeyShort wVk;
        internal ScanCodeShort wScan;
        internal KEYEVENTF dwFlags;
        internal int time;
        internal UIntPtr dwExtraInfo;
    }
}
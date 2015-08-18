using System.Runtime.InteropServices;
using SingleKinect.Manipulation.SystemConstants;
using SingleKinect.Manipulation.SystemConstants.Keyboard;
using SingleKinect.Manipulation.SystemConstants.Mouse;

namespace SingleKinect.Manipulation.MyDataStructures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        public uint type;
        public InputUnion U;

        public static int Size => Marshal.SizeOf(typeof (INPUT));
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct InputUnion
    {
        [FieldOffset(0)] public MOUSEINPUT mi;
        [FieldOffset(0)] internal KEYBDINPUT ki;
        [FieldOffset(0)] internal HARDWAREINPUT hi;
    }
}
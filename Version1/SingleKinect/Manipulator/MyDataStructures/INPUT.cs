using System.Runtime.InteropServices;
using SingleKinect.Manipulator.SystemConstants;
using SingleKinect.Manipulator.SystemConstants.Keyboard;
using SingleKinect.Manipulator.SystemConstants.Mouse;

namespace SingleKinect.Manipulator.MyDataStructures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        public uint type;
        public InputUnion U;

        public static int Size => Marshal.SizeOf(typeof(INPUT));
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct InputUnion
    {
        [FieldOffset(0)] public MOUSEINPUT mi;
        [FieldOffset(0)]
        internal KEYBDINPUT ki;
        [FieldOffset(0)]
        internal HARDWAREINPUT hi;
    }
}
using System.Runtime.InteropServices;

namespace SingleKinect.Manipulator.SystemConstants
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct HARDWAREINPUT
    {
        internal int uMsg;
        internal short wParamL;
        internal short wParamH;
    }
}
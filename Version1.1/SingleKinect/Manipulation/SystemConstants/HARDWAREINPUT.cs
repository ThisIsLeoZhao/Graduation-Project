using System.Runtime.InteropServices;

namespace SingleKinect.Manipulation.SystemConstants
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct HARDWAREINPUT
    {
        internal int uMsg;
        internal short wParamL;
        internal short wParamH;
    }
}
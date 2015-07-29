using Microsoft.Kinect;
using SingleKinect.Manipulator;
using SingleKinect.Manipulator.MyDataStructures;

namespace SingleKinect.EngagementManager
{
    public class EngagerTracker
    {
        public Body Engager { get; set; }
        public float ScrollDis { get; set; }
        public bool IsVerticalScroll { get; set; }
        public RECT IncrementRect { get; set; }
    }
}
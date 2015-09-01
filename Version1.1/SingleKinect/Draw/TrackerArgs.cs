using System.Security.RightsManagement;
using Microsoft.Kinect;

namespace SingleKinect.Draw
{
    public class TrackerArgs
    {
        internal Body body;
        internal HandState LeftState;
        internal HandState RightState;
        internal int Yaw;
        internal int Pitch;
        internal int Roll;

        public TrackerArgs(Body body, HandState left, HandState right, int yaw, int pitch, int roll)
        {
            this.body = body;
            LeftState = left;
            RightState = right;
            Yaw = yaw;
            Pitch = pitch;
            Roll = roll;
        }
    }
}
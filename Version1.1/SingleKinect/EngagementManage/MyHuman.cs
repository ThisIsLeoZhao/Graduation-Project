using Microsoft.Kinect;

namespace SingleKinect.EngagementManage
{
    public class MyHuman
    {
        public Body body;
        public int headPitch;
        public int headYaw;
        public int headRoll;

        public MyHuman(Body body, int headPitch = 1000, int headYaw = 1000, int headRoll = 1000)
        {
            this.body = body;
            this.headPitch = headPitch;
            this.headYaw = headYaw;
            this.headRoll = headRoll;
        }
    }
}
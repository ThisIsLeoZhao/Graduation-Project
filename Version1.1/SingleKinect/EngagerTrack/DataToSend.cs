using System;
using Microsoft.Kinect;

namespace SingleKinect.EngagerTrack
{
    public class DataToSend
    {
        public HandState RightHandState;
        public HandState LeftHandState;

        public TrackingConfidence RighTrackingConfidence;
        public TrackingConfidence LeftTrackingConfidence;

        public CameraSpacePoint RightHandJoint;
        public CameraSpacePoint LeftHandJoint;

        public bool hasReceived;
        public DateTime createdTime { get; set; }
    }
}
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Kinect;
using SingleKinect.Manipulator;
using SingleKinect.Manipulator.MyDataStructures;

namespace SingleKinect.MyUtilities
{
    public class CoordinateConverter
    {
        public static double PAN_WIDTH = 0.4;
        public static int SCREEN_WIDTH;
        public static int SCREEN_HEIGHT;
        public static double Ex;
        public static double Ey;
        public static double Ez;
        public static double PAN_HEIGHT = 0.2;
        public static KinectSensor Sensor { get; set; }

        private static float[] convertToPan(float X, float Y)
        {
            return new[]
            {
                (float) (X - Ex + PAN_WIDTH/2.0),
                (float) (Ey - Y)
            };
        }

        public static POINT cameraPointToScreen(float X, float Y)
        {
            var pan = convertToPan(X, Y);

            var screenPoint = new POINT
            {
                x = (int) (pan[0]/PAN_WIDTH*SCREEN_WIDTH),
                y = (int) (pan[1]/PAN_HEIGHT*SCREEN_HEIGHT)
            };

            return screenPoint;
        }

        public static Dictionary<JointType, DepthSpacePoint> convertJointsToDSPoints(
            IReadOnlyDictionary<JointType, Joint> jointsDic)
        {
            var joints = new Dictionary<JointType, DepthSpacePoint>();
            foreach (var joint in jointsDic)
            {
                if (joint.Value.TrackingState == TrackingState.NotTracked)
                {
                    Debug.Print("Hey");
                    continue;
                }

                var point = JointToDepthSpace(joint.Value);

                joints.Add(joint.Key, point);
            }
            return joints;
        }

        public static DepthSpacePoint JointToDepthSpace(Joint joint)
        {
            var point = Sensor.CoordinateMapper.MapCameraPointToDepthSpace(joint.Position);
            return point;
        }
    }
}
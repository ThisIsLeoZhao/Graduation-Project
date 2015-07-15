using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Kinect;

namespace SingleKinect.MyUtilities
{
    public class CoordinateConverter
    {
        public const int screenWidth = 1920;
        public const int screenHeight = 1080;
        public static double Ex;
        public static double Ey;
        public static double Ez;
        public static KinectSensor Sensor { get; set; }

        private static float[] convertToPan(float X, float Y)
        {
            return new[]
            {
                (float) (X - Ex + 0.2),
                (float) (Ey - Y)
            };
        }

        public static int[] cameraPointToScreen(float X, float Y)
        {
            var pan = convertToPan(X, Y);

            return new[]
            {
                (int) (pan[0]/0.4*screenWidth),
                (int) (pan[1]/0.2*screenHeight)
            };
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Microsoft.Kinect;

namespace WpfApplication1
{
    public class CoordinateConverter
    {
        public static double Ex;
        public static double Ey;
        public static double Ez;

        private const int screenWidth = 1920;
        private const int screenHeight = 1080;

        private static float[] convertToPan(float X, float Y)
        {
            return new float[] {
                (float)(X - Ex + 0.2),
                (float) (Ey - Y)
            };

        }

        public static int[] cameraPointToScreen(float X, float Y)
        {
            float[] pan = convertToPan(X, Y);

            return new int[] {
                (int) (pan[0] / 0.4 * screenWidth),
                (int) (pan[1] / 0.2 * screenHeight)
            };
        }

        public static Dictionary<JointType, DepthSpacePoint> convertJointsToPoints(
            IReadOnlyDictionary<JointType, Joint> jointsDic, 
            KinectSensor sensor)
        {
            Dictionary<JointType, DepthSpacePoint> joints = new Dictionary<JointType, DepthSpacePoint>();
            foreach (var joint in jointsDic)
            {
                if (joint.Value.TrackingState == TrackingState.NotTracked)
                {
                    Debug.Print("Hey");
                    continue;
                }

                DepthSpacePoint point =
                    sensor.CoordinateMapper.MapCameraPointToDepthSpace(joint.Value.Position);
                if (joint.Key == JointType.HandRight)
                {
                    Debug.Print("Original data: " + joint.Value.Position.X + ", " + joint.Value.Position.Y + ", " + joint.Value.Position.Z);
                    Debug.Print("Converted data: " + point.X + ", " + point.Y);

                }

                joints.Add(joint.Key, point);
            }
            return joints;
        }
    }
}
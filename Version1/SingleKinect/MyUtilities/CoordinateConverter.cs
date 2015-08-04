using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Kinect;
using SingleKinect.Manipulator;
using SingleKinect.Manipulator.MyDataStructures;
using SingleKinect.Manipulator.SystemConstants;

namespace SingleKinect.MyUtilities
{
    public class CoordinateConverter
    {
        public static double STEP_WIDTH;
        public static double STEP_HEIGHT;

        public static int SCREEN_WIDTH = MyWindow.GetSystemMetrics(SystemMetric.SM_CXSCREEN);
        public static int SCREEN_HEIGHT = MyWindow.GetSystemMetrics(SystemMetric.SM_CYSCREEN);
        public static double Ex;
        public static double Ey;
        public static double Ez;
        public static KinectSensor Sensor { get; set; }
        
        private static double horizontalRatio => SCREEN_WIDTH / (STEP_WIDTH * STEP_WIDTH);
        private static double verticalRatio => SCREEN_HEIGHT / (STEP_HEIGHT * STEP_HEIGHT);

        private static float[] convertToPan(float X, float Y)
        {
            return new[]
            {
                (float) (X - Ex + STEP_WIDTH/2.0),
                (float) (Ey - Y)
            };
        }

        public static int movementToScreen(double movement, bool isVertical)
        {
            Debug.Print("a {0}, b {1}", horizontalRatio, verticalRatio);
            Debug.Print("c {0}, d {1}", SCREEN_WIDTH, SCREEN_HEIGHT);
            Debug.Print("e {0}, f {1}", STEP_WIDTH, STEP_HEIGHT);
            if (!isVertical)
            {
                Debug.Print("horizontal movement: {0}", (int)(movement * Math.Abs(movement) * horizontalRatio));
                return (int) (movement * Math.Abs(movement) * horizontalRatio);
            }
            Debug.Print("vertical movement: {0}", (int)(movement * Math.Abs(movement) * verticalRatio));
            return (int) (movement * Math.Abs(movement) * verticalRatio);
        }

        public static POINT cameraPointToScreen(float X, float Y)
        {
            var pan = convertToPan(X, Y);

            var screenPoint = new POINT
            {
                x = (int) (pan[0]/STEP_WIDTH*SCREEN_WIDTH),
                y = (int) (pan[1]/STEP_HEIGHT*SCREEN_HEIGHT)
            };

            return screenPoint;
        }

        public static Dictionary<JointType, DepthSpacePoint> convertJointsToDSPoints(
            IReadOnlyDictionary<JointType, Joint> jointsDic)
        {
            var joints = new Dictionary<JointType, DepthSpacePoint>();
            foreach (var joint in jointsDic)
            {
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
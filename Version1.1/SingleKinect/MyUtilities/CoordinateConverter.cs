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

        public static double SCROLL_SENSITIVITY;

        public static int SCREEN_WIDTH = MyWindow.GetSystemMetrics(SystemMetric.SM_CXSCREEN);
        public static int SCREEN_HEIGHT = MyWindow.GetSystemMetrics(SystemMetric.SM_CYSCREEN);
        public static double Ex;
        public static double Ey;
        public static double Ez;
        public static KinectSensor Sensor { get; set; }
        
        private static double horizontalRatio => SCREEN_WIDTH / (STEP_WIDTH * STEP_WIDTH);
        private static double verticalRatio => SCREEN_HEIGHT / (STEP_HEIGHT * STEP_HEIGHT);

        public static int movementToScreen(double movement, bool isVertical)
        {
            if (!isVertical)
            {
                //Debug.Print("horizontal movement: {0}", (int)(movement * Math.Abs(movement) * horizontalRatio));
                return (int) (movement * Math.Abs(movement) * horizontalRatio);
            }
            //Debug.Print("vertical movement: {0}", (int)(movement * Math.Abs(movement) * verticalRatio));
            return (int) (movement * Math.Abs(movement) * verticalRatio);
        }

        public static int scrollToScreen(double movement, bool isVertical)
        {
            double horizontalRatio = SCROLL_SENSITIVITY / (STEP_WIDTH * STEP_WIDTH);
            double verticalRatio = SCROLL_SENSITIVITY / (STEP_HEIGHT * STEP_HEIGHT);

            if (!isVertical)
            {
                Debug.Print("horizontal scroll: {0}", (int)(movement * Math.Abs(movement) * horizontalRatio));
                return (int)(movement * Math.Abs(movement) * horizontalRatio);
            }
            Debug.Print("vertical scroll: {0}", (int)(movement * Math.Abs(movement) * verticalRatio));
            return (int)(movement * Math.Abs(movement) * verticalRatio);
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

        //        public static POINT cameraPointToScreen(float X, float Y)
        //        {
        //            var pan = convertToPan(X, Y);
        //
        //            var screenPoint = new POINT
        //            {
        //                x = (int) (pan[0]/STEP_WIDTH*SCREEN_WIDTH),
        //                y = (int) (pan[1]/STEP_HEIGHT*SCREEN_HEIGHT)
        //            };
        //
        //            return screenPoint;
        //        }
        //
        //        private static float[] convertToPan(float X, float Y)
        //        {
        //            return new[]
        //            {
        //                (float) (X - Ex + STEP_WIDTH/2.0),
        //                (float) (Ey - Y)
        //            };
        //        }


    }
}
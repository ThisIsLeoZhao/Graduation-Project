using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Kinect;
using SingleKinect.Manipulation;
using SingleKinect.MyDataStructures;

namespace SingleKinect.CoordinateConvert
{
    public class CoordinateConverter
    {
        public static double MINIMAL_BAR;
        public static double STEP_WIDTH;
        public static double STEP_HEIGHT;

        public static double SCROLL_SENSITIVITY;

        [DllImport("user32.dll")]
        internal static extern int GetSystemMetrics(SystemMetric smIndex);
        public static int SCREEN_WIDTH = GetSystemMetrics(SystemMetric.SM_CXSCREEN);
        public static int SCREEN_HEIGHT = GetSystemMetrics(SystemMetric.SM_CYSCREEN);

        public static KinectSensor Sensor { get; set; }
        
        private static double horizontalRatio => SCREEN_WIDTH / (STEP_WIDTH * STEP_WIDTH);
        private static double verticalRatio => SCREEN_HEIGHT / (STEP_HEIGHT * STEP_HEIGHT);

        public static int movementToScreen(double movement, bool isVertical)
        {
            if (!isVertical)
            {
                //Debug.Print("horizontal movement: {0}", (int)(movement * Math.Abs(movement) * horizontalRatio));
                int hDis = (int) (movement * Math.Abs(movement) * horizontalRatio);
                if (hDis > 0 && hDis < MINIMAL_BAR)
                {
                    hDis = (int) MINIMAL_BAR;
                }
                if (hDis < 0 && hDis > -MINIMAL_BAR)
                {
                    hDis = (int) -MINIMAL_BAR;
                }

                return hDis;
            }
            //Debug.Print("vertical movement: {0}", (int)(movement * Math.Abs(movement) * verticalRatio));
            int vDis = (int) (movement * Math.Abs(movement) * verticalRatio);
            if (vDis > 0 && vDis < MINIMAL_BAR)
            {
                vDis = (int) MINIMAL_BAR;
            }
            if (vDis < 0 && vDis > -MINIMAL_BAR)
            {
                vDis = (int) -MINIMAL_BAR;
            }

            return vDis;
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
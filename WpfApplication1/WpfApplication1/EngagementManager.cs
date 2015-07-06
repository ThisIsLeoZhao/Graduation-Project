using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Kinect;

namespace WpfApplication1
{
    public class EngagementManager
    {
        public IList<Body> users = new List<Body>();

        public bool engage = false;

        private int engageUserIndex = -1;
        private int[] holdTime = new int[6];
        private CameraSpacePoint handRightPoint;

        double Eh;
        double Ew;

        public void checkEngage()
        {
            for (int i = 0; i < users.Count; i++)
            {
                Body user = users[i];

                if (!engage &&
                    user.Joints[JointType.HandRight].Position.Y > user.Joints[JointType.Head].Position.Y)
                {
                    holdTime[i]++;
                    Debug.Print("headHoldTime " + i + ": " + holdTime[i]);

                    if (holdTime[i] < 50)
                    {
                        continue;
                    }
                    engage = true;
                    engageUserIndex = i;

                    CoordinateConverter.Ex = user.Joints[JointType.HandRight].Position.X;
                    CoordinateConverter.Ey = user.Joints[JointType.HandRight].Position.Y;
                    CoordinateConverter.Ez = user.Joints[JointType.HandRight].Position.Z;
                    
                    Eh = CoordinateConverter.Ez * Math.Tan(Math.PI * (30.0 / 180));
                    Ew = CoordinateConverter.Ez * Math.Tan(Math.PI * (35.0 / 180));

                    for (int j = 0; j < users.Count; j++)
                    {
                        holdTime[j] = 0;
                    }
                    Debug.Print("Engage " + CoordinateConverter.Ex + ", " + 
                        CoordinateConverter.Ey + ", " + CoordinateConverter.Ez);

                    break;
                }

            }

            if (engage && users[engageUserIndex].Joints[JointType.HandRight].Position.Y <
                    users[engageUserIndex].Joints[JointType.SpineBase].Position.Y)
            {
                holdTime[engageUserIndex]++;
                Debug.Print("spineholdTime " + holdTime[engageUserIndex]);
                if (holdTime[engageUserIndex] < 100)
                {
                    return;
                }
                engage = false;
                engageUserIndex = -1;
                Debug.Print("Not Engage");

                for (int j = 0; j < users.Count; j++)
                {
                    holdTime[j] = 0;
                }
            }

            
        }

        public CameraSpacePoint GetHandRightPoint()
        {
            handRightPoint.X = users[engageUserIndex].Joints[JointType.HandRight].Position.X;
            handRightPoint.Y = users[engageUserIndex].Joints[JointType.HandRight].Position.Y;
            handRightPoint.Z = users[engageUserIndex].Joints[JointType.HandRight].Position.Z;

            return handRightPoint;
        }

        public Body getEngager()
        {
            return users[engageUserIndex];
        }
    }
}
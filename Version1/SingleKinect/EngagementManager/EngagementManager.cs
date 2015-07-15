﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Kinect;
using SingleKinect.MyUtilities;

namespace SingleKinect.EngagementManager
{
    public class EngagementManager
    {
        private readonly int[] holdTime = new int[6];
        private double Eh;
        private bool engage;
        private int engageUserIndex = -1;
        private double Ew;
        public IList<Body> users = new List<Body>();

        public bool IsEngage
        {
            get
            {
                checkEngage();
                return engage;
            }
        }

        public Body Engager => users[engageUserIndex];

        private void checkEngage()
        {
            for (var i = 0; i < users.Count; i++)
            {
                var user = users[i];

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

                    Eh = CoordinateConverter.Ez*Math.Tan(Math.PI*(30.0/180));
                    Ew = CoordinateConverter.Ez*Math.Tan(Math.PI*(35.0/180));

                    for (var j = 0; j < users.Count; j++)
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

                for (var j = 0; j < users.Count; j++)
                {
                    holdTime[j] = 0;
                }
            }
        }
    }
}
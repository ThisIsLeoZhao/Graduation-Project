using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Kinect;

namespace SingleKinect.EngagementManage
{
    public class EngagementManager
    {
        private bool engage;
        private ulong engageUserID = 0;

        private double Ew;
        private double Eh;

        public Dictionary<ulong, Body> users = new Dictionary<ulong, Body>();
        public Dictionary<ulong, int> holdTime = new Dictionary<ulong, int>();

        public bool HasEngaged
        {
            get
            {
                Debug.Print("users {0}", users.Count);
//                foreach (var VARIABLE in holdTime)
//                {
//                    Debug.Print("holdtime {0}, {1}", VARIABLE.Key, VARIABLE.Value);
//                }
                checkEngage();
                return engage;
            }
            set { engage = value; }
        }

        public bool DisablingEngagement { get; set; }

        public Body Engager
        {
            get
            {
                try
                {
                    return users[engageUserID];
                }
                catch (KeyNotFoundException)
                {
                    Debug.Print("Engager Leaving");
                    users.Remove(engageUserID);
                    engageUserID = 0;
                    HasEngaged = false;

                    return null;
                }
            }
        } 

        private void checkEngage()
        {
            List<ulong> userKeys = new List<ulong>(users.Keys);
            foreach (var key in userKeys)
            {
                if (!users[key].IsTracked)
                {
                    users.Remove(key);
                    holdTime.Remove(key);
                }
            }

            foreach (var userTuple in users)
            {
                var user = userTuple.Value;
                
                if (!engage &&
                    user.Joints[JointType.HandRight].Position.Y > user.Joints[JointType.Head].Position.Y)
                {
                    //Debug.Print("user {0}, {1}", userTuple.Key, userTuple.Value);
                    holdTime[userTuple.Key] = holdTime[userTuple.Key] + 1;
                    Debug.Print("headHoldTime " + userTuple.Key + ": " + holdTime[userTuple.Key]);

                    if (holdTime[userTuple.Key] < 50)
                    {
                        continue;
                    }
                    engage = true;
                    engageUserID = userTuple.Key;

                    //                    CoordinateConverter.Ex = user.Joints[JointType.HandRight].Position.X;
                    //                    CoordinateConverter.Ey = user.Joints[JointType.HandRight].Position.Y;
                    //                    CoordinateConverter.Ez = user.Joints[JointType.HandRight].Position.Z;
                    //
                    //                    Eh = CoordinateConverter.Ez*Math.Tan(Math.PI*(30.0/180));
                    //                    Ew = CoordinateConverter.Ez*Math.Tan(Math.PI*(35.0/180));

                    List<ulong> keys = new List<ulong>(holdTime.Keys);
                    foreach (var key in keys)
                    {
                        holdTime[key] = 0;
                    }
                    //                    Debug.Print("Engage " + CoordinateConverter.Ex + ", " +
                    //                                CoordinateConverter.Ey + ", " + CoordinateConverter.Ez);
                    Debug.Print("Engage ");
                    break;
                }
            }

            if (engage)
            {
                if (Engager == null)
                {
                    return;
                }

                if (Engager.Joints[JointType.HandRight].Position.Y <
                    Engager.Joints[JointType.SpineBase].Position.Y + 0.1)
                {
                    DisablingEngagement = true;
                    holdTime[engageUserID] += 1;
                    Debug.Print("spineholdTime " + holdTime[engageUserID]);
                    if (holdTime[engageUserID] < 100)
                    {
                        return;
                    }
                    engage = false;
                    engageUserID = 0;
                    Debug.Print("Not Engage");

                    List<ulong> keys = new List<ulong>(holdTime.Keys);
                    foreach (var key in keys)
                    {
                        holdTime[key] = 0;
                    }
                }
                else
                {
                    List<ulong> keys = new List<ulong>(holdTime.Keys);
                    foreach (var key in keys)
                    {
                        holdTime[key] = 0;
                    }
                    DisablingEngagement = false;
                }
            }
        }
    }
}
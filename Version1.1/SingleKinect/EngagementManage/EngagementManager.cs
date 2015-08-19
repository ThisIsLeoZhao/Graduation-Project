using System;
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

        public Dictionary<ulong, MyHuman> users = new Dictionary<ulong, MyHuman>();
        public Dictionary<ulong, int> holdTime = new Dictionary<ulong, int>();

        public bool HasEngaged
        {
            get
            {
                Debug.Print("users {0}", users.Count);

                checkEngage();
                return engage;
            }
            set { engage = value; }
        }

        public bool DisablingEngagement { get; set; }

        public MyHuman Engager
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
                if (!users[key].body.IsTracked)
                {
                    users.Remove(key);
                    holdTime.Remove(key);
                }
            }

            foreach (var userTuple in users)
            {
                var user = userTuple.Value;
                
                if (!engage &&
                    user.body.Joints[JointType.HandRight].Position.Y > user.body.Joints[JointType.Head].Position.Y)
                {
                    if (Math.Abs(user.headYaw) > 40)
                    {
                        Debug.Print("user {0} disabled by head yaw {1}", user.body.TrackingId, user.headYaw);
                        holdTime[userTuple.Key] = 0;
                        continue;
                    }
                    Debug.Print("user {0} pass by head yaw {1}", user.body.TrackingId, user.headYaw);

                    //Debug.Print("user {0}, {1}", userTuple.Key, userTuple.Value);
                    holdTime[userTuple.Key] = holdTime[userTuple.Key] + 1;
                    Debug.Print("headHoldTime " + userTuple.Key + ": " + holdTime[userTuple.Key]);

                    if (holdTime[userTuple.Key] < 50)
                    {
                        continue;
                    }
                    engage = true;
                    engageUserID = userTuple.Key;
                    

                    List<ulong> keys = new List<ulong>(holdTime.Keys);
                    foreach (var key in keys)
                    {
                        holdTime[key] = 0;
                    }

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

                if (Engager.body.Joints[JointType.HandRight].Position.Y <
                    Engager.body.Joints[JointType.SpineBase].Position.Y + 0.1)
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
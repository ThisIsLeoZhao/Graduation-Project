using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Kinect;
using SingleKinect.EngagerTrack;

namespace SingleKinect.EngagementManage
{
    public class EngagementManager
    {
        private const int ENGAGE_BAR = 40;
        private const int DISENGAGE_BAR = 80;
        private bool engage;
        private ulong engageUserID = 0;

        public Dictionary<ulong, MyHuman> users = new Dictionary<ulong, MyHuman>();
        public Dictionary<ulong, int> holdTime = new Dictionary<ulong, int>();

        public EngagerTracker eTracker = EngagerTracker.Instance;

        public bool HasEngaged
        {
            get
            {
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
            clearUntrackedUser();

            foreach (var userTuple in users)
            {
                var user = userTuple.Value;
                
                if (!engage &&
                    user.body.Joints[JointType.HandRight].Position.Y > user.body.Joints[JointType.Head].Position.Y)
                {
                    if (Math.Abs(user.headYaw) > 30)
                    {
                        Debug.Print("user {0} continued by head yaw {1}", user.body.TrackingId, user.headYaw);
                        //holdTime[userTuple.Key] = 0;
                        continue;
                    }
                    Debug.Print("user {0} pass by head yaw {1}", user.body.TrackingId, user.headYaw);

                    //Debug.Print("user {0}, {1}", userTuple.Key, userTuple.Value);
                    holdTime[userTuple.Key] += 1;
                    Debug.Print("headHoldTime " + userTuple.Key + ": " + holdTime[userTuple.Key]);

                    if (holdTime[userTuple.Key] < ENGAGE_BAR)
                    {
                        continue;
                    }

                    alterEngageState(true, userTuple.Key);
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

                if (Math.Abs(Engager.headYaw) > 30)
                {
                    //DisablingEngagement = true;
                    holdTime[engageUserID] += 1;
                    Debug.Print("Disengage time: " + holdTime[engageUserID]);

                }
                if (Engager.body.Joints[JointType.HandRight].Position.Y <
                    Engager.body.Joints[JointType.SpineBase].Position.Y + 0.1 || Math.Abs(Engager.headYaw) > 30)
                {
                    DisablingEngagement = true;
                    holdTime[engageUserID] += 1;
                    Debug.Print("Disengage time: " + holdTime[engageUserID]);
                    if (holdTime[engageUserID] < DISENGAGE_BAR)
                    {
                        return;
                    }

                    alterEngageState(false, 0);
                    Debug.Print("Disengage");
                }
                else
                {
                    resetHoldtime();
                    DisablingEngagement = false;
                }
            }
        }

        private void alterEngageState(bool engageState, ulong key)
        {
            engage = engageState;
            engageUserID = key;

            resetHoldtime();
        }

        private void resetHoldtime()
        {
            List<ulong> keys = new List<ulong>(holdTime.Keys);
            foreach (var key in keys)
            {
                holdTime[key] = 0;
            }
        }

        private void clearUntrackedUser()
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
        }

        public void setTracker()
        {
            eTracker.Engager = Engager.body;
        }

        public void getTrackerInfo(out Body body, out HandState left, out HandState right, out int yaw, out int pitch,
            out int roll)
        {
            body = eTracker.Engager;
            left = eTracker.LeftState;
            right = eTracker.RightState;
            yaw = eTracker.Yaw;
            pitch = eTracker.Pitch;
            roll = eTracker.Roll;
        }

        public void setTrackerFaceOrientation(ulong trackingId)
        {
            eTracker.Pitch = users[trackingId].headPitch;
            eTracker.Roll = users[trackingId].headRoll;
            eTracker.Yaw = users[trackingId].headYaw;
        }
    }
}
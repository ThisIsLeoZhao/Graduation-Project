using System;
using System.Diagnostics;
using Microsoft.Kinect;
using SingleKinect.EngagementManage;
using SingleKinect.EngagerTrack;
using SingleKinect.MyDataStructures;

namespace SingleKinect.GestureRecognise
{
    public class GestureAnalyser
    {
        private static GestureAnalyser instance;
        public EngagerTracker tracker { get; set; }

        private GestureAnalyser() { }

        public static GestureAnalyser Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GestureAnalyser();
                }
                return instance;
            }
        }

        private bool scaleBaseSet;
        private Joint scaleLeftBase;
        private Joint scaleRightBase;

        public static double SCALE_TRIGGER;
        public static double SCALE_SENSITIVITY;
        public static double BLIND_RADIUS;

        public bool tryScale()
        {
            if (!scaleBaseSet)
            {
                scaleRightBase = tracker.curHandRightPoint;
                scaleLeftBase = tracker.curHandLeftPoint;

                scaleBaseSet = true;
                return false;
            }

            if (withinRange(scaleRightBase, tracker.curHandRightPoint, SCALE_TRIGGER) &&
                    withinRange(scaleLeftBase, tracker.curHandLeftPoint, SCALE_TRIGGER))
            {
                return false;
            }

            return true;
        }

        public void setIncrementRect()
        {
            var incrementRect = new RECT();
            int rightMove = (int)((scaleRightBase.Position.Y - tracker.curHandRightPoint.Position.Y) * SCALE_SENSITIVITY);
            int leftMove = (int)((scaleLeftBase.Position.Y - tracker.curHandLeftPoint.Position.Y) * SCALE_SENSITIVITY);

            incrementRect.Bottom = scaleRightBase.Position.Y + 0.2 < scaleLeftBase.Position.Y ? rightMove : leftMove;
            incrementRect.Top = scaleRightBase.Position.Y + 0.2 < scaleLeftBase.Position.Y ? leftMove : rightMove;

            incrementRect.Right = (int)((tracker.curHandRightPoint.Position.X - scaleRightBase.Position.X) * SCALE_SENSITIVITY);
            incrementRect.Left = (int)((tracker.curHandLeftPoint.Position.X - scaleLeftBase.Position.X) * SCALE_SENSITIVITY);

            tracker.IncrementRect = incrementRect;
        }

        public void bothHandsOpen()
        {
            scaleBaseSet = false;
        }

        public void setScrollFactor()
        {
            if (Math.Abs(tracker.curHandRightPoint.Position.Y - tracker.preHandRightPoint.Value.Position.Y) >
                Math.Abs(tracker.curHandRightPoint.Position.X - tracker.preHandRightPoint.Value.Position.X))
            {
                tracker.IsVerticalScroll = true;
                tracker.ScrollDis = tracker.VerticalRightMovement;
            }
            else
            {
                tracker.IsVerticalScroll = false;
                tracker.ScrollDis = tracker.HorizontalRightMovement;
            }
        }

        public bool isRealMove()
        {
            if (withinRange(tracker.curHandRightPoint, tracker.preHandRightPoint.Value, BLIND_RADIUS))
            {
                return false;
            }
//            if (tracker.Engager.HandRightConfidence == TrackingConfidence.Low)
//            {
//                return false;
//            }
            return true;
        }

        private bool withinRange(Joint cur, Joint pre, double range)
        {
            if (range < 0)
            {
                range = -range;
            }

            return Math.Sqrt(Math.Pow(cur.Position.X - pre.Position.X, 2.0) +
                             Math.Pow(cur.Position.Y - pre.Position.Y, 2.0)) < range;
        }
    }
}
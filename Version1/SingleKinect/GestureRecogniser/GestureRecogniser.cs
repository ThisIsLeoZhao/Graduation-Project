using System;
using System.Diagnostics;
using Microsoft.Kinect;
using SingleKinect.EngagementManager;
using SingleKinect.Manipulator;
using SingleKinect.Manipulator.MyDataStructures;

namespace SingleKinect.GestureRecogniser
{
    public class GestureRecogniser
    {
        private const int MINIMISE_TRIGGER = 50;
        private const float OP_TRIGGER = (float) 0.05;
        public static int SCALE_SENSITIVITY = 50;

        private readonly EngagerTracker tracker;

        private Joint? preHandLeftPoint;
        private Joint? preHandRightPoint;
        private Joint curHandLeftPoint;
        private Joint curHandRightPoint;

        private bool doubleClickReady;
        private bool mouseIsDown;
        private float moveDownDis;

        private bool scaleBaseSet;
        private Joint scaleLeftBase;
        private Joint scaleRightBase;

        private HandState LeftState
        {
            get
            {
                if (Engager.HandLeftConfidence == TrackingConfidence.High)
                {
                    return Engager.HandLeftState;
                }
                return HandState.Open;
            }
        }

        private HandState RightState
        {
            get
            {
                if (Engager.HandRightConfidence == TrackingConfidence.High)
                {
                    return Engager.HandRightState;
                }
                return HandState.Open;
            }
        }

        public GestureRecogniser(EngagerTracker eTracker)
        {
            tracker = eTracker;
        }

        public Body Engager => tracker.Engager;

        public Gestures recognise()
        {
            curHandLeftPoint = Engager.Joints[JointType.HandLeft];
            curHandRightPoint = Engager.Joints[JointType.HandRight];

            if (!preHandRightPoint.HasValue)
            {
                preHandRightPoint = curHandRightPoint;
            }
            if (!preHandLeftPoint.HasValue)
            {
                preHandLeftPoint = curHandLeftPoint;
            }
            
            switch (LeftState)
            {
                case HandState.Open:
                    return leftHandOpen();
                    break;

                case HandState.Closed:
                    return leftHandClosed();
                    break;

                case HandState.Lasso:
                    return leftHandLasso();

                default:
                    return Gestures.Move;
                    break;
            }
            return Gestures.None;
        }

        private Gestures leftHandLasso()
        {
            if (withinRange(curHandRightPoint, preHandRightPoint.Value, OP_TRIGGER))
            {
                return Gestures.None;
            }

            if (Math.Abs(curHandRightPoint.Position.Y - preHandRightPoint.Value.Position.Y) >
                Math.Abs(curHandRightPoint.Position.X - preHandRightPoint.Value.Position.X))
            {
                tracker.IsVerticalScroll = true;
                tracker.ScrollDis = curHandRightPoint.Position.Y - preHandRightPoint.Value.Position.Y - OP_TRIGGER;
            }
            else
            {
                tracker.IsVerticalScroll = false;
                tracker.ScrollDis = curHandRightPoint.Position.X - preHandRightPoint.Value.Position.X - OP_TRIGGER;

            }

            preHandRightPoint = curHandRightPoint;

            return Gestures.Scroll;
        }

        private Gestures leftHandClosed()
        {
            if (RightState == HandState.Closed)
            {
                if (!scaleBaseSet)
                {
                    scaleRightBase = curHandRightPoint;
                    scaleLeftBase = curHandLeftPoint;

                    scaleBaseSet = true;
                    return Gestures.None;
                }

                if (withinRange(scaleRightBase, curHandRightPoint, OP_TRIGGER) &&
                    withinRange(scaleLeftBase, curHandLeftPoint, OP_TRIGGER))
                {
                    return Gestures.None;
                }

                //Left hand controls left and lower edge. Right hand for the rest edges.
                var incrementRect = new RECT();
                int rightMove = (int)((scaleRightBase.Position.Y - curHandRightPoint.Position.Y) * SCALE_SENSITIVITY);
                int leftMove = (int)((scaleLeftBase.Position.Y - curHandLeftPoint.Position.Y) * SCALE_SENSITIVITY);

                incrementRect.Bottom = scaleRightBase.Position.Y < scaleLeftBase.Position.Y ? rightMove : leftMove;
                incrementRect.Top = scaleRightBase.Position.Y < scaleLeftBase.Position.Y ? leftMove : rightMove;
                
                incrementRect.Right = (int) ((curHandRightPoint.Position.X - scaleRightBase.Position.X)*SCALE_SENSITIVITY);
                incrementRect.Left = (int) ((curHandLeftPoint.Position.X - scaleLeftBase.Position.X)*SCALE_SENSITIVITY);

                tracker.IncrementRect = incrementRect;

                scaleBaseSet = false;

                return Gestures.Scale;
            }

            if (!doubleClickReady)
            {
                doubleClickReady = true;
            }

            return Gestures.None;
        }

        private Gestures leftHandOpen()
        {
            preHandLeftPoint = null;

            if (doubleClickReady)
            {
                doubleClickReady = false;

                return Gestures.DoubleClick;
            }

            if (RightState == HandState.Open)
            {
                preHandRightPoint = null;
                if (mouseIsDown)
                {
                    Debug.Print("IsPressed: {0}", mouseIsDown);
                    mouseIsDown = false;
                    return Gestures.MouseUp;
                }
                return Gestures.Move;
            }

            if (RightState == HandState.Closed)
            {
                if (mouseIsDown)
                {
                    //drag
                    return Gestures.Move;
                }

                mouseIsDown = true;
                return Gestures.MouseDown;
            }
            return Gestures.None;
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
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
        public static double OP_TRIGGER;
        public static double CURSOR_SENSITIVITY;
        public static int SCALE_SENSITIVITY;

        private readonly EngagerTracker tracker;
        public Body Engager => tracker.Engager;

        private bool doubleClickReady;
        private bool mouseIsDown;
        private bool bothHandsClosedFinished = true;

        private bool scaleBaseSet;
        private Joint scaleLeftBase;
        private Joint scaleRightBase;

        
        public GestureRecogniser(EngagerTracker eTracker)
        {
            tracker = eTracker;
        }

        public Gestures recognise()
        {
            switch (tracker.LeftState)
            {
                case HandState.Open:
                    return leftHandOpen();

                case HandState.Closed:
                    return leftHandClosed();

                case HandState.Lasso:
                    return leftHandLasso();

                default:
                    if (withinRange(tracker.curHandRightPoint, tracker.preHandRightPoint.Value, CURSOR_SENSITIVITY))
                    {
                        return Gestures.None;
                    }
                    return Gestures.Move;
            }
        }

        private Gestures leftHandOpen()
        {
            if (doubleClickReady && bothHandsClosedFinished)
            {
                doubleClickReady = false;

                return Gestures.DoubleClick;
            }

            if (tracker.RightState == HandState.Open)
            {
                bothHandsClosedFinished = true;
                scaleBaseSet = false;

                //preHandRightPoint = null;
                if (mouseIsDown && bothHandsClosedFinished)
                {
                    Debug.Print("IsPressed: {0}", mouseIsDown);
                    mouseIsDown = false;
                    return Gestures.MouseUp;
                }
                
                if (withinRange(tracker.curHandRightPoint, tracker.preHandRightPoint.Value, CURSOR_SENSITIVITY))
                {
                    return Gestures.None;
                }
                return Gestures.Move;
            }

            if (tracker.RightState == HandState.Closed)
            {
                if (mouseIsDown)
                {
                    //drag
                    if (withinRange(tracker.curHandRightPoint, tracker.preHandRightPoint.Value, CURSOR_SENSITIVITY))
                    {
                        return Gestures.None;
                    }
                    return Gestures.Move;
                }

                mouseIsDown = true;
                return Gestures.MouseDown;
            }
            return Gestures.None;
        }

        private Gestures leftHandClosed()
        {  
//            Debug.Print("handCloseFinished {0} \n doubleClickReady {1} \n" +
//                        "clickReady {2}", bothHandsClosedFinished, doubleClickReady, mouseIsDown);
            if (tracker.RightState == HandState.Closed)
            {
                bothHandsClosedFinished = false;
                doubleClickReady = false;
                mouseIsDown = false;

                if (!scaleBaseSet)
                {
                    scaleRightBase = tracker.curHandRightPoint;
                    scaleLeftBase = tracker.curHandLeftPoint;

                    scaleBaseSet = true;
                    return Gestures.None;
                }

                if (withinRange(scaleRightBase, tracker.curHandRightPoint, OP_TRIGGER) &&
                    withinRange(scaleLeftBase, tracker.curHandLeftPoint, OP_TRIGGER))
                {
                    return Gestures.None;
                }

                Debug.Print("act");
                //Left hand controls left and lower edge. Right hand for the rest edges.
                var incrementRect = new RECT();
                int rightMove = (int)((scaleRightBase.Position.Y - tracker.curHandRightPoint.Position.Y) * SCALE_SENSITIVITY);
                int leftMove = (int)((scaleLeftBase.Position.Y - tracker.curHandLeftPoint.Position.Y) * SCALE_SENSITIVITY);

                incrementRect.Bottom = scaleRightBase.Position.Y < scaleLeftBase.Position.Y ? rightMove : leftMove;
                incrementRect.Top = scaleRightBase.Position.Y < scaleLeftBase.Position.Y ? leftMove : rightMove;

                incrementRect.Right = (int)((tracker.curHandRightPoint.Position.X - scaleRightBase.Position.X) * SCALE_SENSITIVITY);
                incrementRect.Left = (int)((tracker.curHandLeftPoint.Position.X - scaleLeftBase.Position.X) * SCALE_SENSITIVITY);

                tracker.IncrementRect = incrementRect;
                return Gestures.Scale;
            }

            if (!doubleClickReady && bothHandsClosedFinished)
            {
                doubleClickReady = true;
            }

            return Gestures.None;
        }

        

        private Gestures leftHandLasso()
        {
            if (withinRange(tracker.curHandRightPoint, tracker.preHandRightPoint.Value, OP_TRIGGER))
            {
                return Gestures.None;
            }

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
            
            return Gestures.Scroll;
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
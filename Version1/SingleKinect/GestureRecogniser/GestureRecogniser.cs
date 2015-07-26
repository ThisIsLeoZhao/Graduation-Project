using System;
using System.Diagnostics;
using Microsoft.Kinect;
using SingleKinect.EngagementManager;
using SingleKinect.MyUtilities;

namespace SingleKinect.GestureRecogniser
{
    public class GestureRecogniser
    {
        private const int MINIMISE_TRIGGER = 100;
        private const double SCALE_TRIGGER = 0.02;
        private readonly EngagerTracker tracker;
        private Joint curHandLeftPoint;
        private Joint curHandRightPoint;
        private bool isPressed;
        private float moveDownDis;
        private Joint? preHandRightPoint;
        private Joint? preHandLeftPoint;

        public GestureRecogniser(EngagerTracker eTracker)
        {
            tracker = eTracker;
        }

        public Body Engager => tracker.Engager;

        public Gestures recognise()
        {
            if (Engager.HandRightState == HandState.Open)
            {
                preHandRightPoint = null;
                if (isPressed)
                {
                    Debug.Print("IsPressed: {0}", isPressed);
                    isPressed = false;
                    return Gestures.MouseUp;
                }
                return Gestures.Move;
            }

            if (Engager.HandLeftState == HandState.Open)
            {
                preHandLeftPoint = null;
            }

            if (Engager.HandLeftState == HandState.Closed)
            {
                if (Engager.HandRightState == HandState.Closed)
                {
                    curHandRightPoint = Engager.Joints[JointType.HandRight];
                    if (!preHandRightPoint.HasValue)
                    {
                        preHandRightPoint = curHandRightPoint;
                    }

                    //return monitorBothHandsClosed();
                }

                curHandLeftPoint = Engager.Joints[JointType.HandLeft];

                if (!preHandLeftPoint.HasValue)
                {
                    preHandLeftPoint = curHandLeftPoint;
                }
                else if (withinRange(curHandLeftPoint, preHandLeftPoint.Value, 1))
                {
                    Debug.Print("Ready to scale!");
                    tracker.ScaleDepth += curHandLeftPoint.Position.Z - preHandLeftPoint.Value.Position.Z;
                    preHandLeftPoint = curHandLeftPoint;
                    //return Gestures.Scale;
                    if (Math.Abs(tracker.ScaleDepth) > SCALE_TRIGGER)
                    {
                        return Gestures.Scale;
                    }
                }
            }
            else if (Engager.HandRightState == HandState.Closed)
            {
                if (isPressed)
                {
                    //drag
                    return Gestures.Move;
                }

                isPressed = true;
                return Gestures.MouseDown;

            }
            return Gestures.None;
        }

        private Gestures monitorBothHandsClosed()
        {
            if (curHandRightPoint.Position.Y < preHandRightPoint.Value.Position.Y &&
                withinRange(curHandLeftPoint, curHandRightPoint, 0.4))
            {
                moveDownDis += CoordinateConverter.JointToDepthSpace(curHandRightPoint).Y -
                               CoordinateConverter.JointToDepthSpace(preHandRightPoint.Value).Y;

                Debug.Print("moveDownDis: {0}", moveDownDis);
                if (moveDownDis > MINIMISE_TRIGGER)
                {
                    preHandRightPoint = null;
                    moveDownDis = 0;

                    return Gestures.Minimise;
                }

                preHandRightPoint = curHandRightPoint;
            }
            else
            {
                moveDownDis /= 2;
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
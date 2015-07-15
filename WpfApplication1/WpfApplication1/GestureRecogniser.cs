using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace WpfApplication1
{
    public enum Gestures
    {
        Minimise,
        MouseDown,
        MouseUp,
        Scale,
        Move,
        None
    }

    class GestureRecogniser
    {
        private const int MINIMISE_TRIGGER = 100;
        private const double SCALE_TRIGGER = 0.05;

        private EngagerTracker tracker;

        private Joint? preHandRightPoint = null;
        private Joint curHandRightPoint;
        private Joint curHandLeftPoint;

        private float moveDownDis = 0;

        private bool isPressed;

        public Body Engager => tracker.Engager;

        public GestureRecogniser(EngagerTracker eTracker)
        {
            tracker = eTracker;
        }

        public Gestures recognise()
        {
            if (Engager.HandRightState == HandState.Open)
            {
                if (isPressed)
                {
                    Debug.Print("IsPressed: {0}", isPressed);
                    isPressed = false;
                    return Gestures.MouseUp;
                }
                return Gestures.Move;
            }

            if (Engager.HandRightState == HandState.Closed)
            {
                //Debug.Print("Recognise right {0}", Engager.HandRightState);
                curHandRightPoint = Engager.Joints[JointType.HandRight];
                curHandLeftPoint = Engager.Joints[JointType.HandLeft];

                if (!preHandRightPoint.HasValue)
                {
                    preHandRightPoint = curHandRightPoint;
                }
                if (withinRange(curHandRightPoint.Position.X, preHandRightPoint.Value.Position.X, 0.1)
                    && withinRange(curHandRightPoint.Position.Y, preHandRightPoint.Value.Position.Y, 0.1))
                {
                    tracker.ScaleDepth = curHandRightPoint.Position.Z - preHandRightPoint.Value.Position.Z;

                    if (Math.Abs(tracker.ScaleDepth) > SCALE_TRIGGER)
                    {
                        return Gestures.Scale;
                    }
                }

                if (Engager.HandLeftState == HandState.Closed)
                {
                    return monitorBothHandsClosed();
                }

                if (!isPressed)
                {
                    isPressed = true;
                    return Gestures.MouseDown;
                }


                return Gestures.Move;

            }


            return Gestures.None;
        }

        private Gestures monitorBothHandsClosed()
        {
            if (curHandRightPoint.Position.Y < preHandRightPoint.Value.Position.Y &&
                (int)curHandLeftPoint.Position.Y == (int)curHandRightPoint.Position.Y)
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
                moveDownDis = 0;
            }


            return Gestures.None;
            
        }

        private bool withinRange(float n1, float n2, double range)
        {
            if (range < 0)
            {
                range = -range;
            }
            
            return n1 > n2 - range || n1 < n2 + range;
        }

    }
}

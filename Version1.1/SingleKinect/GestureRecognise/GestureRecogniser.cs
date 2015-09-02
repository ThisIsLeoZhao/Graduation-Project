using System;
using System.Diagnostics;
using Microsoft.Kinect;
using SingleKinect.EngagementManage;
using SingleKinect.EngagerTrack;
using SingleKinect.MyDataStructures;

namespace SingleKinect.GestureRecognise
{
    public class GestureRecogniser
    {
        private readonly EngagerTracker tracker = EngagerTracker.Instance;
        public Body Engager => tracker.Engager;

        private bool doubleClickReady;
        private bool mouseIsDown;
        private bool bothHandsClosedFinished = true;
        

        private readonly GestureAnalyser analyser = GestureAnalyser.Instance;

        public GestureRecogniser()
        {
            //tracker = eTracker;
            analyser.tracker = EngagerTracker.Instance;
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
                    return analyser.isRealMove() ? Gestures.Move : Gestures.None;
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
                analyser.bothHandsOpen();
                
                if (mouseIsDown && bothHandsClosedFinished)
                {
                    Debug.Print("IsPressed: {0}", mouseIsDown);
                    mouseIsDown = false;
                    return Gestures.MouseUp;
                }

                return analyser.isRealMove() ? Gestures.Move : Gestures.None;
            }

            if (tracker.RightState == HandState.Closed)
            {
                if (mouseIsDown)
                {
                    //drag
                    return analyser.isRealMove() ? Gestures.Move : Gestures.None;
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

                if (analyser.tryScale())
                {
                    analyser.setIncrementRect();
                    return Gestures.Scale;
                }

                return Gestures.None;
            }

            if (bothHandsClosedFinished)
            {
                doubleClickReady = true;
            }

            return Gestures.None;
        }

        private Gestures leftHandLasso()
        {
            analyser.setScrollFactor();
            return Gestures.Scroll;
        }
        
    }
}
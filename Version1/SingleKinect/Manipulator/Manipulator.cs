using System.Diagnostics;
using Microsoft.Kinect;
using SingleKinect.EngagementManager;
using SingleKinect.GestureRecogniser;
using SingleKinect.MyUtilities;

namespace SingleKinect.Manipulator
{
    public class Manipulator
    {
//        private float moveDownDistance = 0;
//        private float previousHandPosition;
//        private float currentHandPosition;

        private readonly EngagerTracker tracker;

        public Manipulator()
        {
            tracker = null;
        }

        public Manipulator(EngagerTracker eTracker)
        {
            tracker = eTracker;
        }

        public void minimiseCurrentWindow()
        {
            var currentWindow = MyWindow.GetForegroundWindow();

            MyWindow.ShowWindow(currentWindow, 6);
        }

        public void scaleWindow()
        {
            MyWindow.moveWindow(tracker.IncrementRect);
        }

        public void moveCursor(int x, int y)
        {
            MyCursor.SetCursorPos(x, y);
        }

        public void leftUp(int x, int y)
        {
            Debug.Print("Mouse Up: {0}, {1}", x, y);
            MyCursor.mouse_event(MyCursor.MOUSEEVENTF_LEFTUP, x, y, 0, 0);
        }

        public void leftDown(int x, int y)
        {
            Debug.Print("Mouse Down: {0}, {1}", x, y);
            MyCursor.mouse_event(MyCursor.MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
        }

        public void scrollWindow()
        {
            Debug.Print("scrollDis {0}", tracker.ScrollDis);
            MyWindow.scrollWindow((int) (tracker.ScrollDis * 100), tracker.IsVerticalScroll);
        }

        public void reactGesture(Gestures recognisedGestures)
        {
            if (recognisedGestures != Gestures.None && recognisedGestures != Gestures.Move)
            {
                Debug.Print("Gesture {0}", recognisedGestures);
            }

            var handRightPoint = tracker.Engager.Joints[JointType.HandRight].Position;
            var leftPin = CoordinateConverter.cameraPointToScreen(handRightPoint.X, handRightPoint.Y);

            switch (recognisedGestures)
            {
                case Gestures.MouseDown:
                    leftDown(leftPin.x, leftPin.y);
                    break;

                case Gestures.MouseUp:
                    leftDown(leftPin.x, leftPin.y);
                    leftUp(leftPin.x, leftPin.y);
                    break;

                case Gestures.DoubleClick:
                    leftDown(leftPin.x, leftPin.y);
                    leftUp(leftPin.x, leftPin.y);
                    leftDown(leftPin.x, leftPin.y);
                    leftUp(leftPin.x, leftPin.y);
                    break;

                case Gestures.Move:
                    moveCursor(leftPin.x, leftPin.y);
                    break;

                case Gestures.Scale:
                    scaleWindow();
                    break;

                case Gestures.Scroll:
                    scrollWindow();
                    break;

                default:
                    //leftUp(0, 0);
                    break;
            }
        }
    }
}
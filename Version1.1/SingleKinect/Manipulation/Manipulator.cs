using System.Diagnostics;
using System.Threading;
using SingleKinect.CoordinateConvert;
using SingleKinect.EngagementManage;
using SingleKinect.EngagerTrack;
using SingleKinect.GestureRecognise;
using SingleKinect.Manipulation.InputConstants.Mouse;
using SingleKinect.MyDataStructures;

namespace SingleKinect.Manipulation
{
    public class Manipulator
    {
        private readonly EngagerTracker tracker;
        private GestureRecogniser recogniser;

        public Manipulator()
        {
            tracker = EngagerTracker.Instance;
            recogniser = new GestureRecogniser();
        }

        public void scaleWindow()
        {
            MyWindow.moveWindow(tracker.IncrementRect);
        }

        public void moveCursor()
        {
            MyCursor.moveCursor(tracker.HorizontalRightMovement, tracker.VerticalRightMovement);
        }

        public void leftUp(int x, int y)
        {
            Debug.Print("Mouse Up: {0}, {1}", x, y);
            MyCursor.mouse_event(MOUSEEVENTF.LEFTUP, x, y, 0, 0);
        }

        public void leftDown(int x, int y)
        {
            Debug.Print("Mouse Down: {0}, {1}", x, y);
            MyCursor.mouse_event(MOUSEEVENTF.LEFTDOWN, x, y, 0, 0);
        }

        public void scrollWindow()
        {
            var scrollDis = CoordinateConverter.scrollToScreen(tracker.ScrollDis, tracker.IsVerticalScroll);
            //Debug.Print("scrollDis {0}", scrollDis);

            MyCursor.scrollWindow(scrollDis, tracker.IsVerticalScroll);
        }

        public void reactToTracker()
        {
            Gestures recognisedGestures = recogniser.recognise();
            if (recognisedGestures != Gestures.None && recognisedGestures != Gestures.Move)
            {
                Debug.Print("Gesture {0}", recognisedGestures);
            }

            //var handRightPoint = tracker.Engager.Joints[JointType.HandRight].Position;
            //var leftPin = CoordinateConverter.cameraPointToScreen(handRightPoint.X, handRightPoint.Y);
            POINT leftPin;
            MyCursor.GetCursorPos(out leftPin);

            switch (recognisedGestures)
            {
                case Gestures.MouseDown:
                    leftDown(leftPin.x, leftPin.y);
                    break;

                case Gestures.MouseUp:
                    //leftDown(leftPin.x, leftPin.y);
                    leftUp(leftPin.x, leftPin.y);
                    break;

                case Gestures.DoubleClick:
                    MyCursor.mouse_event(MOUSEEVENTF.LEFTDOWN | MOUSEEVENTF.LEFTUP, leftPin.x, leftPin.y, 0, 0);
                    Thread.Sleep(150);
                    MyCursor.mouse_event(MOUSEEVENTF.LEFTDOWN | MOUSEEVENTF.LEFTUP, leftPin.x, leftPin.y, 0, 0);
                    break;

                case Gestures.Move:
                    moveCursor();
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
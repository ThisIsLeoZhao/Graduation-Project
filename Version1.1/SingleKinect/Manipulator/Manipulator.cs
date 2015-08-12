using System.Diagnostics;
using System.Threading;
using Microsoft.Kinect;
using SingleKinect.EngagementManager;
using SingleKinect.GestureRecogniser;
using SingleKinect.Manipulator.MyDataStructures;
using SingleKinect.Manipulator.SystemConstants.Mouse;
using SingleKinect.MyUtilities;

namespace SingleKinect.Manipulator
{
    public class Manipulator
    {
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

        public void moveCursor()
        {
            POINT cursor;
            MyCursor.GetCursorPos(out cursor);

            cursor.x += CoordinateConverter.movementToScreen(tracker.HorizontalRightMovement, false);
            cursor.y += CoordinateConverter.movementToScreen(tracker.VerticalRightMovement, true);

            MyCursor.SetCursorPos(cursor.x, cursor.y);
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
            int scrollDis = CoordinateConverter.scrollToScreen(tracker.ScrollDis, tracker.IsVerticalScroll);
            //Debug.Print("scrollDis {0}", scrollDis);

            MyCursor.scrollWindow(scrollDis, tracker.IsVerticalScroll);
        }

        public void reactGesture(Gestures recognisedGestures)
        {
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
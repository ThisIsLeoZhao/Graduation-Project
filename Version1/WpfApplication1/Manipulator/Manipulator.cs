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
            var handRightPoint = tracker.Engager.Joints[JointType.HandRight].Position;

            var handLeftPoint = tracker.Engager.Joints[JointType.HandLeft].Position;
//            int[] rightPin = CoordinateConverter.cameraPointToScreen(handLeftPoint.X, handLeftPoint.Y);

            var dis = (int) (tracker.ScaleDepth*200);
            Debug.Print("dis {0}", dis);

            MyWindow.moveWindow(dis);
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

        public void reactGesture(Gestures recognisedGestures)
        {
            if (recognisedGestures != Gestures.Move)
            {
                Debug.Print("Gesture {0}", recognisedGestures);
            }

            var handRightPoint = tracker.Engager.Joints[JointType.HandRight].Position;
            var leftPin = CoordinateConverter.cameraPointToScreen(handRightPoint.X, handRightPoint.Y);

            switch (recognisedGestures)
            {
                case Gestures.MouseDown:
                    leftDown(leftPin[0], leftPin[1]);
                    break;

                case Gestures.MouseUp:
                    leftDown(leftPin[0], leftPin[1]);
                    leftUp(leftPin[0], leftPin[1]);
                    break;

                case Gestures.Minimise:
                    minimiseCurrentWindow();
                    break;

                case Gestures.Move:
                    moveCursor(leftPin[0], leftPin[1]);
                    break;

                case Gestures.Scale:
                    scaleWindow();
                    break;

                default:
                    //leftUp(0, 0);
                    break;
            }
        }
    }
}
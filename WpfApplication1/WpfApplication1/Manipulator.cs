using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using Microsoft.Kinect;

namespace WpfApplication1
{
    public class Manipulator
    {
//        private float moveDownDistance = 0;
//        private float previousHandPosition;
//        private float currentHandPosition;

        private EngagerTracker tracker;

        public Manipulator(EngagerTracker eTracker)
        {
            tracker = eTracker;
        }

        public void minimiseCurrentWindow()
        {
            
            IntPtr currentWindow = MyWindow.GetForegroundWindow();

            MyWindow.ShowWindow(currentWindow, 6);
        }

        public void scaleWindow()
        {
            CameraSpacePoint handRightPoint = tracker.Engager.Joints[JointType.HandRight].Position;

            CameraSpacePoint handLeftPoint = tracker.Engager.Joints[JointType.HandLeft].Position;
//            int[] rightPin = CoordinateConverter.cameraPointToScreen(handLeftPoint.X, handLeftPoint.Y);

            int dis = (int) (tracker.ScaleDepth *　200);
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

            CameraSpacePoint handRightPoint = tracker.Engager.Joints[JointType.HandRight].Position;
            int[] leftPin = CoordinateConverter.cameraPointToScreen(handRightPoint.X, handRightPoint.Y);

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
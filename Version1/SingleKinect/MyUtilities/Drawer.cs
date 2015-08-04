using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Kinect;
using SingleKinect.EngagementManager;

namespace SingleKinect.MyUtilities
{
    public class Drawer
    {
        private readonly Canvas bodyCanvas;
        private readonly List<Tuple<JointType, JointType>> bones;

        private EngagerTracker tracker;
        public Drawer(Canvas canvas, EngagerTracker tracker)
        {
            // a bone defined as a line between two joints
            bones = new List<Tuple<JointType, JointType>>();

            // Torso
            bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
            bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

            // Right Arm
            bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));

            // Left Arm
            bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));

            // Right Leg
            bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));

            // Left Leg
            bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));

            bodyCanvas = canvas;
            this.tracker = tracker;
        }

        public void showHands(DepthSpacePoint rightHand, DepthSpacePoint leftHand, HandState rightHandState,
            HandState leftHandState)
        {
            Brush openBrush = new SolidColorBrush(Color.FromArgb(100, 120, 5, 250));
            Brush closeBrush = new SolidColorBrush(Color.FromArgb(100, 0, 200, 250));
            Brush lassoBrush = new SolidColorBrush(Color.FromArgb(100, 255, 100, 0));

            var rightBrush = openBrush;
            switch (tracker.RightState)
            {
                case HandState.Closed:
                    rightBrush = closeBrush;
                    break;

                case HandState.Lasso:
                    rightBrush = lassoBrush;
                    break;
            }
            drawCircle(50, rightHand.X, rightHand.Y, rightBrush);

            var leftBrush = openBrush;
            switch (tracker.LeftState)
            {
                case HandState.Closed:
                    leftBrush = closeBrush;
                    break;

                case HandState.Lasso:
                    leftBrush = lassoBrush;
                    break;
            }
            drawCircle(50, leftHand.X, leftHand.Y, leftBrush);
        }

        public void drawBones(Dictionary<JointType, DepthSpacePoint> jointPoints)
        {
            foreach (var bone in bones)
            {
                var end1 = bone.Item1;
                var end2 = bone.Item2;

                if (!jointPoints.ContainsKey(end1) || !jointPoints.ContainsKey(end2))
                {
                    continue;
                }

                var myLine = new Line
                {
                    X1 = jointPoints[end1].X,
                    X2 = jointPoints[end2].X,
                    Y1 = jointPoints[end1].Y,
                    Y2 = jointPoints[end2].Y,
                    Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 255, 255)),
                    StrokeThickness = 2
                };

                bodyCanvas.Children.Add(myLine);
            }
        }

        public void drawCircle(int radius, float X, float Y, Brush color)
        {
            var leftHandEllipse = new Ellipse
            {
                Height = radius,
                Width = radius,
                Fill = color
            };
            bodyCanvas.Children.Add(leftHandEllipse);
            Canvas.SetLeft(leftHandEllipse, X - radius/2);
            Canvas.SetTop(leftHandEllipse, Y - radius/2);
        }

        public void drawSkeleton(Body body, Dictionary<JointType, DepthSpacePoint> joints)
        {
            drawBones(joints);

            foreach (var joint in joints.Values)
            {
                drawCircle(10, joint.X, joint.Y, new SolidColorBrush(Color.FromArgb(255, 100, 255, 100)));
            }

            showHands(joints[JointType.HandRight], joints[JointType.HandLeft],
                body.HandRightState, body.HandLeftState);
        }
    }
}
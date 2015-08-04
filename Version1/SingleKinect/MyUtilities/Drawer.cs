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
        public Canvas CurrentCanvas { get; set; }
        private readonly List<Tuple<JointType, JointType>> bones;

        public Drawer()
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
            
        }

        //Draw all people
        public void drawSkeleton(Body body)
        {
            var joints = CoordinateConverter.convertJointsToDSPoints(body.Joints);

            drawBones(joints);

            foreach (var joint in joints.Values)
            {
                drawCircle(10, joint.X, joint.Y, new SolidColorBrush(Color.FromArgb(255, 100, 255, 100)));
            }

            showHands(joints[JointType.HandRight], joints[JointType.HandLeft],
                body.HandRightState, body.HandLeftState);
        }

        //Draw engager
        public void drawSkeleton(EngagerTracker tracker)
        {
            var joints = CoordinateConverter.convertJointsToDSPoints(tracker.Engager.Joints);

            drawBones(joints);

            foreach (var joint in joints.Values)
            {
                drawCircle(10, joint.X, joint.Y, new SolidColorBrush(Color.FromArgb(255, 100, 255, 100)));
            }

            showHands(joints[JointType.HandRight], joints[JointType.HandLeft],
                tracker.RightState, tracker.LeftState);


            MainWindow.labels[0].Content = "HandLeftState: " + tracker.LeftState;
            MainWindow.labels[1].Content = "HandRightState: " + tracker.RightState;
        }

        private void showHands(DepthSpacePoint rightHand, DepthSpacePoint leftHand, HandState rightHandState,
            HandState leftHandState)
        {
            var rightBrush = decideHandBrush(rightHandState);
            drawCircle(50, rightHand.X, rightHand.Y, rightBrush);

            var leftBrush = decideHandBrush(leftHandState);
            drawCircle(50, leftHand.X, leftHand.Y, leftBrush);

        }

        private void drawBones(Dictionary<JointType, DepthSpacePoint> jointPoints)
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

                CurrentCanvas.Children.Add(myLine);
            }
        }

        private void drawCircle(int radius, float X, float Y, Brush color)
        {
            var leftHandEllipse = new Ellipse
            {
                Height = radius,
                Width = radius,
                Fill = color
            };
            CurrentCanvas.Children.Add(leftHandEllipse);
            Canvas.SetLeft(leftHandEllipse, X - radius/2);
            Canvas.SetTop(leftHandEllipse, Y - radius/2);
        }

        private Brush decideHandBrush(HandState handState)
        {
            Brush openBrush = new SolidColorBrush(Color.FromArgb(100, 120, 5, 250));
            Brush closeBrush = new SolidColorBrush(Color.FromArgb(100, 0, 200, 250));
            Brush lassoBrush = new SolidColorBrush(Color.FromArgb(100, 255, 100, 0));
            Brush unknownBrush = new SolidColorBrush(Color.FromArgb(100, 50, 50, 50));
            Brush notTrackedBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

            switch (handState)
            {
                case HandState.Closed:
                    return closeBrush;

                case HandState.Lasso:
                    return lassoBrush;

                case HandState.Open:
                    return openBrush;

                case HandState.NotTracked:
                    return notTrackedBrush;

            }
            return unknownBrush;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace WpfApplication1
{
    public class Drawer
    {
        private readonly List<Tuple<JointType, JointType>> bones; 
        public Drawer()
        {
            // a bone defined as a line between two joints
            this.bones = new List<Tuple<JointType, JointType>>();

            // Torso
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

            // Right Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));

            // Left Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));

            // Right Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));

            // Left Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));
        }
        

        public void showHands(DepthSpacePoint rightHand, DepthSpacePoint leftHand, HandState rightHandState, HandState leftHandState, Canvas bodyCanvas)
        {
            Brush openBrush = new SolidColorBrush(Color.FromArgb(100, 120, 5, 250));
            Brush closeBrush = new SolidColorBrush(Color.FromArgb(100, 0, 200, 250));

            Brush rightBrush = (rightHandState == HandState.Closed ? closeBrush : openBrush);
            drawCircle(50, rightHand.X, rightHand.Y, rightBrush, bodyCanvas);

            Brush leftBrush = (leftHandState == HandState.Closed ? closeBrush : openBrush);
            drawCircle(50, leftHand.X, leftHand.Y, leftBrush, bodyCanvas);
            
        }

        public void drawBones(Dictionary<JointType, DepthSpacePoint> jointPoints, Canvas bodyCanvas)
        {
            foreach (Tuple<JointType, JointType> bone in bones)
            {
                JointType end1 = bone.Item1;
                JointType end2 = bone.Item2;

                if (!jointPoints.ContainsKey(end1) || !jointPoints.ContainsKey(end2))
                {
                    continue;
                }

                Line myLine = new Line
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

        public void drawCircle(int radius, float X, float Y, Brush color, Canvas bodyCanvas)
        {
            Ellipse leftHandEllipse = new Ellipse()
            {
                Height = radius,
                Width = radius,
                Fill = color
            };
            bodyCanvas.Children.Add(leftHandEllipse);
            Canvas.SetLeft(leftHandEllipse, X - radius / 2);
            Canvas.SetTop(leftHandEllipse, Y - radius / 2);
        }
    }
}
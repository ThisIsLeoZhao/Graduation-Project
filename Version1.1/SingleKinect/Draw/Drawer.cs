using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Kinect;
using SingleKinect.CoordinateConvert;
using SingleKinect.EngagementManage;
using SingleKinect.EngagerTrack;
using SingleKinect.FrameProcess;

namespace SingleKinect.Draw
{
    public class Drawer
    {
        private static Drawer drawer;

        public static Drawer Instance
        {
            get
            {
                if (drawer == null)
                {
                    drawer = new Drawer();
                }
                return drawer;
            }
        }

        public string currentCanvasName;

        private Canvas CurrentCanvas
        {
            get
            {
                if (currentCanvasName == "body")
                {
                    return bodyCanvas;
                }
                if (currentCanvasName == "engager")
                {
                    return engagerCanvas;
                }
                
                Debug.Print("WWWWWWWWWWWWWWRONG CANVAS NAME");
                return null;
                
            }
        }

        private readonly List<Tuple<JointType, JointType>> bones;
        private Label leftLabel;
        private Label rightLabel;
        private Label faceLabel;
        private Canvas bodyCanvas;
        private Canvas engagerCanvas;

        private Drawer()
        {
            // a bone defined as a line between two joints
            bones = new BoneList().Bones;
        }

        public void bindComponents(ComponentsArgs args)
        {
            leftLabel = (Label) args.Components[0];
            rightLabel = (Label) args.Components[1];
            bodyCanvas = (Canvas) args.Components[2];
            engagerCanvas = (Canvas) args.Components[3];
            faceLabel = (Label) args.Components[4];
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
        public void drawSkeleton(Body body, HandState left, HandState right, int yaw, int pitch, int roll)
        {
            var joints = CoordinateConverter.convertJointsToDSPoints(body.Joints);

            drawBones(joints);

            foreach (var joint in joints.Values)
            {
                drawCircle(10, joint.X, joint.Y, new SolidColorBrush(Color.FromArgb(255, 100, 255, 100)));
            }

            showHands(joints[JointType.HandRight], joints[JointType.HandLeft],
                right, left);


            leftLabel.Content = "HandLeftState: " + left;
            rightLabel.Content = "HandRightState: " + right;
            faceLabel.Content = "Face Yaw Pitch Roll: " + yaw + ", " + pitch + ", " + roll;

        }

        public void clear()
        {
            bodyCanvas.Children.Clear();
            engagerCanvas.Children.Clear();
        }

        private void showHands(DepthSpacePoint rightHand, DepthSpacePoint leftHand, HandState rightHandState,
            HandState leftHandState)
        {
            var rightBrush = decideHandBrush(rightHandState);
            drawCircle(50, rightHand.X, rightHand.Y, rightBrush);

            var leftBrush = decideHandBrush(leftHandState);
            drawCircle(50, leftHand.X, leftHand.Y, leftBrush);

        }

        private bool drawBones(Dictionary<JointType, DepthSpacePoint> jointPoints)
        {
            foreach (var bone in bones)
            {
                var end1 = bone.Item1;
                var end2 = bone.Item2;

                if (!jointPoints.ContainsKey(end1) || !jointPoints.ContainsKey(end2))
                {
                    continue;
                }

                try
                {
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
                catch
                {
                    //Debug.Print("aaaaa");
                    return false;
                }
            }
            return true;
        }

        private void drawCircle(int radius, float X, float Y, Brush color)
        {
            var leftHandEllipse = new Ellipse
            {
                Height = radius,
                Width = radius,
                Fill = color
            };

            try
            {
                CurrentCanvas.Children.Add(leftHandEllipse);

                Canvas.SetLeft(leftHandEllipse, X - radius/2);
                Canvas.SetTop(leftHandEllipse, Y - radius/2);
            }
            catch
            {
                Debug.Print("bbbbb");
                return;
            }
            
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
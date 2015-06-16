using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.UI.Xaml.Media.Imaging;
using WindowsPreview.Kinect;

using Windows.UI.Xaml.Shapes;
using Windows.UI;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private KinectSensor sensor;

        private Body[] bodies;
        private BodyFrameReader bfr;
        private List<Tuple<JointType, JointType>> bones;

        public MainPage()
        {
            this.InitializeComponent();

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

            this.Loaded += MainPage_Loaded;
        }
 
        


        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.GetDefault();

            bodies = new Body[6];
            bfr = sensor.BodyFrameSource.OpenReader();

            sensor.Open();
            bfr.FrameArrived += bfr_FrameArrived;

        }

        private void bfr_FrameArrived(BodyFrameReader sender, BodyFrameArrivedEventArgs args)
        {
            using (BodyFrame bodyFrame = args.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    bodyFrame.GetAndRefreshBodyData(bodies);
                    bodyCanvas.Children.Clear();

                    foreach (var body in bodies)
                    {
                        if (!body.IsTracked)
                        {
                            continue;
                        }

                        var jointsDic = body.Joints;
                        Dictionary<JointType, DepthSpacePoint> joints = new Dictionary<JointType, DepthSpacePoint>(); 
                        foreach (var joint in jointsDic)
                        {
                            if (joint.Value.TrackingState == TrackingState.NotTracked)
                            {
                                continue;
                            }

                            DepthSpacePoint point =
                                sensor.CoordinateMapper.MapCameraPointToDepthSpace(joint.Value.Position);

                            joints.Add(joint.Key, point);
                        }

                        DrawBones(joints);

                        foreach (var joint in joints.Values)
                        {
                            Ellipse circle = new Ellipse
                            {
                                Width = 10,
                                Height = 10,

                                Fill = new SolidColorBrush(Color.FromArgb(255, 100, 255, 100))
                            };

                            bodyCanvas.Children.Add(circle);
                            Canvas.SetLeft(circle, joint.X - 5);
                            Canvas.SetTop(circle, joint.Y - 5);
                        }
                    }

                }
            }
        }

        private void DrawBones(Dictionary<JointType, DepthSpacePoint> jointPoints)
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

//        private void irReader_FrameArrived(InfraredFrameReader sender, InfraredFrameArrivedEventArgs args)
//        {
//            using (InfraredFrame irFrame = args.FrameReference.AcquireFrame())
//            {
//                if (irFrame != null)
//                {
//                    irFrame.CopyFrameDataToArray(irData);
//                    for (int i = 0; i < irData.Length; i++)
//                    {
//                        byte intensity = (byte)(irData[i] >> 8);
//
//                        irDataConverted[i * 4] = intensity;
//                        irDataConverted[i * 4 + 1] = intensity;
//                        irDataConverted[i * 4 + 2] = intensity;
//                        irDataConverted[i * 4 + 3] = 255;
//                    }
//                    irDataConverted.CopyTo(irBitmap.PixelBuffer);
//                    irBitmap.Invalidate();
//
//                }
//            }
//        }
    }
}

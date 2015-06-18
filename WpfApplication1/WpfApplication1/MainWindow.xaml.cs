using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using Microsoft.Kinect;
//using Microsoft.Kinect.VisualGestureBuilder;



namespace WpfApplication1
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor sensor;

        private Body[] bodies;
        private BodyFrameReader bfr;
        private List<Tuple<JointType, JointType>> bones;

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("User32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        public MainWindow()
        {
            InitializeComponent();
            //moveCursor();

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

        private void bfr_FrameArrived(object o, BodyFrameArrivedEventArgs args)
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
                                Debug.Print("Hey");
                                continue;
                            }

                            DepthSpacePoint point =
                                sensor.CoordinateMapper.MapCameraPointToDepthSpace(joint.Value.Position);

                            joints.Add(joint.Key, point);
                        }

                        DrawBones(joints);
                        showHands(joints[JointType.HandRight], joints[JointType.HandLeft],
                            body.HandRightState, body.HandLeftState);
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

                        //Debug.Print(joints.ContainsKey(JointType.HandRight).ToString());
                        if (joints.ContainsKey(JointType.HandRight))
                        {
                            int a = (int) (joints[JointType.HandRight].X / 514.0 * SystemParameters.PrimaryScreenWidth);
                            int b = (int)(joints[JointType.HandRight].Y / 424.0 * SystemParameters.PrimaryScreenHeight * 2);

                            Debug.Print(a.ToString());
                            Debug.Print(b.ToString());

//                            Debug.Print(SystemParameters.PrimaryScreenWidth.ToString("N"));
//                            Debug.Print(SystemParameters.PrimaryScreenHeight.ToString("N"));

                            moveCursor(a, b);
                            
                            if (body.HandRightState == HandState.Closed)
                            {
                                Debug.Print("Closed");
                                leftClick(a, b); 
                            }
                        }

                    }

                }
            }
        }

        private void showHands(DepthSpacePoint rightHand, DepthSpacePoint leftHand,
            HandState rightHandState, HandState leftHandState)
        {
            Brush openBrush = new SolidColorBrush(Color.FromArgb(100, 120, 5, 250));
            Brush closeBrush = new SolidColorBrush(Color.FromArgb(100, 0, 200, 250));

            Ellipse rightHandEllipse = new Ellipse()
            {
                Height = 50,
                Width = 50,
                Fill = (rightHandState == HandState.Closed ? closeBrush : openBrush)
            };
            bodyCanvas.Children.Add(rightHandEllipse);
            Canvas.SetLeft(rightHandEllipse, rightHand.X - 25);
            Canvas.SetTop(rightHandEllipse, rightHand.Y - 25);

            Ellipse leftHandEllipse = new Ellipse()
            {
                Height = 50,
                Width = 50,
                Fill = (leftHandState == HandState.Closed ? closeBrush : openBrush)
            };
            bodyCanvas.Children.Add(leftHandEllipse);
            Canvas.SetLeft(leftHandEllipse, leftHand.X - 25);
            Canvas.SetTop(leftHandEllipse, leftHand.Y - 25);
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

        private void moveCursor(int a, int b)
        {
            SetCursorPos(a, b);
        }

        private void leftClick(int a, int b)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, a, b, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, a, b, 0, 0);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Microsoft.Kinect.Wpf.Controls;


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

        private Drawer drawer;
        private Manipulation man;
        private EngagementManager eManager;
        

        

        public MainWindow()
        {
            this.InitializeComponent();
            drawer = new Drawer(bodyCanvas);
            man = new Manipulation();
            eManager = new EngagementManager();

//            KinectRegion.SetKinectRegion(this, kinectRegion);
//
//            this.kinectRegion.KinectSensor = KinectSensor.GetDefault();
//            

            this.Loaded += MainPage_Loaded;
            this.Closing += MainWindow_Closing;
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
                if (bodyFrame == null)
                {
                    return;
                }
                
                bodyFrame.GetAndRefreshBodyData(bodies);
                bodyCanvas.Children.Clear();

                foreach (var body in bodies.Where(body => body.IsTracked))
                {
                    if (!eManager.users.Contains(body))
                    {
                        eManager.users.Add(body);
                    }
                    

                    var joints = CoordinateConverter.convertJointsToPoints(body.Joints, sensor);
                    drawer.drawSkeleton(body, joints);
                }

                eManager.checkEngage();
                if (!eManager.engage)
                {
                    return;
                }

                CameraSpacePoint handRightPoint = eManager.GetHandRightPoint();

                int[] leftPin = CoordinateConverter.cameraPointToScreen(handRightPoint.X, handRightPoint.Y);
                

                Debug.Print("handRightPoint: " + handRightPoint.X + ", " + handRightPoint.Y + ", " + handRightPoint.Z);
                Debug.Print("cursor: " + leftPin[0] + ", " + leftPin[1]);
//
//                                            Debug.Print(SystemParameters.PrimaryScreenWidth.ToString("N"));
//                                            Debug.Print(SystemParameters.PrimaryScreenHeight.ToString("N"));

                MyCursor.moveCursor(leftPin[0], leftPin[1]);

                if (eManager.getEngager().HandRightState == HandState.Closed)
                {
                    Debug.Print("Closed");
                    MyCursor.leftClick(leftPin[0], leftPin[1]);

                    if (eManager.getEngager().HandLeftState == HandState.Closed)
                    {
                        var joints = CoordinateConverter.convertJointsToPoints(eManager.getEngager().Joints, sensor);

                        int[] rightPin = CoordinateConverter.cameraPointToScreen(joints[JointType.HandLeft].X,
                            joints[JointType.HandLeft].Y);

                        int dis = (int)Math.Sqrt(Math.Pow((rightPin[0] - leftPin[0]), 2) + 
                            Math.Pow((rightPin[1] - leftPin[1]), 2));

                        MyWindow.moveWindow(dis);
                    }

                }
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.bfr != null)
            {
                // BodyFrameReader is IDisposable
                this.bfr.Dispose();
                this.bfr = null;
            }

            if (this.sensor != null)
            {
                this.sensor.Close();
                this.sensor = null;
            }
        }

        
    }
}

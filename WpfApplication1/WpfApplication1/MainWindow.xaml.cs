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

        private Drawer drawer;
        private Manipulation man;
        

        private int offset = 0;

        public MainWindow()
        {
            InitializeComponent();
            drawer = new Drawer();
            man = new Manipulation();

            //moveCursor();

            

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
                if (bodyFrame == null)
                {
                    return;
                }
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
                        
                    drawer.drawBones(joints, bodyCanvas);
                    drawer.showHands(joints[JointType.HandRight], joints[JointType.HandLeft],
                        body.HandRightState, body.HandLeftState, bodyCanvas);

                    foreach (var joint in joints.Values)
                    {
                        drawer.drawCircle(10, joint.X, joint.Y, new SolidColorBrush(Color.FromArgb(255, 100, 255, 100)), bodyCanvas);
                    }

                    if (!joints.ContainsKey(JointType.HandRight))
                    {
                        continue;
                    }

                    int a = (int) (joints[JointType.HandRight].X / 514.0 * SystemParameters.PrimaryScreenWidth);
                    int b = (int)(joints[JointType.HandRight].Y / 424.0 * SystemParameters.PrimaryScreenHeight * 2);

//                            Debug.Print(a.ToString());
//                            Debug.Print(b.ToString());

//                            Debug.Print(SystemParameters.PrimaryScreenWidth.ToString("N"));
//                            Debug.Print(SystemParameters.PrimaryScreenHeight.ToString("N"));

                    man.moveCursor(a, b);
                            
                    if (body.HandRightState == HandState.Closed)
                    {
                        Debug.Print("Closed");
                        man.leftClick(a, b);

                        if (body.HandLeftState == HandState.Closed)
                        {
                            int la = (int)(joints[JointType.HandLeft].X / 514.0 * SystemParameters.PrimaryScreenWidth);
                            int lb = (int)(joints[JointType.HandLeft].Y / 424.0 * SystemParameters.PrimaryScreenHeight * 2);
                                    
                            int dis = (int) Math.Sqrt(Math.Pow((la - a), 2) + Math.Pow((lb - b), 2));

                            man.moveWindow(dis);
//                                    if (offset < 500)
//                                    {
//                                        offset +=  50;
//                                    }
                            Debug.Print(offset.ToString());

                        }

                    }
                }
            }
        }
         
        
    }
}

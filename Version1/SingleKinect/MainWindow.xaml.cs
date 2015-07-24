using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Windows;
using Microsoft.Kinect;
using SingleKinect.EngagementManager;
using SingleKinect.MyUtilities;

using System.Net.WebSockets;

//using Microsoft.Kinect.VisualGestureBuilder;

namespace SingleKinect
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Drawer drawer;
        private readonly EngagementManager.EngagementManager eManager;
        private readonly EngagerTracker eTracker;
        private readonly Manipulator.Manipulator man;
        private readonly GestureRecogniser.GestureRecogniser recogniser;
        private BodyFrameReader bfr;
        private Body[] bodies;
        private KinectSensor sensor;

        public IEnumerable<Body> Bodies
        {
            get
            {
                //Debug.Print("11111111111111111111111111111111111111111111111");
                return bodies.Where(body => body.IsTracked);
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            drawer = new Drawer(bodyCanvas);
            eManager = new EngagementManager.EngagementManager();
            eTracker = new EngagerTracker();

            man = new Manipulator.Manipulator(eTracker);
            recogniser = new GestureRecogniser.GestureRecogniser(eTracker);

            Loaded += MainPage_Loaded;
            Closing += MainWindow_Closing;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.GetDefault();

            bodies = new Body[6];
            bfr = sensor.BodyFrameSource.OpenReader();

            sensor.Open();
            CoordinateConverter.Sensor = sensor;

            bfr.FrameArrived += bfr_FrameArrived;
        }

        private void bfr_FrameArrived(object o, BodyFrameArrivedEventArgs args)
        {
            using (var bodyFrame = args.FrameReference.AcquireFrame())
            {
                if (bodyFrame == null)
                {
                    return;
                }

                bodyFrame.GetAndRefreshBodyData(bodies);
                bodyCanvas.Children.Clear();

                foreach (var body in Bodies)
                {
                    if (!eManager.users.Contains(body))
                    {
                        eManager.users.Add(body);
                    }

                    var joints = CoordinateConverter.convertJointsToDSPoints(body.Joints);
                    
                   // joints.
                    drawer.drawSkeleton(body, joints);
                }

                if (!eManager.IsEngage)
                {
                    return;
                }

                eTracker.Engager = eManager.Engager;
                var recognisedGestures = recogniser.recognise();

                man.reactGesture(recognisedGestures);
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (bfr != null)
            {
                // BodyFrameReader is IDisposable
                bfr.Dispose();
                bfr = null;
            }

            if (sensor != null)
            {
                sensor.Close();
                sensor = null;
            }
        }
    }
}
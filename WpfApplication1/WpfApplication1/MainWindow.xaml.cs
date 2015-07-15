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
        private Manipulator man;
        private EngagementManager eManager;
        private EngagerTracker eTracker;
        private GestureRecogniser recogniser;

        public MainWindow()
        {
            InitializeComponent();

            drawer = new Drawer(bodyCanvas);
            eManager = new EngagementManager();
            eTracker = new EngagerTracker();

            man = new Manipulator(eTracker);
            recogniser = new GestureRecogniser(eTracker);

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
                    
                    var joints = CoordinateConverter.convertJointsToDSPoints(body.Joints);
                    drawer.drawSkeleton(body, joints);
                }

                if (!eManager.IsEngage)
                {
                    return;
                }

                eTracker.Engager = eManager.Engager;
                Gestures recognisedGestures = recogniser.recognise();

                man.reactGesture(recognisedGestures);
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

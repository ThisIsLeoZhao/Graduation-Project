using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Microsoft.Kinect;
using SingleKinect.EngagementManager;
using SingleKinect.MyUtilities;
using System.Windows.Controls;

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

        public static Label[] labels = new Label[2];

        public MainWindow()
        {
            InitializeComponent();

            ReadConfiguration.read("../../MyConfiguration.txt");
            
            eTracker = new EngagerTracker();
            eManager = new EngagementManager.EngagementManager();

            drawer = new Drawer();

            man = new Manipulator.Manipulator(eTracker);
            recogniser = new GestureRecogniser.GestureRecogniser(eTracker);

            Loaded += MainPage_Loaded;
            Closing += MainWindow_Closing;
        }

        public IEnumerable<Body> Bodies
        {
            get { return bodies.Where(body => body.IsTracked); }
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.GetDefault();

            bodies = new Body[6];
            bfr = sensor.BodyFrameSource.OpenReader();

            sensor.Open();
            CoordinateConverter.Sensor = sensor;
            
            labels[0] = leftLabel;
            labels[1] = rightLabel;

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
                engagerCanvas.Children.Clear();

                
                foreach (var body in Bodies)
                {
                    if (!eManager.users.Contains(body))
                    {
                        eManager.users.Add(body);
                    }

                    // Multithreading maybe
                    drawer.CurrentCanvas = bodyCanvas;
                    drawer.drawSkeleton(body);
                }

                if (!eManager.IsEngage)
                {
                    return;
                }

                eTracker.Engager = eManager.Engager;

                // Multithreading maybe
                drawer.CurrentCanvas = engagerCanvas;
                eManager.IsEngage = drawer.drawSkeleton(eTracker);

                if (eManager.DisablingEngagement)
                {
                    return;
                }
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
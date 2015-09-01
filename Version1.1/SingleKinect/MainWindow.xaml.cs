using System.ComponentModel;
using System.Windows;
using Microsoft.Kinect;
using SingleKinect.Draw;
using SingleKinect.FrameProcess;
using SingleKinect.MyConfiguration;

//using Microsoft.Kinect.VisualGestureBuilder;

namespace SingleKinect
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Drawer drawer = Drawer.Instance;
        private KinectSensor sensor;
        //private SendData dataSender = SendData.Instance;

        private FrameReader frameReader = FrameReader.Instance;

        public MainWindow()
        {
            InitializeComponent();

            ReadConfiguration.read("../../MyConfiguration/MyConfiguration.txt");

            drawer.bindComponents(new ComponentsArgs(leftLabel, rightLabel, bodyCanvas, engagerCanvas, faceLabel));

            Loaded += MainPage_Loaded;
            Closing += MainWindow_Closing;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.GetDefault();

            frameReader.sensor = sensor;
            
            sensor.Open();

            frameReader.start();
            
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            frameReader.end();
        }
    }
}
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
using SingleKinect.Manipulation.MyDataStructures;
using SingleKinect.Manipulation.SystemConstants.Mouse;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //        [DllImport("user32.dll")]
        //        internal static extern int GetSystemMetrics(SystemMetric smIndex);

        [DllImport("user32.dll")]
        internal static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs,
            [MarshalAs(UnmanagedType.LPArray), InAttribute] INPUT[] pInputs,
            int cbSize);

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Manipulator man = new Manipulator();
            
            

            POINT cursorPoint;
            GetCursorPos(out cursorPoint);
            // Positive value for scroll up
            man.leftDown(cursorPoint.x, cursorPoint.y);
            man.leftUp(cursorPoint.x, cursorPoint.y);

            INPUT[] input = new INPUT[1];
            input[0].type = 0;

            Debug.Print("cursor: {0}, {1}", (cursorPoint.x * 65536) / 1920,
                (cursorPoint.y * 65536) / 1080);

            input[0].U.mi = new MOUSEINPUT
            {
                dwFlags = MOUSEEVENTF.WHEEL | MOUSEEVENTF.ABSOLUTE,
                dwExtraInfo = new UIntPtr(0),
                dx = (cursorPoint.x * 65536) / 1920,
                dy = (cursorPoint.y * 65536) / 1080,
                mouseData = 2400,
                time = 0
            };

            //mouse_event(MOUSEEVENTF.WHEEL, 0, 0, DWORD(-WHEEL_DELTA), 0);

            SendInput(1, input, INPUT.Size);
        }
    }
}

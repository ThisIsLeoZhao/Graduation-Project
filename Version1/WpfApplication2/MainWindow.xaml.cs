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
using SingleKinect.Manipulator;
using SingleKinect.Manipulator.SystemConstants;
using SingleKinect.Manipulator.SystemConstants.Keyboard;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Manipulator man = new Manipulator();
            man.moveCursor(0x0400, 0x0200);

            man.leftDown(0x0400, 0x0200);
            man.leftUp(0x0400, 0x0200);
//            IntPtr curWindow = MyWindow.GetForegroundWindow();
//
//            SCROLLBARINFO info = new SCROLLBARINFO();
//            info.cbSize = Marshal.SizeOf(info);
//
//            int i = GetScrollBarInfo(curWindow, OBJID_VSCROLL, ref info);
//            
//            //Debug.Print(info.ToString());



            
        }
    }
}

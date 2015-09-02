using SingleKinect.CoordinateConvert;
using SingleKinect.GestureRecognise;

namespace SingleKinect.ConstantDispatcher
{
    public class AdjustableProperties
    {
        public static double SCALE_TRIGGER
        {
            get { return GestureAnalyser.SCALE_TRIGGER; }
            set { GestureAnalyser.SCALE_TRIGGER = value; }
        }

        public static double BLIND_RADIUS
        {
            get { return GestureAnalyser.BLIND_RADIUS; }
            set { GestureAnalyser.BLIND_RADIUS = value; }
        }

        public static double SCALE_SENSITIVITY
        {
            get { return GestureAnalyser.SCALE_SENSITIVITY; }
            set { GestureAnalyser.SCALE_SENSITIVITY = value; }
        }

        public static double SCROLL_SENSITIVITY
        {
            get { return CoordinateConverter.SCROLL_SENSITIVITY; }
            set { CoordinateConverter.SCROLL_SENSITIVITY = value; }
        }

        public static double STEP_WIDTH
        {
            get { return CoordinateConverter.STEP_WIDTH; }
            set { CoordinateConverter.STEP_WIDTH = value; }
        }

        public static double STEP_HEIGHT
        {
            get { return CoordinateConverter.STEP_HEIGHT; }
            set { CoordinateConverter.STEP_HEIGHT = value; }
        }

        public static double MINIMAL_BAR
        {
            get { return CoordinateConverter.MINIMAL_BAR; }
            set { CoordinateConverter.MINIMAL_BAR = value; }
        }
    }
}
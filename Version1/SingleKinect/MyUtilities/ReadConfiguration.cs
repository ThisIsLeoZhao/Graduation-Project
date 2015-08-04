using System.IO;

namespace SingleKinect.MyUtilities
{
    public class ReadConfiguration
    {
        public static void read(string filePath)
        {
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var configuration = line.Split(':')[0];
                var value = double.Parse(line.Split(':')[1]);

                switch (configuration)
                {
                    case "SCREEN_WIDTH":
                        //CoordinateConverter.SCREEN_WIDTH = value;
                        break;
                    case "SCREEN_HEIGHT":
                        //CoordinateConverter.SCREEN_HEIGHT = value;
                        break;
                    case "OP_TRIGGER":
                        GestureRecogniser.GestureRecogniser.OP_TRIGGER = value;
                        break;
                    case "CURSOR_SENSITIVITY":
                        GestureRecogniser.GestureRecogniser.CURSOR_SENSITIVITY = value;
                        break;
                    case "SCALE_SENSITIVITY":
                        GestureRecogniser.GestureRecogniser.SCALE_SENSITIVITY = (int) value;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
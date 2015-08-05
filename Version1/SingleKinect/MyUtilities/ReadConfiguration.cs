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
                if (line.Length == 0)
                {
                    continue;
                }
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
                    case "STEP_WIDTH":
                        CoordinateConverter.STEP_WIDTH = value;
                        break;
                    case "STEP_HEIGHT":
                        CoordinateConverter.STEP_HEIGHT = value;
                        break;
                        
                    case "OP_TRIGGER":
                        GestureRecogniser.GestureRecogniser.OP_TRIGGER = value;
                        break;
                    case "CURSOR_SENSITIVITY":
                        GestureRecogniser.GestureRecogniser.CURSOR_SENSITIVITY = value;
                        break;
                    case "SCALE_SENSITIVITY":
                        GestureRecogniser.GestureRecogniser.SCALE_SENSITIVITY = value;
                        break;
                    case "SCROLL_SENSITIVITY":
                        CoordinateConverter.SCROLL_SENSITIVITY = value;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
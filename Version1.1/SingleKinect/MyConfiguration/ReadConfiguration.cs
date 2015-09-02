using System.IO;
using SingleKinect.ConstantDispatcher;

namespace SingleKinect.MyConfiguration
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
                    case "STEP_WIDTH":
                        AdjustableProperties.STEP_WIDTH = value;
                        break;
                    case "STEP_HEIGHT":
                        AdjustableProperties.STEP_HEIGHT = value;
                        break;
                    case "SCROLL_SENSITIVITY":
                        AdjustableProperties.SCROLL_SENSITIVITY = value;
                        break;
                    case "MINIMAL_BAR":
                        AdjustableProperties.MINIMAL_BAR = value;
                        break;
                    case "SCALE_TRIGGER":
                        AdjustableProperties.SCALE_TRIGGER = value;
                        break;
                    case "BLIND_RADIUS":
                        AdjustableProperties.BLIND_RADIUS = value;
                        break;
                    case "SCALE_SENSITIVITY":
                        AdjustableProperties.SCALE_SENSITIVITY = value;
                        break;
                    
                    default:
                        break;
                }
            }
        }
    }
}
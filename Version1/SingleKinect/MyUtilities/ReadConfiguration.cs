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
                var value = int.Parse(line.Split(':')[1]);

                switch (configuration)
                {
                    case "SCREEN_WIDTH":
                        //CoordinateConverter.SCREEN_WIDTH = value;
                        break;
                    case "SCREEN_HEIGHT":
                        //CoordinateConverter.SCREEN_HEIGHT = value;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;

namespace SingleKinect.MyUtilities
{
    public class ReadConfiguration
    {
        public static void read(string filePath)
        {
            string[] lines = System.IO.File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string configuration = line.Split(':')[0];
                int value = int.Parse(line.Split(':')[1]);

                switch (configuration)
                {
                    case "screenWidth":
                        CoordinateConverter.screenWidth = value;
                        break;
                    case "screenHeight":
                        CoordinateConverter.screenHeight = value;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
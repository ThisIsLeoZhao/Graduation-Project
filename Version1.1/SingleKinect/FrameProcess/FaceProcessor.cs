using System;
using System.Diagnostics;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using SingleKinect.CoordinateConvert;

namespace SingleKinect.FrameProcess
{
    public class FaceProcessor
    {
        private int bodyCount = 6;

        private const double FaceRotationIncrementInDegrees = 5.0;

        public FaceProcessor()
        {
        }

        public bool validateFaceFrame(FaceFrame faceFrame)
        {
            // check if this matchFaceWithBody frame has valid matchFaceWithBody frame results
            return ValidateFaceBoxAndPoints(faceFrame.FaceFrameResult);

        }

        private bool ValidateFaceBoxAndPoints(FaceFrameResult faceResult)
        {
            bool isFaceValid = faceResult != null;
            int displayHeight = CoordinateConverter.SCREEN_HEIGHT;
            int displayWidth = CoordinateConverter.SCREEN_WIDTH;

            if (isFaceValid)
            {
                var faceBox = faceResult.FaceBoundingBoxInColorSpace;
                //Debug.Print("faceBox {0}, {1}, {2}, {3}", faceBox.Left, faceBox.Top, faceBox.Right, faceBox.Bottom);
                // check if we have a valid rectangle within the bounds of the screen space
                isFaceValid = (faceBox.Right - faceBox.Left) > 0 &&
                              (faceBox.Bottom - faceBox.Top) > 0 &&
                              faceBox.Right <= displayWidth &&
                              faceBox.Bottom <= displayHeight;
            
                if (isFaceValid)
                {
                    var facePoints = faceResult.FacePointsInColorSpace;
                    if (facePoints != null)
                    {
                        foreach (PointF pointF in facePoints.Values)
                        {
                            // check if we have a valid face point within the bounds of the screen space
                            bool isFacePointValid = pointF.X > 0.0f &&
                                                    pointF.Y > 0.0f &&
                                                    pointF.X < displayWidth &&
                                                    pointF.Y < displayHeight;
            
                            if (!isFacePointValid)
                            {
                                isFaceValid = false;
                                break;
                            }
                        }
                    }
                }
            }

            return isFaceValid;
        }

        public void ExtractFaceRotationInDegrees(Vector4 rotQuaternion, out int pitch, out int yaw, out int roll)
        {
            double x = rotQuaternion.X;
            double y = rotQuaternion.Y;
            double z = rotQuaternion.Z;
            double w = rotQuaternion.W;

            // convert matchFaceWithBody rotation quaternion to Euler angles in degrees
            double yawD, pitchD, rollD;
            pitchD = Math.Atan2(2 * ((y * z) + (w * x)), (w * w) - (x * x) - (y * y) + (z * z)) / Math.PI * 180.0;
            yawD = Math.Asin(2 * ((w * y) - (x * z))) / Math.PI * 180.0;
            rollD = Math.Atan2(2 * ((x * y) + (w * z)), (w * w) + (x * x) - (y * y) - (z * z)) / Math.PI * 180.0;

            // clamp the values to a multiple of the specified increment to control the refresh rate
            double increment = FaceRotationIncrementInDegrees;
            pitch = (int)(Math.Floor((pitchD + ((increment / 2.0) * (pitchD > 0 ? 1.0 : -1.0))) / increment) * increment);
            yaw = (int)(Math.Floor((yawD + ((increment / 2.0) * (yawD > 0 ? 1.0 : -1.0))) / increment) * increment);
            roll = (int)(Math.Floor((rollD + ((increment / 2.0) * (rollD > 0 ? 1.0 : -1.0))) / increment) * increment);
        }
    }
}
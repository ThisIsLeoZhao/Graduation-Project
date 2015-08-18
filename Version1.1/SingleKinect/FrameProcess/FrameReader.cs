using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using SingleKinect.Draw;
using SingleKinect.EngagementManage;
using SingleKinect.GestureRecognise;
using SingleKinect.Manipulation;
using SingleKinect.MyUtilities;

namespace SingleKinect.FrameProcess
{
    public class FrameReader
    {
        public static FrameReader frameReader;
        private FrameReader() { }

        public static FrameReader Instance
        {
            get
            {
                if (frameReader == null)
                {
                    frameReader = new FrameReader();
                }
                return frameReader;
            }
        }

        public KinectSensor sensor;

        private readonly Drawer drawer = Drawer.Instance;
        private EngagementManager eManager;
        private EngagerTracker eTracker;
        private Manipulator man;
        private GestureRecogniser recogniser;
        private BodyFrameReader bfr;
        private BodyProcessor bodyProcessor;

        private FaceFrameSource[] faceFrameSources;
        private FaceFrameReader[] faceFrameReaders;
        private FaceFrameResult[] faceFrameResults;

        private int bodyCount;

        private const double FaceRotationIncrementInDegrees = 5.0;

        private FaceProcessor faceProcessor;
        public int pitch, yaw, roll;

        public void start()
        {
            eTracker = new EngagerTracker();
            eManager = new EngagementManager();

            man = new Manipulator(eTracker);
            recogniser = new GestureRecogniser(eTracker);

            bodyProcessor = new BodyProcessor(drawer, eManager);
            faceProcessor = new FaceProcessor();

            CoordinateConverter.Sensor = sensor;
            bodyCount = sensor.BodyFrameSource.BodyCount;

            readBodyFrame();
            readFaceFrame();
        }

        private void readBodyFrame()
        {
            bfr = sensor.BodyFrameSource.OpenReader();
            bfr.FrameArrived += bfr_FrameArrived;
        }

        private void readFaceFrame()
        {

            // specify the required face frame results
            FaceFrameFeatures faceFrameFeatures =
                FaceFrameFeatures.BoundingBoxInColorSpace
                | FaceFrameFeatures.PointsInColorSpace
                | FaceFrameFeatures.RotationOrientation
                | FaceFrameFeatures.FaceEngagement
                | FaceFrameFeatures.Glasses
                | FaceFrameFeatures.Happy
                | FaceFrameFeatures.LeftEyeClosed
                | FaceFrameFeatures.RightEyeClosed
                | FaceFrameFeatures.LookingAway
                | FaceFrameFeatures.MouthMoved
                | FaceFrameFeatures.MouthOpen;

            // create a face frame source + reader to track each face in the FOV
            faceFrameSources = new FaceFrameSource[bodyCount];
            faceFrameReaders = new FaceFrameReader[bodyCount];
            for (int i = 0; i < bodyCount; i++)
            {
                // create the face frame source with the required face frame features and an initial tracking Id of 0
                faceFrameSources[i] = new FaceFrameSource(sensor, 0, faceFrameFeatures);

                // open the corresponding reader
                faceFrameReaders[i] = faceFrameSources[i].OpenReader();
            }

            // allocate storage to store face frame results for each face in the FOV
            faceFrameResults = new FaceFrameResult[bodyCount];

            for (int i = 0; i < bodyCount; i++)
            {
                if (faceFrameReaders[i] != null)
                {
                    // wire handler for face frame arrival
                    faceFrameReaders[i].FrameArrived += Reader_FaceFrameArrived;
                }
            }
        }

        private void bfr_FrameArrived(object o, BodyFrameArrivedEventArgs args)
        {
            using (var bodyFrame = args.FrameReference.AcquireFrame())
            {
                drawer.clear();
                if (!bodyProcessor.processBodyFrame(bodyFrame))
                {
                    return;
                }

                // iterate through each face source
                for (int i = 0; i < 6; i++)
                {
                    // check if a valid face is tracked in this face source
                    if (faceFrameSources[i].IsTrackingIdValid)
                    {
                        // check if we have valid face frame results
                        if (faceFrameResults[i] != null)
                        {
                            if (faceFrameResults[i].FaceRotationQuaternion != null)
                            {
                                
                                ExtractFaceRotationInDegrees(faceFrameResults[i].FaceRotationQuaternion, out pitch, out yaw, out roll);
//                                
                            }
                        }
                    }
                    else
                    {
                        bodyProcessor.face(faceFrameSources, i);
                    }
                }

                if (!eManager.HasEngaged)
                {
                    return;
                }

                eTracker.Engager = eManager.Engager;
                //dataSender.send(eTracker.DataToSend);

                // Multithreading maybe
                drawer.currentCanvasName = "engager";
                drawer.drawSkeleton(eTracker);

                if (eManager.DisablingEngagement)
                {
                    return;
                }
                var recognisedGestures = recogniser.recognise();

                man.reactGesture(recognisedGestures);

                
            }
        }

        private void Reader_FaceFrameArrived(object sender, FaceFrameArrivedEventArgs e)
        {
            using (FaceFrame faceFrame = e.FrameReference.AcquireFrame())
            {
                if (faceFrame != null)
                {
                    // get the index of the face source from the face source array
                    int index = GetFaceSourceIndex(faceFrame.FaceFrameSource);
//
                    // check if this face frame has valid face frame results
                    if (ValidateFaceBoxAndPoints(faceFrame.FaceFrameResult))
                    {
                        // store this face frame result to draw later
                        faceFrameResults[index] = faceFrame.FaceFrameResult;
                    }
                    else
                    {
                        // indicates that the latest face frame result from this reader is invalid
                        faceFrameResults[index] = null;
                    }
                }
            }
        }

        private static void ExtractFaceRotationInDegrees(Vector4 rotQuaternion, out int pitch, out int yaw, out int roll)
        {
            double x = rotQuaternion.X;
            double y = rotQuaternion.Y;
            double z = rotQuaternion.Z;
            double w = rotQuaternion.W;

            // convert face rotation quaternion to Euler angles in degrees
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

        private int GetFaceSourceIndex(FaceFrameSource faceFrameSource)
        {
            int index = -1;

            for (int i = 0; i < bodyCount; i++)
            {
                if (faceFrameSources[i] == faceFrameSource)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private bool ValidateFaceBoxAndPoints(FaceFrameResult faceResult)
        {
            bool isFaceValid = faceResult != null;

//            if (isFaceValid)
//            {
//                var faceBox = faceResult.FaceBoundingBoxInColorSpace;
//                if (faceBox != null)
//                {
//                    // check if we have a valid rectangle within the bounds of the screen space
//                    isFaceValid = (faceBox.Right - faceBox.Left) > 0 &&
//                                  (faceBox.Bottom - faceBox.Top) > 0 &&
//                                  faceBox.Right <= displayWidth &&
//                                  faceBox.Bottom <= displayHeight;
//
//                    if (isFaceValid)
//                    {
//                        var facePoints = faceResult.FacePointsInColorSpace;
//                        if (facePoints != null)
//                        {
//                            foreach (PointF pointF in facePoints.Values)
//                            {
//                                // check if we have a valid face point within the bounds of the screen space
//                                bool isFacePointValid = pointF.X > 0.0f &&
//                                                        pointF.Y > 0.0f &&
//                                                        pointF.X < displayWidth &&
//                                                        pointF.Y < displayHeight;
//
//                                if (!isFacePointValid)
//                                {
//                                    isFaceValid = false;
//                                    break;
//                                }
//                            }
//                        }
//                    }
//                }
//            }

            return isFaceValid;
        }

        public void end()
        {
            for (int i = 0; i < bodyCount; i++)
            {
                if (faceFrameReaders[i] != null)
                {
                    // FaceFrameReader is IDisposable
                    faceFrameReaders[i].Dispose();
                    faceFrameReaders[i] = null;
                }

                if (faceFrameSources[i] != null)
                {
                    // FaceFrameSource is IDisposable
                    faceFrameSources[i].Dispose();
                    faceFrameSources[i] = null;
                }
            }

            if (bfr != null)
            {
                // BodyFrameReader is IDisposable
                bfr.Dispose();
                bfr = null;
            }

            if (sensor != null)
            {
                sensor.Close();
                sensor = null;
            }
        }
    }
}
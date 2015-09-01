using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using SingleKinect.CoordinateConvert;
using SingleKinect.Draw;
using SingleKinect.EngagementManage;
using SingleKinect.EngagerTrack;
using SingleKinect.GestureRecognise;
using SingleKinect.Manipulation;

namespace SingleKinect.FrameProcess
{
    public class FrameReader
    {
        private static FrameReader frameReader;
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
        //private EngagerTracker eTracker;
        private Manipulator man;
       // private GestureRecogniser recogniser;
        private BodyFrameReader bfr;
        private BodyProcessor bodyProcessor;

        private FaceFrameSource[] faceFrameSources;
        private FaceFrameReader[] faceFrameReaders;
        private FaceFrameResult[] faceFrameResults;
        private Dictionary<FaceFrameSource, int> ffsDic;
         
        private int bodyCount;

        private FaceProcessor faceProcessor;
        //private EngagerTracker eTracker;

        public void start()
        {
            //eTracker = new EngagerTracker();
            eManager = new EngagementManager();

            man = new Manipulator();
            //recogniser = new GestureRecogniser();

            bodyProcessor = new BodyProcessor(drawer, eManager);
            faceProcessor = new FaceProcessor();

            ffsDic = new Dictionary<FaceFrameSource, int>();

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
            // specify the required matchFaceWithBody frame results
            FaceFrameFeatures faceFrameFeatures =
                FaceFrameFeatures.BoundingBoxInColorSpace
                | FaceFrameFeatures.PointsInColorSpace
                | FaceFrameFeatures.RotationOrientation
                | FaceFrameFeatures.FaceEngagement
                | FaceFrameFeatures.LookingAway;

            // create a matchFaceWithBody frame source + reader to track each matchFaceWithBody in the FOV
            faceFrameSources = new FaceFrameSource[bodyCount];
            faceFrameReaders = new FaceFrameReader[bodyCount];
            for (int i = 0; i < bodyCount; i++)
            {
                // create the matchFaceWithBody frame source with the required matchFaceWithBody frame features and an initial tracking Id of 0
                faceFrameSources[i] = new FaceFrameSource(sensor, 0, faceFrameFeatures);
                ffsDic.Add(faceFrameSources[i], i);

                // open the corresponding reader
                faceFrameReaders[i] = faceFrameSources[i].OpenReader();
            }

            // allocate storage to store matchFaceWithBody frame results for each matchFaceWithBody in the FOV
            faceFrameResults = new FaceFrameResult[bodyCount];

            for (int i = 0; i < bodyCount; i++)
            {
                if (faceFrameReaders[i] != null)
                {
                    // wire handler for matchFaceWithBody frame arrival
                    faceFrameReaders[i].FrameArrived += Reader_FaceFrameArrived;
                }
            }
        }

        private void bfr_FrameArrived(object o, BodyFrameArrivedEventArgs args)
        {
            using (var bodyFrame = args.FrameReference.AcquireFrame())
            {
                if (bodyFrame == null)
                {
                    return;
                }

                drawer.clear();
                bodyProcessor.processBodyFrame(bodyFrame);
                bodyProcessor.matchFaceWithBody(faceFrameSources, faceFrameResults, faceProcessor);
                
                if (!eManager.HasEngaged)
                {
                    return;
                }

                eManager.setTracker();

                // Multithreading maybe
                drawer.currentCanvasName = "engager";
                Body body;
                HandState left, right;
                int yaw, pitch, roll;
                eManager.getTrackerInfo(out body, out left, out right, out yaw, out pitch, out roll);
                drawer.drawSkeleton(body, left, right, yaw, pitch, roll);

                if (eManager.DisablingEngagement)
                {
                    return;
                }
                man.reactToTracker();
            }
        }

        private void Reader_FaceFrameArrived(object sender, FaceFrameArrivedEventArgs e)
        {
            using (FaceFrame faceFrame = e.FrameReference.AcquireFrame())
            {
                if (faceFrame == null)
                {
                    return;
                }
                
                int index = ffsDic[faceFrame.FaceFrameSource];
               // Debug.Print("Face {0} comes with {1} ID {2}", index, faceFrame.FaceFrameSource.IsTrackingIdValid, faceFrame.FaceFrameSource.TrackingId);

                if (faceProcessor.validateFaceFrame(faceFrame))
                {
                    //Debug.Print("Validate Face {0} Succeed with ID {1}", index, faceFrame.FaceFrameSource.TrackingId);
                    faceFrameResults[index] = faceFrame.FaceFrameResult;
                }
                else
                {
                    faceFrameResults[index] = null;
                    //Debug.Print("Face {0} not valid", index);
                }
            }
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using SingleKinect.Draw;
using SingleKinect.EngagementManage;

namespace SingleKinect.FrameProcess
{
    public class BodyProcessor
    {
        private int bodyCount = 6;
        private Body[] bodies;
        private Drawer drawer;
        private EngagementManager eManager;

        private IEnumerable<Body> Bodies
        {
            get { return bodies.Where(body => body.IsTracked); }
        }

        public BodyProcessor(Drawer drawer, EngagementManager eManager)
        {
            bodies = new Body[bodyCount];

            this.drawer = drawer;
            this.eManager = eManager;
        }

        public void processBodyFrame(BodyFrame frame)
        {
            frame.GetAndRefreshBodyData(bodies);

            foreach (var body in Bodies)
            {
                if (!eManager.users.ContainsKey(body.TrackingId))
                {
                    eManager.users[body.TrackingId] = new MyHuman(body);
                    eManager.holdTime[body.TrackingId] = 0;
                }

                // Multithreading maybe
                drawer.currentCanvasName = "body";
                drawer.drawSkeleton(body);
            }
            
        }

        public void matchFaceWithBody(FaceFrameSource[] faceFrameSources, FaceFrameResult[] faceFrameResults, FaceProcessor faceProcessor)
        {
            foreach (var body in bodies)
            {
                if (!body.IsTracked)
                {
                    continue;
                }
                

                int i = getBodyIndex(body);
                //Debug.Print("Body {0} comes with ID {1}", i, body.TrackingId);
                
                if (faceFrameSources[i].IsTrackingIdValid)
                {
                    // check if we have valid face frame results
                    //Debug.Print("Source Valid {0}", i);

                    if (faceFrameResults[i] != null)
                    {
                        //Debug.Print("Result Valid {0}", i);
                        int pitch, yaw, roll;
                        faceProcessor.ExtractFaceRotationInDegrees(faceFrameResults[i].FaceRotationQuaternion, out pitch,
                            out yaw, out roll);
                        

                        eManager.users[faceFrameSources[i].TrackingId].headPitch = pitch;
                        eManager.users[faceFrameSources[i].TrackingId].headYaw = yaw;
                        eManager.users[faceFrameSources[i].TrackingId].headRoll = roll;
                    }
                    else
                    {
                        eManager.users[faceFrameSources[i].TrackingId].headPitch = 1000;
                        eManager.users[faceFrameSources[i].TrackingId].headYaw = 1000;
                        eManager.users[faceFrameSources[i].TrackingId].headRoll = 1000;
                    }

                    if (eManager.HasEngaged && eManager.Engager.body.TrackingId == faceFrameSources[i].TrackingId)
                    {
                        //Debug.Print("Engager: {0}", eManager.Engager.body.TrackingId);
                        eManager.setTrackerFaceOrientation(faceFrameSources[i].TrackingId);
                        
                    }
                }
                else
                {
                    faceFrameSources[i].TrackingId = bodies[i].TrackingId;
                }
            }
        }

        private int getBodyIndex(Body body)
        {
            for (int i = 0; i < bodyCount; i++)
            {
                if (body == bodies[i])
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
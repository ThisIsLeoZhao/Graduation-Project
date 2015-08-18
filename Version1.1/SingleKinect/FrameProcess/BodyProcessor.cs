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
        public int bodyCount = 6;
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

        public bool processBodyFrame(BodyFrame frame)
        {
            if (frame == null)
            {
                return false;
            }

            frame.GetAndRefreshBodyData(bodies);

            foreach (var body in Bodies)
            {
                Debug.Print("Tracking ID {0}", body.TrackingId);

                if (!eManager.users.ContainsKey(body.TrackingId))
                {
                    eManager.users[body.TrackingId] = body;
                    eManager.holdTime[body.TrackingId] = 0;
                }

//                if (!eManager.users.Contains(body))
//                {
//                    eManager.users.Add(body);
//
//                }

                // Multithreading maybe
                drawer.currentCanvasName = "body";
                drawer.drawSkeleton(body);
            }

            return true;
        }

        public void face(FaceFrameSource[] faceFrameSources, int i)
        {
            if (bodies[i].IsTracked)
            {
                // update the face frame source to track this body
                faceFrameSources[i].TrackingId = bodies[i].TrackingId;
            }
        }
    }
}
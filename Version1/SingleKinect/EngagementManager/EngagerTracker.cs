﻿using System;
using System.Data;
using Microsoft.Kinect;
using SingleKinect.Manipulator;
using SingleKinect.Manipulator.MyDataStructures;

namespace SingleKinect.EngagementManager
{
    public class EngagerTracker
    {
        public Joint? preHandLeftPoint = null;
        public Joint? preHandRightPoint = null;
        public Joint curHandLeftPoint;
        public Joint curHandRightPoint;

        private Body engager;
        public Body Engager
        {
            get
            {
                return engager;
            }
            set
            {
                engager = value;
                updateHands();
            } 
        }

        public double ScrollDis { get; set; }
        public bool IsVerticalScroll { get; set; }
        public RECT IncrementRect { get; set; }

        //Positive value for moving down or moving right
        public double VerticalLeftMovement => preHandLeftPoint.Value.Position.Y - curHandLeftPoint.Position.Y;
        public double HorizontalLeftMovement => curHandLeftPoint.Position.X - preHandLeftPoint.Value.Position.X;
        public double VerticalRightMovement => preHandRightPoint.Value.Position.Y - curHandRightPoint.Position.Y;
        public double HorizontalRightMovement => curHandRightPoint.Position.X - preHandRightPoint.Value.Position.X;

        private HandState lastHighConfidenceRightState = HandState.Open;
        private HandState lastHighConfidenceLeftState = HandState.Open;

        public HandState LeftState
        {
            get
            {
                if (engager.HandLeftConfidence == TrackingConfidence.High)
                {
                    lastHighConfidenceLeftState = engager.HandLeftState;
                    if (engager.HandLeftState == HandState.NotTracked || engager.HandLeftState == HandState.Unknown)
                    {
                        lastHighConfidenceLeftState = HandState.Open;
                        return lastHighConfidenceLeftState;
                    }
                    return engager.HandLeftState;
                }
                
                return lastHighConfidenceLeftState;
            }
        }

        public HandState RightState
        {
            get
            {
                if (engager.HandRightConfidence == TrackingConfidence.High)
                {
                    lastHighConfidenceRightState = engager.HandRightState;
                    if (engager.HandRightState == HandState.NotTracked || engager.HandRightState == HandState.Unknown)
                    {
                        lastHighConfidenceRightState = HandState.Open;
                        return HandState.Open;
                    }
                    return engager.HandRightState;
                }
                
                return lastHighConfidenceRightState;
            }
        }

        private void updateHands()
        {
            Joint tempLeftJoint = curHandLeftPoint;
            Joint tempRightJoint = curHandRightPoint;

            curHandLeftPoint = Engager.Joints[JointType.HandLeft];
            curHandRightPoint = Engager.Joints[JointType.HandRight];

            if (!preHandRightPoint.HasValue)
            {
                preHandRightPoint = curHandRightPoint;
            }
            else
            {
                preHandRightPoint = tempRightJoint;

            }

            if (!preHandLeftPoint.HasValue)
            {
                preHandLeftPoint = curHandLeftPoint;
            }
            else
            {
                preHandLeftPoint = tempLeftJoint;

            }
        }
    }
}
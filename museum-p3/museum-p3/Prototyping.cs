using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

using Microsoft;

namespace SkeletalTracking
{
    class CustomController1 : SkeletonController
    {
        public CustomController1(MainWindow win) : base(win){ window = win; }

        private MainWindow window;

        private int highlighting = 0;
        private int selecting = 0;
        private int numTargets = 6;

        //Note: targets is a dictionary that allows you to retrieve the corresponding target on screen
        //and manipulate its state and position, as well as hide/show it (see class defn. below).
        //It is indexed from 1, thus you can retrieve an individual target with the expression
        //targets[3], which would retrieve the target labeled "3" on screen.

            Joint previousLeftHand, previousRightHand;
            Joint head = skeleton.Joints[JointID.Head].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            //Joint center = skeleton.Joints[JointID.HipCenter].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint rightShoulder = skeleton.Joints[JointID.ShoulderRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint leftHand = skeleton.Joints[JointID.HandLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint rightHand = skeleton.Joints[JointID.HandRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            


        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {

            Joint leftElbow = skeleton.Joints[JointID.ElbowLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint leftWrist = skeleton.Joints[JointID.WristLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint leftShoulder = skeleton.Joints[JointID.ShoulderLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);

            float slopeStoHinY = (leftShoulder.Position.Y - leftHand.Position.Y) / (leftShoulder.Position.Z - leftShoulder.Position.Z);
            float slopeStoHinX = (leftShoulder.Position.X - leftHand.Position.X) / (leftShoulder.Position.Z - leftShoulder.Position.Z);

            double y_from_kinect = (((leftHand.Position.Y - leftShoulder.Position.Y) / (leftHand.Position.Z - leftShoulder.Position.Z)  ) * leftShoulder.Position.Z) + leftShoulder.Position.Y;
            double x_from_kinect = (((leftHand.Position.X - leftShoulder.Position.X) / (leftHand.Position.Z - leftShoulder.Position.Z)  ) * leftShoulder.Position.Z) + leftShoulder.Position.X;

            int highlighted = 0;
            for(int i=0; i<numTargets; i++)
            {
                if( (Math.Abs(targets[i].getXPosition() - x_from_kinect) < 20 ) && (Math.Abs(targets[i].getYPosition() - y_from_kinect) < 20) )
                {
                    highlighted = i;
                }
            }
        }

            public float AngleBetweenTwoVectors(Vector3 vectorA, Vector3 vectorB)
            {
                float dotProduct = 0.0f;
                dotProduct=  Vector3.Dot( vectorA,  vectorB );

                return (float)Math.Acos(dotProduct); 
            }






            // while the hands are both in front of the user within a square centered on their chest
                    if ((leftHand.Position.X < (leftShoulder.Position.X + 20)) && (leftHand.Position.X > (leftShoulder.Position.X - 20)) &&
                        (rightHand.Position.X < (rightShoulder.Position.X + 20)) && (rightHand.Position.X > (rightShoulder.Position.X - 20))
                    )
                    {
                        float leftDirection = leftHand.Position.X - previousLeftHand.Position.X;
                        float rightDirection = rightHand.Position.X - previousRightHand.Position.X;
                        float destX, destY;
                        calculateVector(leftHand.Position.X, rightHand.Position.X, leftHand.Position.Y, rightHand.Position.Y, destX, destY);
                        // if hands separating
                        if (leftDirection < 0 && rightDirection > 0)
                        {
                            // zoom into the image at point where the directional vector pointing from the midpoint of the two hands
                            // to the "screen" meets
                            zoomInAtPoint(destX, destY);
                        }

                        // if hands coming together
                        else if (leftDirection > 0 && rightDirection < 0)
                        {
                            // zoom out of the image at point where the vector pointing from the midpoint of the two hands
                            // to the "screen" meets
                            zoomOutAtPoint(destX, destY);
                        }


                        // if hands moving in the same direction
                        // if moving in the minux X direction, pan to the left

                        else if (leftDirection > 0 && rightDirection > 0)
                        {
                            float distanceMoved = previousLeftHand.Position.X - leftHand.Position.X;
                            //pan to the Left by number of pixels moved scaled by the zoom
                            panLeft(distanceMoved);
                        }
                        // if moving in the positive X direction, pan to the right
                        else if (leftDirection < 0 && rightDirection < 0)
                        {
                            float distanceMoved = previousRightHand.Position.X - rightHand.Position.X;
                            //pan to the Right by number of pixels moved scaled by the zoom
                            panRight(distanceMoved);
                        }
                    }
        }


        public void panLeft(float distance)
        {
        }

        public void panRight(float distance)
        {
        }

        public void calculateVector(float X1, float X2, float Y1, float Y2, float resultX, float resultY)
        {

        }

        public void zoomInAtPoint(float X, float Y)
        {
        
        }

        public void zoomOutAtPoint(float X, float Y)
        {
        
        }


        //This is called when the controller becomes active. This allows you to place your targets and do any 
        //initialization that you don't want to repeat with each new skeleton frame. You may also 
        //directly move the targets in the MainWindow.xaml file to achieve the same initial repositioning.
        public override void controllerActivated(Dictionary<int, Target> targets)
        {
            resetTargets(targets);

            //Set target position
            int yVal = 100;
            int xVal = 50;
            int xIncrement = 110;
            for (int i = 1; i <= 5; i++)
            {
                targets[i].setTargetPosition(xVal, yVal);
                xVal += xIncrement;
            }

            //Set target text
            targets[1].setTargetText("eyes");
            targets[2].setTargetText("ears");
            targets[3].setTargetText("heart");
            targets[4].setTargetText("stomach");
            targets[5].setTargetText("hands");

            //Set text size and properties
            for (int i = 1; i <= 5; i++)
            {
                targets[i].setTargetTextSize(18);
                targets[i].setTargetTextPadding("0,25,0,0");
                targets[i].setTargetTextAlign(TextAlignment.Center);
            }

            //targets[2].hideTarget();
            //targets[2].showTarget();
            //targets[5].isHidden();
            //targets[3].setTargetHighlighted();

            //Runtime runtime = Runtime.Kinects[0];
            //runtime.NuiCamera.ElevationAngle = 10;
        }
    }
}

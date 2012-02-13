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

namespace SkeletalTracking
{
    class CustomController2 : SkeletonController
    {
        public CustomController2(MainWindow win) : base(win) { window = win; }

        private MainWindow window;

        private int highlighting = 0;
        private int selecting = 0;
        private long highlightStart;

        //Note: targets is a dictionary that allows you to retrieve the corresponding target on screen
        //and manipulate its state and position, as well as hide/show it (see class defn. below).
        //It is indexed from 1, thus you can retrieve an individual target with the expression
        //targets[3], which would retrieve the target labeled "3" on screen.
        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {
            Joint head = skeleton.Joints[JointID.Head].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint leftHand = skeleton.Joints[JointID.HandLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint rightHand = skeleton.Joints[JointID.HandRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint leftShoulder = skeleton.Joints[JointID.ShoulderLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint rightShoulder = skeleton.Joints[JointID.ShoulderRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint center = skeleton.Joints[JointID.HipCenter].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            
            int targetHighlighted = 0;
            
            //Check for eyes gesture
            //double leftXYDist = Math.Sqrt(Math.Pow(Math.Abs(leftHand.Position.X - head.Position.X), 2) + Math.Pow(Math.Abs(leftHand.Position.Y - head.Position.Y), 2));
            //double rightXYDist = Math.Sqrt(Math.Pow(Math.Abs(rightHand.Position.X - head.Position.X), 2) + Math.Pow(Math.Abs(rightHand.Position.Y - head.Position.Y), 2));
            double leftXDist = Math.Abs(head.Position.X - leftHand.Position.X);
            double rightXDist = Math.Abs(head.Position.X - rightHand.Position.X);
            if (leftXDist < 30 && rightXDist < 30)
            {
                double leftYDist = Math.Abs(head.Position.Y - leftHand.Position.Y);
                double rightYDist = Math.Abs(head.Position.Y - rightHand.Position.Y);
                if (leftYDist < 40 && rightYDist < 40)
                {
                    targetHighlighted = 1;
                }
            }
            
            //Check for ears gesture
            leftXDist = head.Position.X - leftHand.Position.X;
            rightXDist = rightHand.Position.X - head.Position.X;
            if (leftXDist > 30 && rightXDist > 30 && leftXDist < 100 && rightXDist < 100)
            {
                double leftYDist = Math.Abs(head.Position.Y - leftHand.Position.Y);
                double rightYDist = Math.Abs(head.Position.Y - rightHand.Position.Y);
                if (leftYDist < 40 && rightYDist < 40)
                {
                    targetHighlighted = 2;
                }
            }

            //Check for heart gesture
            double rightXDistFromRightShoulder = rightHand.Position.X - rightShoulder.Position.X;

            if (rightHand.Position.X < rightShoulder.Position.X && rightHand.Position.X > leftShoulder.Position.X)
            {
                double rightYDistFromLeftShoulder = rightHand.Position.Y - leftShoulder.Position.Y;
                double rightYDistFromRightShoulder = rightHand.Position.Y - rightShoulder.Position.Y;
                if ((rightYDistFromLeftShoulder > 0 && rightYDistFromLeftShoulder < 50) || (rightYDistFromLeftShoulder > 0 && rightYDistFromLeftShoulder < 50))
                {
                    targetHighlighted = 3;
                }
            }

            //Check for stomach gesture
            leftXDist = center.Position.X - leftHand.Position.X;
            rightXDist = rightHand.Position.X - center.Position.X;

            if (leftXDist > -20 && leftXDist < 80 && rightXDist > -20 && rightXDist < 80)
            {
                double leftYDist = center.Position.Y - leftHand.Position.Y;
                double rightYDist = center.Position.Y - rightHand.Position.Y;

                if (leftYDist > -10 && leftYDist < 10 && rightYDist > -10 && rightYDist < 30)
                {
                    targetHighlighted = 4;
                }
            }

            //Check for hands gesture
            leftXDist = leftShoulder.Position.X - leftHand.Position.X;
            rightXDist = rightHand.Position.X - rightShoulder.Position.X;

            if (leftXDist > 55 && rightXDist > 55)
            {
                double leftYDist = leftHand.Position.Y - leftShoulder.Position.Y;
                double rightYDist = rightHand.Position.Y - rightShoulder.Position.Y;

                if (leftYDist > -20 && leftYDist < 100 && rightYDist > -20 && rightYDist < 100)
                {
                    targetHighlighted = 5;
                }
            }

            //Check if we detected a gesture and aren't currently highlighting
            if (targetHighlighted > 0)
            {
                //Start the highlight timer if it hasn't started already or if we're switching targets
                if (highlighting == 0 || (targetHighlighted > 0 && targetHighlighted != highlighting))
                {
                    highlighting = targetHighlighted;

                    //Start the timer
                    highlightStart = DateTime.Now.Ticks;
                }
            }

            //If it's been long enough, select the currently highlighted target
            if (highlighting > 0 && (DateTime.Now.Ticks - highlightStart) > 20000000)
            {
                //Deselect all other targets
                for (int i = 1; i <= 5; i++)
                {
                    targets[i].setTargetUnselected();
                }

                selecting = highlighting;
                highlighting = 0;
            }

            if (highlighting > 0 && targetHighlighted == 0)
            {
                highlighting = 0;
            }

            //Reset all highlighting
            for (int i = 1; i <= 5; i++)
            {
                targets[i].setTargetUnselected();
            }

            //Highlight the current highlighted target
            if(highlighting > 0) targets[highlighting].setTargetHighlighted();
            if(selecting > 0) targets[selecting].setTargetSelected();
        }

        //This is called when the controller becomes active. This allows you to place your targets and do any 
        //initialization that you don't want to repeat with each new skeleton frame. You may also 
        //directly move the targets in the MainWindow.xaml file to achieve the same initial repositioning.
        public override void controllerActivated(Dictionary<int, Target> targets)
        {
            resetTargets(targets);

            //Set target position and text
            targets[1].setTargetPosition(50, 50);
            targets[1].setTargetText("eyes");
            targets[2].setTargetPosition(480, 50);
            targets[2].setTargetText("ears");
            targets[3].setTargetPosition(50, 340);
            targets[3].setTargetText("heart");
            targets[4].setTargetPosition(480, 340);
            targets[4].setTargetText("stomach");
            targets[5].setTargetPosition(262, 50);
            targets[5].setTargetText("hands");

            //Set text size and properties
            for (int i = 1; i <= 5; i++)
            {
                targets[i].setTargetTextSize(18);
                targets[i].setTargetTextPadding("0,25,0,0");
                targets[i].setTargetTextAlign(TextAlignment.Center);
            }

            //targets[1].setTargetPosition(80, 200);
            //targets[2].hideTarget();
            //targets[2].showTarget();
            //targets[5].isHidden();
            //targets[3].setTargetHighlighted();
        }
    }
}

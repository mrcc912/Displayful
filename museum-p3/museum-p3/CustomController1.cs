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
    class CustomController1 : SkeletonController
    {
        public CustomController1(MainWindow win) : base(win){ window = win; }

        private MainWindow window;

        private int highlighting = 0;
        private int selecting = 0;

        //Note: targets is a dictionary that allows you to retrieve the corresponding target on screen
        //and manipulate its state and position, as well as hide/show it (see class defn. below).
        //It is indexed from 1, thus you can retrieve an individual target with the expression
        //targets[3], which would retrieve the target labeled "3" on screen.
        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {
            Joint head = skeleton.Joints[JointID.Head].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            //Joint center = skeleton.Joints[JointID.HipCenter].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint leftShoulder = skeleton.Joints[JointID.ShoulderLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint rightShoulder = skeleton.Joints[JointID.ShoulderRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint leftHand = skeleton.Joints[JointID.HandLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint rightHand = skeleton.Joints[JointID.HandRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);

            //Check targets for highlighting and selection
            for (var i = 1; i <= 5; i++)
            {
                double targetXPos = targets[i].getXPosition();

                //Check to see which region
                if (Math.Abs(targetXPos - head.Position.X) < 40)
                {
                    highlighting = i;

                    //Check to see if the arms are above the head
                    if (leftHand.Position.Y < leftShoulder.Position.Y || rightHand.Position.Y < leftShoulder.Position.Y || leftHand.Position.Y < rightShoulder.Position.Y || rightHand.Position.Y < rightShoulder.Position.Y)
                    {
                        selecting = i;
                    }
                }
            }

            //Reflect the current highlighting/selection state in the targets
            for (var i = 1; i <= 5; i++)
            {
                targets[i].setTargetUnselected();
            }
            if (highlighting > 0) targets[highlighting].setTargetHighlighted();
            if (selecting > 0) targets[selecting].setTargetSelected();
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

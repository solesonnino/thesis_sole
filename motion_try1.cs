/*code to see if it is possible to make the robot move in sequence: move the base, pick&place and compute the jacobian*/

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Tecnomatix.Engineering;
using System.Collections.Generic;
using Tecnomatix.Engineering.Olp;
using System.Linq;
using System.Collections;


class Program 
{
    static public void Main(ref StringWriter output)
    {
        //an array that represents the one recieved by python 
        int[] layout= {100, -200, 800};

       
        for (int pos=0; pos < 4; pos++)
        {
            //move the base of the robot in the defined position 
            // move along x axis 
            TxObjectList selectedObjects = TxApplication.ActiveSelection.GetItems();
            selectedObjects = TxApplication.ActiveDocument.GetObjectsByName("UR5e");
            var robot = selectedObjects[1] as ITxLocatableObject;
            double move_X_Val=0;
            move_X_Val= layout[pos];	
            var position = new TxTransformation(robot.LocationRelativeToWorkingFrame);
            position.Translation = new TxVector(move_X_Val, 0, 0);
            robot.LocationRelativeToWorkingFrame = position;
            TxApplication.RefreshDisplay();
            output.Write("the current position is: \n");
            output.Write(move_X_Val.ToString());
            output.Write("\n");

            //move the robot
            // Define some variables
                
            string operation_name = "RoboticProgram_2";

            /// string new_tcp = "tcp_1";
            string new_motion_type = "MoveL";
            string new_speed = "1000";
            string new_accel = "1200";
            string new_blend = "0";
            string new_coord = "Cartesian";
            
            bool verbose = false; // Controls some display options
        
            // Save the robot (the index may change)  	
            TxObjectList objects = TxApplication.ActiveDocument.GetObjectsByName("UR5e");
            var robot2 = objects[1] as TxRobot;
                
            // Create the new operation    	
            TxContinuousRoboticOperationCreationData data = new TxContinuousRoboticOperationCreationData(operation_name);
            TxApplication.ActiveDocument.OperationRoot.CreateContinuousRoboticOperation(data);
            
            // Get the created operation
            TxTypeFilter opFilter = new TxTypeFilter(typeof(TxContinuousRoboticOperation));
            TxOperationRoot opRoot = TxApplication.ActiveDocument.OperationRoot;
                    
            TxObjectList allOps = opRoot.GetAllDescendants(opFilter);
            TxContinuousRoboticOperation MyOp = allOps[0] as TxContinuousRoboticOperation; // The index may change

            // Create all the necessary points       
            TxRoboticViaLocationOperationCreationData Point1 = new TxRoboticViaLocationOperationCreationData();
            Point1.Name = "point1"; // First point
            
            TxRoboticViaLocationOperationCreationData Point2 = new TxRoboticViaLocationOperationCreationData();
            Point2.Name = "point2"; // Second point
            
            TxRoboticViaLocationOperationCreationData Point3 = new TxRoboticViaLocationOperationCreationData();
            Point3.Name = "point3"; // Third point
            
            TxRoboticViaLocationOperation FirstPoint = MyOp.CreateRoboticViaLocationOperation(Point1);
            TxRoboticViaLocationOperation SecondPoint = MyOp.CreateRoboticViaLocationOperationAfter(Point2, FirstPoint);
            TxRoboticViaLocationOperation ThirdPoint = MyOp.CreateRoboticViaLocationOperationAfter(Point3, SecondPoint);
            
            // Impose a position to the new waypoint		
            double rotVal = Math.PI;
            TxTransformation rotX = new TxTransformation(new TxVector(rotVal, 0, 0), 
            TxTransformation.TxRotationType.RPY_XYZ);
            FirstPoint.AbsoluteLocation = rotX;
            
            var pointA = new TxTransformation(FirstPoint.AbsoluteLocation);
            pointA.Translation = new TxVector(300, 0, 300);
            FirstPoint.AbsoluteLocation = pointA;
            
            // Impose a position to the second waypoint		
            double rotVal2 = Math.PI;
            TxTransformation rotX2 = new TxTransformation(new TxVector(rotVal2, 0, 0), 
            TxTransformation.TxRotationType.RPY_XYZ);
            SecondPoint.AbsoluteLocation = rotX2;
            
            var pointB = new TxTransformation(SecondPoint.AbsoluteLocation);
            pointB.Translation = new TxVector(300, 0, 25);
            SecondPoint.AbsoluteLocation = pointB;
            
            // Impose a position to the third waypoint		
            double rotVal3 = Math.PI;
            TxTransformation rotX3 = new TxTransformation(new TxVector(rotVal3, 0, 0), 
            TxTransformation.TxRotationType.RPY_XYZ);
            ThirdPoint.AbsoluteLocation = rotX3;
            
            var pointC = new TxTransformation(ThirdPoint.AbsoluteLocation);
            pointC.Translation = new TxVector(300, 0, 300);
            ThirdPoint.AbsoluteLocation = pointC;

            // NOTE: you must associate the robot to the operation!
            MyOp.Robot = robot2; 

            // Implement the logic to access the parameters of the controller		
            TxOlpControllerUtilities ControllerUtils = new TxOlpControllerUtilities();		
            TxRobot AssociatedRobot = ControllerUtils.GetRobot(MyOp); // Verify the correct robot is associated 
                    
            ITxOlpRobotControllerParametersHandler paramHandler = (ITxOlpRobotControllerParametersHandler)
            ControllerUtils.GetInterfaceImplementationFromController(robot2.Controller.Name,
            typeof(ITxOlpRobotControllerParametersHandler), typeof(TxRobotSimulationControllerAttribute),
            "ControllerName");
            
            // Set the new parameters for the waypoint					
            //	paramHandler.OnComplexValueChanged("Tool", new_tcp, FirstPoint);
            paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, FirstPoint);
            paramHandler.OnComplexValueChanged("Speed", new_speed, FirstPoint);
            paramHandler.OnComplexValueChanged("Accel", new_accel, FirstPoint);
            paramHandler.OnComplexValueChanged("Blend", new_blend, FirstPoint);
            paramHandler.OnComplexValueChanged("Coord Type", new_coord, FirstPoint);
            
            //	paramHandler.OnComplexValueChanged("Tool", new_tcp, SecondPoint);
            paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, SecondPoint);
            paramHandler.OnComplexValueChanged("Speed", new_speed, SecondPoint);
            paramHandler.OnComplexValueChanged("Accel", new_accel, SecondPoint);
            paramHandler.OnComplexValueChanged("Blend", new_blend, SecondPoint);
            paramHandler.OnComplexValueChanged("Coord Type", new_coord, SecondPoint);
            
            //	paramHandler.OnComplexValueChanged("Tool", new_tcp, ThirdPoint);
            paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, ThirdPoint);
            paramHandler.OnComplexValueChanged("Speed", new_speed, ThirdPoint);
            paramHandler.OnComplexValueChanged("Accel", new_accel, ThirdPoint);
            paramHandler.OnComplexValueChanged("Blend", new_blend, ThirdPoint);
            paramHandler.OnComplexValueChanged("Coord Type", new_coord, ThirdPoint);

        }

    }    

}
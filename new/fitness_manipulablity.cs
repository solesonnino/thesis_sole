/*
Test1: more than one array is sent to C# (in sequence)
*/

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
     static StringWriter m_output;

    static double determinantSum = 0;
    static double determinantCounter = 0;
    static public void Main(ref StringWriter output)
    {
        m_output = output;
        TcpListener server = null;
        try
        {
            // Define the number of simulations
            int Nsim = 8;
            int port = 12345;
            int particles=5;
            double[] fitness = new double[particles];

            // Start listening for incoming connections
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            server.Start();
            output.Write("Server started...");

            // Accept a client connection
            TcpClient client = server.AcceptTcpClient();
            NetworkStream stream = client.GetStream();

            // Loop for all the simulations
            for (int ii = 0; ii < Nsim ; ii++)
            {
                // Receive positions array
                var layout = ReceiveNumpyArray(stream);
                output.Write("positions: \n");
                PrintArray(layout, output);
                output.Write("\n");

                //select each position 
                for (int pos=0; pos < 5; pos++)
                {
                    determinantCounter=0;
                    determinantSum=0;
                    //move the base of the robot in the defined position 
                    // move along x axis 
                    TxObjectList selectedObjects = TxApplication.ActiveSelection.GetItems();
                    selectedObjects = TxApplication.ActiveDocument.GetObjectsByName("UR5e");
                    var robot = selectedObjects[0] as ITxLocatableObject;
                    double move_Y_Val=0;
                    move_Y_Val= layout[0,pos];	
                    var position = new TxTransformation(robot.LocationRelativeToWorkingFrame);
                    position.Translation = new TxVector(0, move_Y_Val, 0);
                    robot.LocationRelativeToWorkingFrame = position;
                    TxApplication.RefreshDisplay();
                    output.Write("the current position is: \n");
                    output.Write(move_Y_Val.ToString());
                    output.Write("\n");
                    
                    // Define some variables
                    string pos_string = pos.ToString();    
                    string operation_name = "RoboticProgram_" + ii.ToString()+ pos_string;

                    /// string new_tcp = "tcp_1";
                    string new_motion_type = "MoveL";
                    string new_speed = "1000";
                    string new_accel = "1200";
                    string new_blend = "0";
                    string new_coord = "Cartesian";
                    string new_tcp = "T_gripper";
                    
                    bool verbose = false; // Controls some display options
                
                    // Save the robot (the index may change)  	
                    TxObjectList objects = TxApplication.ActiveDocument.GetObjectsByName("UR5e");
                    var robot2 = objects[0] as TxRobot;
                        
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

                    TxRoboticViaLocationOperationCreationData Point4 = new TxRoboticViaLocationOperationCreationData();
                    Point4.Name = "point4"; // fourth point
                    
                    TxRoboticViaLocationOperationCreationData Point5 = new TxRoboticViaLocationOperationCreationData();
                    Point5.Name = "point5"; // fifth point
                    
                    TxRoboticViaLocationOperationCreationData Point6 = new TxRoboticViaLocationOperationCreationData();
                    Point6.Name = "point6"; // sixth point

                    TxRoboticViaLocationOperationCreationData Point7 = new TxRoboticViaLocationOperationCreationData();
                    Point7.Name = "point7"; // seventh point
                    
                    TxRoboticViaLocationOperationCreationData Point8 = new TxRoboticViaLocationOperationCreationData();
                    Point8.Name = "point8"; // eighth point
                    

                    TxRoboticViaLocationOperation FirstPoint = MyOp.CreateRoboticViaLocationOperation(Point1);
                    TxRoboticViaLocationOperation SecondPoint = MyOp.CreateRoboticViaLocationOperationAfter(Point2, FirstPoint);
                    TxRoboticViaLocationOperation ThirdPoint = MyOp.CreateRoboticViaLocationOperationAfter(Point3, SecondPoint);
                    TxRoboticViaLocationOperation FourthPoint = MyOp.CreateRoboticViaLocationOperationAfter(Point4, ThirdPoint);
                    TxRoboticViaLocationOperation FifthPoint = MyOp.CreateRoboticViaLocationOperationAfter(Point5, FourthPoint);
                    TxRoboticViaLocationOperation SixthPoint = MyOp.CreateRoboticViaLocationOperationAfter(Point6, FifthPoint);
                    TxRoboticViaLocationOperation SeventhPoint = MyOp.CreateRoboticViaLocationOperationAfter(Point7, SixthPoint);
                    TxRoboticViaLocationOperation EighthPoint = MyOp.CreateRoboticViaLocationOperationAfter(Point8, SeventhPoint);

                    //save the initial position of the tcp in EighthPoint
                    TxFrame TCPpose1 = TxApplication.ActiveDocument.GetObjectsByName("TCPF")[0] as TxFrame;
                    var TCP_pose1 = new TxTransformation(TCPpose1.LocationRelativeToWorkingFrame); 
                    EighthPoint.LocationRelativeToWorkingFrame = TCP_pose1;

                    // Impose a position to the new waypoint		
                    double rotVal = Math.PI;
                    TxTransformation rotX = new TxTransformation(new TxVector(rotVal, 0, 0), 
                    TxTransformation.TxRotationType.RPY_XYZ);
                    FirstPoint.AbsoluteLocation = rotX;
                    
                    var pointA = new TxTransformation(FirstPoint.AbsoluteLocation);
                    pointA.Translation = new TxVector(600, -300, 300);
                    FirstPoint.AbsoluteLocation = pointA;
                    
                    // Impose a position to the second waypoint		
                    double rotVal2 = Math.PI;
                    TxTransformation rotX2 = new TxTransformation(new TxVector(rotVal2, 0, 0), 
                    TxTransformation.TxRotationType.RPY_XYZ);
                    SecondPoint.AbsoluteLocation = rotX2;
                    
                    var pointB = new TxTransformation(SecondPoint.AbsoluteLocation);
                    pointB.Translation = new TxVector(600, -300, 25);
                    SecondPoint.AbsoluteLocation = pointB;
                    
                    // Impose a position to the third waypoint		
                    double rotVal3 = Math.PI;
                    TxTransformation rotX3 = new TxTransformation(new TxVector(rotVal3, 0, 0), 
                    TxTransformation.TxRotationType.RPY_XYZ);
                    ThirdPoint.AbsoluteLocation = rotX3;
                    
                    var pointC = new TxTransformation(ThirdPoint.AbsoluteLocation);
                    pointC.Translation = new TxVector(600, -300, 300);
                    ThirdPoint.AbsoluteLocation = pointC;

                    // Impose a position to the fourth waypoint		
                    double rotVal4 = Math.PI;
                    TxTransformation rotX4 = new TxTransformation(new TxVector(rotVal4, 0, 0), 
                    TxTransformation.TxRotationType.RPY_XYZ);
                    FourthPoint.AbsoluteLocation = rotX4;
                    
                    var pointD = new TxTransformation(FourthPoint.AbsoluteLocation);
                    pointD.Translation = new TxVector(600, 300, 300);
                    FourthPoint.AbsoluteLocation = pointD;

                    // Impose a position to the fifth waypoint		
                    double rotVal5 = Math.PI;
                    TxTransformation rotX5 = new TxTransformation(new TxVector(rotVal5, 0, 0), 
                    TxTransformation.TxRotationType.RPY_XYZ);
                    FifthPoint.AbsoluteLocation = rotX5;
                    
                    var pointE = new TxTransformation(FifthPoint.AbsoluteLocation);
                    pointE.Translation = new TxVector(600, 300, 25);
                    FifthPoint.AbsoluteLocation = pointE;

                    // Impose a position to the sixth waypoint		
                    double rotVal6 = Math.PI;
                    TxTransformation rotX6 = new TxTransformation(new TxVector(rotVal6, 0, 0), 
                    TxTransformation.TxRotationType.RPY_XYZ);
                    SixthPoint.AbsoluteLocation = rotX6;
                    
                    var pointF = new TxTransformation(SixthPoint.AbsoluteLocation);
                    pointF.Translation = new TxVector(600,300,300);
                    SixthPoint.AbsoluteLocation = pointF;



                    // Impose a position to the seventh waypoint --> go back to initial position		
                    /*double rotVal7 = Math.PI;
                    TxTransformation rotX7 = new TxTransformation(new TxVector(rotVal7, 0, 0), 
                    TxTransformation.TxRotationType.RPY_XYZ);
                    SeventhPoint.AbsoluteLocation = rotX7;

                    var pointG = new TxTransformation(SeventhPoint.AbsoluteLocation);
                    pointG.Translation = new TxVector(EighthPoint.LocationRelativeToWorkingFrame);*/
                    SeventhPoint.AbsoluteLocation = EighthPoint.LocationRelativeToWorkingFrame;

                    // Impose a position to the eighth waypoint		
                    /*double rotVal8 = Math.PI;
                    TxTransformation rotX8 = new TxTransformation(new TxVector(rotVal8, 0, 0), 
                    TxTransformation.TxRotationType.RPY_XYZ);
                    EighthPoint.AbsoluteLocation = rotX8;
                    
                    var pointF = new TxTransformation(EighthPoint.AbsoluteLocation);
                    pointF.Translation = new TxVector(pointA);
                    EighthPoint.AbsoluteLocation = pointG;*/




                    //move the robot


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
                    paramHandler.OnComplexValueChanged("Tool", new_tcp, FirstPoint);

                    //	paramHandler.OnComplexValueChanged("Tool", new_tcp, SecondPoint);
                    paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, SecondPoint);
                    paramHandler.OnComplexValueChanged("Speed", new_speed, SecondPoint);
                    paramHandler.OnComplexValueChanged("Accel", new_accel, SecondPoint);
                    paramHandler.OnComplexValueChanged("Blend", new_blend, SecondPoint);
                    paramHandler.OnComplexValueChanged("Coord Type", new_coord, SecondPoint);
                    paramHandler.OnComplexValueChanged("Tool", new_tcp, SecondPoint);

                    //	paramHandler.OnComplexValueChanged("Tool", new_tcp, ThirdPoint);
                    paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, ThirdPoint);
                    paramHandler.OnComplexValueChanged("Speed", new_speed, ThirdPoint);
                    paramHandler.OnComplexValueChanged("Accel", new_accel, ThirdPoint);
                    paramHandler.OnComplexValueChanged("Blend", new_blend, ThirdPoint);
                    paramHandler.OnComplexValueChanged("Coord Type", new_coord, ThirdPoint);
                    paramHandler.OnComplexValueChanged("Tool", new_tcp, ThirdPoint);

                    //	paramHandler.OnComplexValueChanged("Tool", new_tcp, FourthPoint);
                    paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, FourthPoint);
                    paramHandler.OnComplexValueChanged("Speed", new_speed, FourthPoint);
                    paramHandler.OnComplexValueChanged("Accel", new_accel, FourthPoint);
                    paramHandler.OnComplexValueChanged("Blend", new_blend, FourthPoint);
                    paramHandler.OnComplexValueChanged("Coord Type", new_coord, FourthPoint);
                    paramHandler.OnComplexValueChanged("Tool", new_tcp, FourthPoint);

                    //	paramHandler.OnComplexValueChanged("Tool", new_tcp, FifthPoint);
                    paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, FifthPoint);
                    paramHandler.OnComplexValueChanged("Speed", new_speed, FifthPoint);
                    paramHandler.OnComplexValueChanged("Accel", new_accel, FifthPoint);
                    paramHandler.OnComplexValueChanged("Blend", new_blend, FifthPoint);
                    paramHandler.OnComplexValueChanged("Coord Type", new_coord, FifthPoint);
                    paramHandler.OnComplexValueChanged("Tool", new_tcp, FifthPoint);

                    //	paramHandler.OnComplexValueChanged("Tool", new_tcp, SixthPoint);
                    paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, SixthPoint);
                    paramHandler.OnComplexValueChanged("Speed", new_speed, SixthPoint);
                    paramHandler.OnComplexValueChanged("Accel", new_accel, SixthPoint);
                    paramHandler.OnComplexValueChanged("Blend", new_blend, SixthPoint);
                    paramHandler.OnComplexValueChanged("Coord Type", new_coord, SixthPoint);
                    paramHandler.OnComplexValueChanged("Tool", new_tcp, SixthPoint);

                    //	paramHandler.OnComplexValueChanged("Tool", new_tcp, Seventh);
                    paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, SeventhPoint);
                    paramHandler.OnComplexValueChanged("Speed", new_speed, SeventhPoint);
                    paramHandler.OnComplexValueChanged("Accel", new_accel, SeventhPoint);
                    paramHandler.OnComplexValueChanged("Blend", new_blend, SeventhPoint);
                    paramHandler.OnComplexValueChanged("Coord Type", new_coord, SeventhPoint);
                    paramHandler.OnComplexValueChanged("Tool", new_tcp, SeventhPoint);

                    // Save the second point to close the gripper		
                    TxRoboticViaLocationOperation Waypoint =  TxApplication.ActiveDocument.
                    GetObjectsByName("point2")[0] as TxRoboticViaLocationOperation;

                    // Save the gripper "Camozzi gripper" 	
                    ITxObject Gripper = TxApplication.ActiveDocument.
                    GetObjectsByName("Robotiq_hande_Bonserio")[0] as TxGripper;
                    
                    // Save the pose "Gripper Closed"  		
                    ITxObject Pose = TxApplication.ActiveDocument.
                    GetObjectsByName("CLOSE")[0] as TxPose;
                    
                    // Save the reference frame of the gripper 		
                    ITxObject tGripper = TxApplication.ActiveDocument.
                    GetObjectsByName("tf_T_gripper")[0] as TxFrame;
                    
                    // Create an array called "elements" and the command to be written in it
                    ArrayList elements1 = new ArrayList();
                    ArrayList elements2 = new ArrayList();
                    ArrayList elements3 = new ArrayList();
                    ArrayList elements4 = new ArrayList();
                    ArrayList elements5 = new ArrayList();
                
                    var myCmd1 = new TxRoboticCompositeCommandStringElement("# Destination");
                    var myCmd11 = new TxRoboticCompositeCommandTxObjectElement(Gripper);

                    var myCmd2 = new TxRoboticCompositeCommandStringElement("# Drive");
                    var myCmd21 = new TxRoboticCompositeCommandTxObjectElement(Pose);

                    var myCmd3 = new TxRoboticCompositeCommandStringElement("# Destination");
                    var myCmd31 = new TxRoboticCompositeCommandTxObjectElement(Gripper);

                    var myCmd4 = new TxRoboticCompositeCommandStringElement("# WaitDevice");
                    var myCmd41 = new TxRoboticCompositeCommandTxObjectElement(Pose);

                    var myCmd5 = new TxRoboticCompositeCommandStringElement("# Grip");
                    var myCmd51 = new TxRoboticCompositeCommandTxObjectElement(tGripper);
                
                    // First line of command	
                    elements1.Add(myCmd1);
                    elements1.Add(myCmd11);
                    
                    TxRoboticCompositeCommandCreationData txRoboticCompositeCommandCreationData1 =
                    new TxRoboticCompositeCommandCreationData(elements1);
                
                    Waypoint.CreateCompositeCommand(txRoboticCompositeCommandCreationData1);
                    
                    // Second line of command
                    elements2.Add(myCmd2);
                    elements2.Add(myCmd21);

                    TxRoboticCompositeCommandCreationData txRoboticCompositeCommandCreationData2 =
                    new TxRoboticCompositeCommandCreationData(elements2);
                
                    Waypoint.CreateCompositeCommand(txRoboticCompositeCommandCreationData2);
                    
                    // Third line of command
                    elements3.Add(myCmd3);
                    elements3.Add(myCmd31);

                    TxRoboticCompositeCommandCreationData txRoboticCompositeCommandCreationData3 =
                    new TxRoboticCompositeCommandCreationData(elements3);
                
                    Waypoint.CreateCompositeCommand(txRoboticCompositeCommandCreationData3);
                    
                    // Fourth line of command
                    elements4.Add(myCmd4);
                    elements4.Add(myCmd41);

                    TxRoboticCompositeCommandCreationData txRoboticCompositeCommandCreationData4 =
                    new TxRoboticCompositeCommandCreationData(elements4);
                
                    Waypoint.CreateCompositeCommand(txRoboticCompositeCommandCreationData4);
                    
                    // Fifth line of command	
                    elements5.Add(myCmd5);
                    elements5.Add(myCmd51);

                    TxRoboticCompositeCommandCreationData txRoboticCompositeCommandCreationData5 =
                    new TxRoboticCompositeCommandCreationData(elements5);
                
                    Waypoint.CreateCompositeCommand(txRoboticCompositeCommandCreationData5);                    

                    // Save the fifth point to close the gripper		
                    TxRoboticViaLocationOperation Waypoint2 =  TxApplication.ActiveDocument.
                    GetObjectsByName("point5")[0] as TxRoboticViaLocationOperation;

                    // Save the gripper "Camozzi gripper" 	
                    ITxObject Gripper2 = TxApplication.ActiveDocument.
                    GetObjectsByName("Robotiq_hande_Bonserio")[0] as TxGripper;

                    // Save the pose "Gripper Closed"  		
                    ITxObject Pose2 = TxApplication.ActiveDocument.
                    GetObjectsByName("OPEN")[0] as TxPose;
                    
                    // Save the reference frame of the gripper 		
                    ITxObject tGripper2 = TxApplication.ActiveDocument.
                    GetObjectsByName("tf_T_gripper")[0] as TxFrame;

                    // Create an array called "elements" and the command to be written in it
                    ArrayList elements6 = new ArrayList();
                    ArrayList elements7 = new ArrayList();
                    ArrayList elements8 = new ArrayList();
                    ArrayList elements9 = new ArrayList();
                    ArrayList elements10 = new ArrayList();
                
                    var myCmd6 = new TxRoboticCompositeCommandStringElement("# Destination");
                    var myCmd61 = new TxRoboticCompositeCommandTxObjectElement(Gripper2);

                    var myCmd7 = new TxRoboticCompositeCommandStringElement("# Drive");
                    var myCmd71 = new TxRoboticCompositeCommandTxObjectElement(Pose2);

                    var myCmd8 = new TxRoboticCompositeCommandStringElement("# Destination");
                    var myCmd81 = new TxRoboticCompositeCommandTxObjectElement(Gripper2);

                    var myCmd9 = new TxRoboticCompositeCommandStringElement("# WaitDevice");
                    var myCmd91 = new TxRoboticCompositeCommandTxObjectElement(Pose2);

                    var myCmd10 = new TxRoboticCompositeCommandStringElement("# Release");
                    var myCmd101 = new TxRoboticCompositeCommandTxObjectElement(tGripper2);
                
                    // First line of command	
                    elements6.Add(myCmd6);
                    elements6.Add(myCmd61);
                    
                    TxRoboticCompositeCommandCreationData txRoboticCompositeCommandCreationData6 =
                    new TxRoboticCompositeCommandCreationData(elements6);
                
                    Waypoint2.CreateCompositeCommand(txRoboticCompositeCommandCreationData6);
                    
                    // Second line of command
                    elements7.Add(myCmd7);
                    elements7.Add(myCmd71);

                    TxRoboticCompositeCommandCreationData txRoboticCompositeCommandCreationData7 =
                    new TxRoboticCompositeCommandCreationData(elements7);
                
                    Waypoint2.CreateCompositeCommand(txRoboticCompositeCommandCreationData7);
                    
                    // Third line of command
                    elements8.Add(myCmd8);
                    elements8.Add(myCmd81);

                    TxRoboticCompositeCommandCreationData txRoboticCompositeCommandCreationData8 =
                    new TxRoboticCompositeCommandCreationData(elements8);
                
                    Waypoint2.CreateCompositeCommand(txRoboticCompositeCommandCreationData8);
                    
                    // Fourth line of command
                    elements9.Add(myCmd9);
                    elements9.Add(myCmd91);

                    TxRoboticCompositeCommandCreationData txRoboticCompositeCommandCreationData9 =
                    new TxRoboticCompositeCommandCreationData(elements9);
                
                    Waypoint2.CreateCompositeCommand(txRoboticCompositeCommandCreationData9);
                    
                    // Fifth line of command	
                    elements10.Add(myCmd10);
                    elements10.Add(myCmd101);

                    TxRoboticCompositeCommandCreationData txRoboticCompositeCommandCreationData10 =
                    new TxRoboticCompositeCommandCreationData(elements10);
                
                    Waypoint2.CreateCompositeCommand(txRoboticCompositeCommandCreationData10);


                    // select the Robotic Program by name
                    var descendants = TxApplication.ActiveDocument.OperationRoot.GetAllDescendants(new TxTypeFilter(typeof(TxContinuousRoboticOperation)));

                    TxContinuousRoboticOperation op = null;

                    foreach (var descendant in descendants)
                    {
                        if (descendant.Name.Equals("RoboticProgram_"+ ii.ToString() + pos_string))
                        {
                            op = descendant as TxContinuousRoboticOperation;
                            break; // Exit loop after finding the first match
                        }
                    }

                    TxApplication.ActiveDocument.CurrentOperation = op;
                    TxSimulationPlayer player = TxApplication.ActiveDocument.SimulationPlayer;
                    player.Rewind();
                
                    //display the determinant
                    if (!player.IsSimulationRunning())
                    {
                        m_output = output;        
                        player.TimeIntervalReached += new TxSimulationPlayer_TimeIntervalReachedEventHandler(player_TimeIntervalReached);                      
                        player.Play(); // Perform the simulation at the current time step 
                        player.TimeIntervalReached -= new TxSimulationPlayer_TimeIntervalReachedEventHandler(player_TimeIntervalReached);
                    }
                            
                    // Rewind the simulation
                    player.Rewind();
                    output.Write("fine simulazione " + pos_string + "\n");
                    double MeanDeterminant = 100000*determinantSum/determinantCounter;
                    output.Write("determinante medio della simulazione numero " + pos_string + " è di: " + MeanDeterminant.ToString() + "\n");
                    string Time = op.Duration.ToString();
		            output.Write("tempo della simulazione numero " + pos_string + " è di: " + Time + "\n");
                    int fitness_int =(int)MeanDeterminant;
                    fitness[pos]= fitness_int;
                    MyOp.Delete();
                    
                }

                //send fitness values
                
                string fitnes_s = string.Join(",", fitness);
                byte[] fitness_Vec = Encoding.ASCII.GetBytes(fitnes_s);
                stream.Write(fitness_Vec, 0, fitness_Vec.Length);
                output.Write("The fitness is:\n" + fitnes_s + "\n");

                // Separate the display information on the terminal
                output.Write("\n");

                // Send the trigger_end back to Python
                string trigger_end = (ii + 1).ToString();
                byte[] byte_trigger_end = Encoding.ASCII.GetBytes(trigger_end);
                stream.Write(byte_trigger_end, 0, byte_trigger_end.Length);
            }

            // Close all the instances
            stream.Close();
            client.Close();
            server.Stop();
        }
        catch (Exception e)
        {
            output.Write("Exception: " + e.Message);
        }
    }

    // Definition of custom functions   
    static int[,] ReceiveNumpyArray(NetworkStream stream)
    {
        // Receive the shape of the array
        byte[] shapeBuffer = new byte[8]; // Assuming the shape is of two int32 values
        stream.Read(shapeBuffer, 0, shapeBuffer.Length);
        int rows = BitConverter.ToInt32(shapeBuffer, 0);
        int cols = BitConverter.ToInt32(shapeBuffer, 4);

        // Receive the array data
        int arraySize = rows * cols * sizeof(int); // Assuming int32 values
        byte[] arrayBuffer = new byte[arraySize];
        stream.Read(arrayBuffer, 0, arrayBuffer.Length);

        // Convert byte array to int array
        int[,] array = new int[rows, cols];
        Buffer.BlockCopy(arrayBuffer, 0, array, 0, arrayBuffer.Length);

        return array;
    }

    static void PrintArray(int[,] array, StringWriter m_output)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                m_output.Write(array[i, j] + " ");
            }
            m_output.Write("\n");
        }
    }

    // Custom method to calculate the cross product of two vectors
    static double[] CrossProduct(double[] vectorA, double[] vectorB)
    {
        // Check that both vectors are 3x1
        if (vectorA.Length != 3 || vectorB.Length != 3)
        {
            throw new ArgumentException("Vectors must be of length 3.");
        }

        // Compute the cross product
        double[] result = new double[3];
        result[0] = vectorA[1] * vectorB[2] - vectorA[2] * vectorB[1];
        result[1] = vectorA[2] * vectorB[0] - vectorA[0] * vectorB[2];
        result[2] = vectorA[0] * vectorB[1] - vectorA[1] * vectorB[0];

        return result;
    }

    // Custom method to calculate the determinant
    static double CalculateDeterminant(double[,] matrix)
    {
        // Check if the matrix is square
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        if (rows != cols || rows != 6)
        {
            throw new ArgumentException("Matrix must be a square 6x6 matrix to calculate determinant.");
        }

        // Calculate the determinant by calling the custom method 'RecursiveDeterminant'
        double determinant = RecursiveDeterminant(matrix);

        return determinant;
    }

    // Custom method to recursively calculate the determinant
    static double RecursiveDeterminant(double[,] matrix)
    {
        int size = matrix.GetLength(0);

        // Base case: for a 1x1 matrix, the determinant is the single element
        if (size == 1)
        {
            return matrix[0, 0];
        }

        double result = 0.0;

        // Expand along the first row
        for (int j = 0; j < size; j++)
        {
            // Calculate the minor matrix
            double[,] minorMatrix = new double[size - 1, size - 1];
            for (int k = 1; k < size; k++)
            {
                for (int l = 0, m = 0; l < size; l++)
                {
                    if (l != j)
                    {
                        minorMatrix[k - 1, m++] = matrix[k, l];
                    }
                }
            }

            // Calculate the cofactor and recursively compute the determinant
            double cofactor = matrix[0, j] * RecursiveDeterminant(minorMatrix);

            // Alternate signs for each element in the row
            result += (j % 2 == 0 ? 1 : -1) * cofactor;
        }

        return result;
    }

    // Define a method to display the value of the determinant during the simulation
    private static void player_TimeIntervalReached(object sender, TxSimulationPlayer_TimeIntervalReachedEventArgs args)
    {
        // Ground
        TxFrame DH0 = TxApplication.ActiveDocument.GetObjectsByName("BASEFRAME")[0] as TxFrame;
        var Frame0 = new TxTransformation(DH0.LocationRelativeToWorkingFrame);

        // Joint 1 (Base)
        TxFrame DH1 = TxApplication.ActiveDocument.GetObjectsByName("fr1")[0] as TxFrame;
        var Frame1 = new TxTransformation(DH1.LocationRelativeToWorkingFrame);

        // Joint 2 (Shoulder)
        TxFrame DH2 = TxApplication.ActiveDocument.GetObjectsByName("fr2")[0] as TxFrame;
        var Frame2 = new TxTransformation(DH2.LocationRelativeToWorkingFrame);

        // Joint 3 (Elbow)
        TxFrame DH3 = TxApplication.ActiveDocument.GetObjectsByName("fr3")[0] as TxFrame;
        var Frame3 = new TxTransformation(DH3.LocationRelativeToWorkingFrame);

        // Joint 4 (Wrist 1)
        TxFrame DH4 = TxApplication.ActiveDocument.GetObjectsByName("fr4")[0] as TxFrame;
        var Frame4 = new TxTransformation(DH4.LocationRelativeToWorkingFrame);

        // Joint 5 (Wrist 2)
        TxFrame DH5 = TxApplication.ActiveDocument.GetObjectsByName("fr5")[0] as TxFrame;
        var Frame5 = new TxTransformation(DH5.LocationRelativeToWorkingFrame);

        // Joint 6 (Wrist 3)
        TxFrame DH6 = TxApplication.ActiveDocument.GetObjectsByName("TOOLFRAME")[0] as TxFrame;
        var Frame6 = new TxTransformation(DH6.LocationRelativeToWorkingFrame);

        // Store the x, y, z coordinates to create vectors (They need to be in meters!)
        var x1 = Frame1[0, 3] / 1000;
        var y1 = Frame1[1, 3] / 1000;
        var z1 = Frame1[2, 3] / 1000;

        var x2 = Frame2[0, 3] / 1000;
        var y2 = Frame2[1, 3] / 1000;
        var z2 = Frame2[2, 3] / 1000;

        var x3 = Frame3[0, 3] / 1000;
        var y3 = Frame3[1, 3] / 1000;
        var z3 = Frame3[2, 3] / 1000;

        var x4 = Frame4[0, 3] / 1000;
        var y4 = Frame4[1, 3] / 1000;
        var z4 = Frame4[2, 3] / 1000;

        var x5 = Frame5[0, 3] / 1000;
        var y5 = Frame5[1, 3] / 1000;
        var z5 = Frame5[2, 3] / 1000;

        var x6 = Frame6[0, 3] / 1000;
        var y6 = Frame6[1, 3] / 1000;
        var z6 = Frame6[2, 3] / 1000;

        // Define vectors z (3 columns of the homogeneous matrices stored in Framei)
        double[] Z0 = { Frame1[0, 2], Frame1[1, 2], Frame1[2, 2] };
        double[] Z1 = { Frame2[0, 2], Frame2[1, 2], Frame2[2, 2] };
        double[] Z2 = { Frame3[0, 2], Frame3[1, 2], Frame3[2, 2] };
        double[] Z3 = { Frame4[0, 2], Frame4[1, 2], Frame4[2, 2] };
        double[] Z4 = { Frame5[0, 2], Frame5[1, 2], Frame5[2, 2] };
        double[] Z5 = { Frame6[0, 2], Frame6[1, 2], Frame6[2, 2] };

        // Define the position vectors
        double[] p0 = { 0.0, 0.0, 0.0 };
        double[] p1 = { x1, y1, z1 };
        double[] p2 = { x2, y2, z2 };
        double[] p3 = { x3, y3, z3 };
        double[] p4 = { x4, y4, z4 };
        double[] p5 = { x5, y5, z5 };
        double[] p6 = { x6, y6, z6 };
        double[] p = { x6, y6, z6 };

        // Subtract the vectors (needed in the next step)
        double[] Pp0 = { p[0] - p1[0], p[1] - p1[1], p[2] - p1[2] };
        double[] Pp1 = { p[0] - p2[0], p[1] - p2[1], p[2] - p2[2] };
        double[] Pp2 = { p[0] - p3[0], p[1] - p3[1], p[2] - p3[2] };
        double[] Pp3 = { p[0] - p4[0], p[1] - p4[1], p[2] - p4[2] };
        double[] Pp4 = { p[0] - p5[0], p[1] - p5[1], p[2] - p5[2] };
        double[] Pp5 = { p[0] - p6[0], p[1] - p6[1], p[2] - p6[2] };

        // Compute the cross products by calling the custom method 'CrossProduct'
        double[] result0 = CrossProduct(Z0, Pp0);
        double[] result1 = CrossProduct(Z1, Pp1);
        double[] result2 = CrossProduct(Z2, Pp2);
        double[] result3 = CrossProduct(Z3, Pp3);
        double[] result4 = CrossProduct(Z4, Pp4);
        double[] result5 = CrossProduct(Z5, Pp5);

        // Create the Jacobian matrix (6x6 for the UR5e)
        double[,] matrixData = {
            { result0[0], result1[0], result2[0], result3[0], result4[0], result5[0] },
            { result0[1], result1[1], result2[1], result3[1], result4[1], result5[1] },
            { result0[2], result1[2], result2[2], result3[2], result4[2], result5[2] },
            { Z0[0], Z1[0], Z2[0], Z3[0], Z4[0], Z5[0] },
            { Z0[1], Z1[1], Z2[1], Z3[1], Z4[1], Z5[1] },
            { Z0[2], Z1[2], Z2[2], Z3[2], Z4[2], Z5[2] }
        };

        // Calculate the determinant by calling the custom method 'CalculateDeterminant'
        double determinant = CalculateDeterminant(matrixData);

        determinantSum = determinantSum + determinant;
        determinantCounter = determinantCounter + 1;

        // Display the current value of the determinant
        // m_output.Write(determinant.ToString() + m_output.NewLine);


    }
}
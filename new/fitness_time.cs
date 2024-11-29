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
                    TxContinuousRoboticOperation MyOp = allOps[allOps.Count-1] as TxContinuousRoboticOperation; // The index may change
                    

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

                    //	paramHandler.OnComplexValueChanged("Tool", new_tcp, FourthPoint);
                    paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, FourthPoint);
                    paramHandler.OnComplexValueChanged("Speed", new_speed, FourthPoint);
                    paramHandler.OnComplexValueChanged("Accel", new_accel, FourthPoint);
                    paramHandler.OnComplexValueChanged("Blend", new_blend, FourthPoint);
                    paramHandler.OnComplexValueChanged("Coord Type", new_coord, FourthPoint);

                    //	paramHandler.OnComplexValueChanged("Tool", new_tcp, FifthPoint);
                    paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, FifthPoint);
                    paramHandler.OnComplexValueChanged("Speed", new_speed, FifthPoint);
                    paramHandler.OnComplexValueChanged("Accel", new_accel, FifthPoint);
                    paramHandler.OnComplexValueChanged("Blend", new_blend, FifthPoint);
                    paramHandler.OnComplexValueChanged("Coord Type", new_coord, FifthPoint);

                    //	paramHandler.OnComplexValueChanged("Tool", new_tcp, SixthPoint);
                    paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, SixthPoint);
                    paramHandler.OnComplexValueChanged("Speed", new_speed, SixthPoint);
                    paramHandler.OnComplexValueChanged("Accel", new_accel, SixthPoint);
                    paramHandler.OnComplexValueChanged("Blend", new_blend, SixthPoint);
                    paramHandler.OnComplexValueChanged("Coord Type", new_coord, SixthPoint);

                    //	paramHandler.OnComplexValueChanged("Tool", new_tcp, Seventh);
                   paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, SeventhPoint);
                    paramHandler.OnComplexValueChanged("Speed", new_speed, SeventhPoint);
                    paramHandler.OnComplexValueChanged("Accel", new_accel, SeventhPoint);
                    paramHandler.OnComplexValueChanged("Blend", new_blend, SeventhPoint);
                    paramHandler.OnComplexValueChanged("Coord Type", new_coord, SeventhPoint);

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
                    player.Play(); // Perform the simulation at the current time step 
                           
                    // Rewind the simulation
                    player.Rewind();
                    output.Write("fine simulazione " + pos_string + "\n");
                    string Time_string = op.Duration.ToString();
		            output.Write("tempo della simulazione numero " + pos_string + " è di: " + Time_string + "\n");
                    double time = op.Duration;
                    double time_inv=10000/time;
                    int fitness_int =(int)time_inv;
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
   
}
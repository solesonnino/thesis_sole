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
        int N=15;
        
        try
        {
            

            for (int i=0; i<N; i++)
            {
                
             

                //ricevi pos place


                //ricevi pick
                var pick= 0;
                           
                //fai calcoli 
                //move the base of the robot in the defined position 
                                        int fitness_int=0;
                                        TxObjectList selectedObjects = TxApplication.ActiveSelection.GetItems();
                                        selectedObjects = TxApplication.ActiveDocument.GetObjectsByName("GoFa12");
                                        var robot = selectedObjects[0] as ITxLocatableObject;
                                        double move_X_Val=-153;
                                        
                                        var position = new TxTransformation(robot.LocationRelativeToWorkingFrame);
                                        position.Translation = new TxVector(move_X_Val, 0, 0);
                                        robot.LocationRelativeToWorkingFrame = position;
                                        TxApplication.RefreshDisplay();

                                        determinantCounter=0;
                                        determinantSum=0;
                                    
                                        // move along x axis 
                                        
                                        // Define some variables
                           
                                        string operation_name = "RoboticProgram" ;

                                        
                                        string new_tcp = "tgripper_tf";
                                        string new_motion_type = "PTP";
                                        string new_speed = "100%";
                                        string new_accel = "100%";
                                        string new_blend = "fine";
                                        //string new_coord = "Cartesian";
                                        
                                        bool verbose = false; // Controls some display options
                                    
                                        // Save the robot (the index may change)  	
                                        TxObjectList objects = TxApplication.ActiveDocument.GetObjectsByName("GoFa12");
                                        var robot2 = objects[0] as TxRobot;

                                        // Get the object to attach to the tool (and the tool)
		                                ITxObject considered_item = TxApplication.ActiveDocument.
		                                GetObjectsByName("Cube_0"+pick.ToString())[0];

		                                ITxObject tool = TxApplication.ActiveDocument.
		                                GetObjectsByName("Suction cup")[0];
                                            
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
                                        
                                        // define the to be picked and the pick point
                                        TxObjectList selectedObject = TxApplication.ActiveSelection.GetItems();
                                        selectedObject = TxApplication.ActiveDocument.GetObjectsByName("Cube_0"+pick.ToString());
                                        var Cube = selectedObject[0] as ITxLocatableObject;
                                        var cube_name = Cube.Name; // Save the name of the object

                                        // the pick point is on the top face of the object (vacuum gripper) --> translate the pick point
                                        var pick_point = new TxVector(Cube.LocationRelativeToWorkingFrame.Translation);

                                        

                                        //define the place point of the object --> varies depending on the object
                                        var place_point_x=-863; 
                                        var place_point_y=-455; 
                                        var place_point_z=-47;
                                        var place_point = new TxVector (place_point_x, place_point_y, place_point_z);

                                        //define the point above the pick/place point
                                        var zoffset = new TxVector(0, 0, 100);

                                        //save the initial position of the tcp in EighthPoint
                                        TxFrame TCPpose1 = TxApplication.ActiveDocument.GetObjectsByName("TOOLFRAME")[0] as TxFrame;
                                        var TCP_pose1 = new TxTransformation(TCPpose1.LocationRelativeToWorkingFrame); 
                                        EighthPoint.LocationRelativeToWorkingFrame = TCP_pose1;

                                        // Impose a position to the new waypoint		
                                        double rotVal = Math.PI;
                                        TxTransformation rotX = new TxTransformation(new TxVector(rotVal, 0, 0), 
                                        TxTransformation.TxRotationType.RPY_XYZ);
                                        FirstPoint.AbsoluteLocation = rotX;
                                        
                                        var pointA = new TxTransformation(FirstPoint.AbsoluteLocation);
                                        pointA.Translation = new TxVector(pick_point + zoffset);
                                        FirstPoint.AbsoluteLocation = pointA;
                                        
                                        // Impose a position to the second waypoint		
                                        double rotVal2 = Math.PI;
                                        TxTransformation rotX2 = new TxTransformation(new TxVector(rotVal2, 0, 0), 
                                        TxTransformation.TxRotationType.RPY_XYZ);
                                        SecondPoint.AbsoluteLocation = rotX2;
                                        
                                        var pointB = new TxTransformation(SecondPoint.AbsoluteLocation);
                                        pointB.Translation = new TxVector(pick_point);
                                        SecondPoint.AbsoluteLocation = pointB;
                                    
                                        
                                        // Impose a position to the third waypoint		
                                        double rotVal3 = Math.PI;
                                        TxTransformation rotX3 = new TxTransformation(new TxVector(rotVal3, 0, 0), 
                                        TxTransformation.TxRotationType.RPY_XYZ);
                                        ThirdPoint.AbsoluteLocation = rotX3;
                                        
                                        var pointC = new TxTransformation(ThirdPoint.AbsoluteLocation);
                                        pointC.Translation = new TxVector(pick_point + zoffset);
                                        ThirdPoint.AbsoluteLocation = pointC;

                                        // Impose a position to the fourth waypoint		
                                        double rotVal4 = Math.PI;
                                        TxTransformation rotX4 = new TxTransformation(new TxVector(rotVal4, 0, 0), 
                                        TxTransformation.TxRotationType.RPY_XYZ);
                                        FourthPoint.AbsoluteLocation = rotX4;
                                        
                                        var pointD = new TxTransformation(FourthPoint.AbsoluteLocation);
                                        pointD.Translation = new TxVector(place_point + zoffset);
                                        FourthPoint.AbsoluteLocation = pointD;

                                        // Impose a position to the fifth waypoint		
                                        double rotVal5 = Math.PI;
                                        double rot_z_place=0;
                                        
                                        TxTransformation rotX5 = new TxTransformation(new TxVector(rotVal5, 0, rot_z_place), 
                                        TxTransformation.TxRotationType.RPY_XYZ);
                                        FifthPoint.AbsoluteLocation = rotX5;
                                        
                                        var pointE = new TxTransformation(FifthPoint.AbsoluteLocation);
                                        pointE.Translation = new TxVector(place_point);
                                        FifthPoint.AbsoluteLocation = pointE;

                                        // Impose a position to the sixth waypoint		
                                        double rotVal6 = Math.PI;
                                        
                                        
                                        TxTransformation rotX6 = new TxTransformation(new TxVector(rotVal6, 0, 0), 
                                        TxTransformation.TxRotationType.RPY_XYZ);
                                        SixthPoint.AbsoluteLocation = rotX6;
                                        
                                        var pointF = new TxTransformation(SixthPoint.AbsoluteLocation);
                                        pointF.Translation = new TxVector(place_point + zoffset);
                                        SixthPoint.AbsoluteLocation = pointF;

                                        // Impose a position to the seventh waypoint --> go back to initial position		
                                        SeventhPoint.AbsoluteLocation = EighthPoint.LocationRelativeToWorkingFrame;

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
                                        paramHandler.OnComplexValueChanged("Tool Frame", new_tcp, FirstPoint);
                                        paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, FirstPoint);
                                        paramHandler.OnComplexValueChanged("Speed", new_speed, FirstPoint);
                                        paramHandler.OnComplexValueChanged("Acc", new_accel, FirstPoint);
                                        paramHandler.OnComplexValueChanged("Zone", new_blend, FirstPoint);
                                        
                                        paramHandler.OnComplexValueChanged("Tool Frame", new_tcp, SecondPoint);
                                        paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, SecondPoint);
                                        paramHandler.OnComplexValueChanged("Speed", new_speed, SecondPoint);
                                        paramHandler.OnComplexValueChanged("Acc", new_accel, SecondPoint);
                                        paramHandler.OnComplexValueChanged("Zone", new_blend, SecondPoint);
                                        
                                        paramHandler.OnComplexValueChanged("Tool Frame", new_tcp, ThirdPoint);
                                        paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, ThirdPoint);
                                        paramHandler.OnComplexValueChanged("Speed", new_speed, ThirdPoint);
                                        paramHandler.OnComplexValueChanged("Acc", new_accel, ThirdPoint);
                                        paramHandler.OnComplexValueChanged("Zone", new_blend, ThirdPoint);

                                        paramHandler.OnComplexValueChanged("Tool Frame", new_tcp, FourthPoint);
                                        paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, FourthPoint);
                                        paramHandler.OnComplexValueChanged("Speed", new_speed, FourthPoint);
                                        paramHandler.OnComplexValueChanged("Acc", new_accel, FourthPoint);
                                        paramHandler.OnComplexValueChanged("Zone", new_blend, FourthPoint);


                                        paramHandler.OnComplexValueChanged("Tool Frame", new_tcp, FifthPoint);
                                        paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, FifthPoint);
                                        paramHandler.OnComplexValueChanged("Speed", new_speed, FifthPoint);
                                        paramHandler.OnComplexValueChanged("Acc", new_accel, FifthPoint);
                                        paramHandler.OnComplexValueChanged("Zone", new_blend, FifthPoint);

                                        paramHandler.OnComplexValueChanged("Tool Frame", new_tcp, SixthPoint);
                                        paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, SixthPoint);
                                        paramHandler.OnComplexValueChanged("Speed", new_speed, SixthPoint);
                                        paramHandler.OnComplexValueChanged("Acc", new_accel, SixthPoint);
                                        paramHandler.OnComplexValueChanged("Zone", new_blend, SixthPoint);

                                        paramHandler.OnComplexValueChanged("Tool Frame", new_tcp, SeventhPoint);
                                        paramHandler.OnComplexValueChanged("Motion Type", new_motion_type, SeventhPoint);
                                        paramHandler.OnComplexValueChanged("Speed", new_speed, SeventhPoint);
                                        paramHandler.OnComplexValueChanged("Acc", new_accel, SeventhPoint);
                                        paramHandler.OnComplexValueChanged("Zone", new_blend, SeventhPoint);


                                        // Choose the point for the 'attach'
                                        TxRoboticViaLocationOperation Waypoint1 =  TxApplication.ActiveDocument.
                                        GetObjectsByName("point2")[0] as TxRoboticViaLocationOperation;

                                        // Choose the point for the 'detach'
                                        TxRoboticViaLocationOperation Waypoint2 =  TxApplication.ActiveDocument.
                                        GetObjectsByName("point5")[0] as TxRoboticViaLocationOperation;

                                        // Create the OLP command for attachment
                                        ArrayList elements1 = new ArrayList();
                                        ArrayList elements2 = new ArrayList();
                                    
                                        var myCmd1 = new TxRoboticCompositeCommandStringElement("# Attach ");	
                                        var myCmd11 = new TxRoboticCompositeCommandTxObjectElement(considered_item);
                                        var myCmd111 = new TxRoboticCompositeCommandTxObjectElement(tool);

                                        // Append all the command	
                                        elements1.Add(myCmd1);  
                                        elements1.Add(myCmd11);  
                                        elements1.Add(myCmd111); 

                                        // Write the command 	
                                        TxRoboticCompositeCommandCreationData txRoboticCompositeCommandCreationData1 =
                                        new TxRoboticCompositeCommandCreationData(elements1);	
                                        Waypoint1.CreateCompositeCommand(txRoboticCompositeCommandCreationData1);	

                                        // Create the OLP command for detachment
                                        var myCmd2 = new TxRoboticCompositeCommandStringElement("# Detach ");	
                                        var myCmd21 = new TxRoboticCompositeCommandTxObjectElement(considered_item);

                                        // Append all the command	
                                        elements2.Add(myCmd2);  
                                        elements2.Add(myCmd21);  


                                        // Write the command 	
                                        TxRoboticCompositeCommandCreationData txRoboticCompositeCommandCreationData2 =
                                        new TxRoboticCompositeCommandCreationData(elements2);	
                                        Waypoint2.CreateCompositeCommand(txRoboticCompositeCommandCreationData2);


                                        // select the Robotic Program by name
                                        var descendants = TxApplication.ActiveDocument.OperationRoot.GetAllDescendants(new TxTypeFilter(typeof(TxContinuousRoboticOperation)));

                                        TxContinuousRoboticOperation op = null;

                                        foreach (var descendant in descendants)
                                        {
                                            if (descendant.Name.Equals("RoboticProgram"))
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
                                       
                                        double MeanDeterminant = 100000000000000000*determinantSum/determinantCounter;
                                        if (MeanDeterminant<0)
                                        {
                                            MeanDeterminant= - MeanDeterminant;
                                        }

                                        int fitness_int_partial =(int)MeanDeterminant;
                                        fitness_int= fitness_int+fitness_int_partial;
                                        output.Write(fitness_int.ToString() + "\n");
                                        MyOp.Delete();


            }

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

        // Create the Jacobian matrix (6x6 for the GoFa12)
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
        if (determinant<0)
        {
            determinant=-determinant;
        }

        determinantSum = determinantSum + determinant;
        determinantCounter = determinantCounter + 1;

        // Display the current value of the determinant
        // m_output.Write(determinant.ToString() + m_output.NewLine);

    }

}
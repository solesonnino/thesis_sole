// Copyright 2019 Siemens Industry Software Ltd.
using System;
using System.IO;
using System.Windows.Forms;
using Tecnomatix.Engineering;

public class MainScript
{
	public static void Main(ref StringWriter output)
    {
		TxObjectList objects = TxApplication.ActiveDocument.GetObjectsByName("UR5e");
                    var robot2 = objects[0] as TxRobot;
                        
                    // Create the new operation    	
                    TxContinuousRoboticOperationCreationData data = new TxContinuousRoboticOperationCreationData("operation1");
                    TxApplication.ActiveDocument.OperationRoot.CreateContinuousRoboticOperation(data);
                    
                    // Get the created operation
                    TxTypeFilter opFilter = new TxTypeFilter(typeof(TxContinuousRoboticOperation));
                    TxOperationRoot opRoot = TxApplication.ActiveDocument.OperationRoot;
                            
                    TxObjectList allOps = opRoot.GetAllDescendants(opFilter);
                    TxContinuousRoboticOperation MyOp = allOps[0] as TxContinuousRoboticOperation; // The index may changeTxRoboticViaLocationOperationCreationData Point1 = new TxRobotic
                    TxRoboticViaLocationOperationCreationData Point1 = new TxRoboticViaLocationOperationCreationData();
                    Point1.Name = "point1"; // First point
                    TxRoboticViaLocationOperation FirstPoint = MyOp.CreateRoboticViaLocationOperation(Point1);
        TxFrame TCPpose1 = TxApplication.ActiveDocument.GetObjectsByName("TCPF")[0] as TxFrame;
        var TCP_pose1 = new TxTransformation(TCPpose1.LocationRelativeToWorkingFrame); 
        FirstPoint.LocationRelativeToWorkingFrame = TCP_pose1;
     }   
}

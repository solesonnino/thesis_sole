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

class Program
{
    static StringWriter m_output;

    static double determinantSum = 0;
    static double determinantCounter = 0;
    static public void Main(ref StringWriter output)
    {
        TcpListener server = null;
        try
        {
            // Define the number of simulations
            int Nsim = 4;
            int port = 12345;

            // Start listening for incoming connections
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            server.Start();
            output.Write("Server started...");

            // Accept a client connection
            TcpClient client = server.AcceptTcpClient();
            NetworkStream stream = client.GetStream();

            // Loop for all the simulations
            for (int ii = 0; ii < Nsim - 1; ii++)
            {
                // Receive positions array
                var layout = ReceiveNumpyArray(stream);
                output.Write("positions: \n");
                PrintArray(layout, output);
                output.Write("\n");

                //select each position 
                for (int pos=0; pos < 4; pos++)
                {
                    //move the base of the robot in the defined position 
                    // move along x axis 
				  TxObjectList selectedObjects = TxApplication.ActiveSelection.GetItems();
				 selectedObjects = TxApplication.ActiveDocument.GetObjectsByName("UR5e");
				 var robot = selectedObjects[1] as ITxLocatableObject;
                 double move_X_Val=0;
                 move_X_Val= layout[0, pos];	
				 var position = new TxTransformation(robot.LocationRelativeToWorkingFrame);
				  position.Translation = new TxVector(move_X_Val, 0, 0);
				 robot.LocationRelativeToWorkingFrame = position;
				 TxApplication.RefreshDisplay();
				 output.Write("the current position is: \n");
				 output.Write(move_X_Val.ToString());
				 output.Write("\n");
                }

                //send fitness values
                //int Num = ii;
                int[] fitness = { 1, 2, 3, 4, 5 };
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
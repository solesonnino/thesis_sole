// Import libraries (.dll files)
 
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Tecnomatix.Engineering;
 
class Program
{
    static public void Main(ref StringWriter output)
    {
        TcpListener server = null;
 
        /* 
         * Definition of the 'try - catch' architecture:
         * First, the code inside 'try' is executed
         * If no exception was found, the 'catch' block is never executed
         * If an exception was found, the control is passed to that block
            a) This catch block can handle exceptions of type 'Exception' (so it can catch any type of them)
            b) The caught exception is represented by the variable 'e'
            c) The line 'e.Message' reterives the error message
        */
 
        try
        {
            int max_iterations = 5;
        
 
            var ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 12345;
            server = new TcpListener(ipAddress, port);
            server.Start();
 
            // Display the message saying that the server is waiting for the client
			output.Write("The C# client is waiting the Python server to connect ...\n");
            TcpClient client = server.AcceptTcpClient();
 
            // If the client successfully connected, print a message
 
			output.Write("Connection successfully established with the Python server!.\n");
            /*
             * In this part, the first API-guided simulation is performed: time and RULA are the results
            */
 
            /*
             * This part is the core of the process and it allows to run automatic simulations until an upper bound (Nsim) is reached
            */

            for (int ii = 1; ii <= max_iterations; ii++)
            {
 
            
                int[] kpis = { 1, 2 }; // pack the KPIs
                string data = string.Join(",", kpis); // convert tthe array into a string
                NetworkStream stream1 = client.GetStream(); // open the first stream 
                byte[] kpi_vec = Encoding.ASCII.GetBytes(data); // ASCII encoding              
                stream1.Write(kpi_vec, 0, kpi_vec.Length); // Write on the stream
                output.Write("The Key Performance Indicator(s) sent to Python are:\n");
                output.Write(data.ToString());
                output.Write("\n");
         
 
                // b) Get the new 'tentative' layout to run the new simulation
                output.Write("sto aspettando le particelle\n");
                var receivedArray = ReceiveNumpyArray(client); // static method defined below
                output.Write("The particles are: \n");
				output.Write(ArrayToString(receivedArray));
                //output.Write(receivedArray[0], receivedArray[1], receivedArray[2], receivedArray[3], receivedArray[4], receivedArray[5], receivedArray[6], receivedArray[7]);
				output.Write("\n");
				
				// move along x axis 
				/* TxObjectList selectedObjects = TxApplication.ActiveSelection.GetItems();
				selectedObjects = TxApplication.ActiveDocument.GetObjectsByName("UR5e");
				var robot = selectedObjects[0] as ITxLocatableObject;
	
				int move_X_Val= (ii)*20;	
				var position = new TxTransformation(robot.LocationRelativeToWorkingFrame);
				position.Translation = new TxVector(move_X_Val, 0, 0);
				robot.LocationRelativeToWorkingFrame = position;
				TxApplication.RefreshDisplay();
				output.Write("the current position is: \n");
				output.Write(move_X_Val.ToString());
				output.Write("\n"); 
				*/
               

                // compute fitness and send it back to the python  code
                /*output.Write("computing fitness\n");
                int Num = ii;
                int[] fitness = { Num, 2, 3, 4, 5 }; // pack the KPIs
                output.Write("packing fitness\n");
                string fitnes_s = string.Join(",", fitness); // convert tthe array into a string
                output.Write("opening the stream\n");
                NetworkStream stream2 = client.GetStream(); // open the first stream 
                output.Write("ascii encoding...\n");
                byte[] fitness_Vec = Encoding.ASCII.GetBytes(fitnes_s); // ASCII encoding              
                stream2.Write(fitness_Vec, 0, fitness_Vec.Length); // Write on the stream
                output.Write("the fitness is:\n");
                output.Write(fitnes_s.ToString());
                output.Write("\n");*/
 
                
 
                // c) Send the varible trigger_end to python
 
                /*string trigger_end = ii.ToString(); // convert the current iteration index to string
                NetworkStream stream3 = client.GetStream(); // open the second stream
                byte[] byte_trigger_end = Encoding.ASCII.GetBytes(trigger_end); // ASCII encoding           
                stream3.Write(byte_trigger_end, 0, byte_trigger_end.Length); // Write on the stream
                output.Write("The current iteration number is sent to Python and it is equal to:\n");
                output.Write(trigger_end.ToString());
                output.Write("\n");*/
            }
 
            // Close the connection after the 'Nsim' simulations
 
            client.Close();
        }
        catch (Exception e)
        {
 
            // If necessary, write the type of exception found
 
            output.Write("Error: {e.Message}");
        }
    }
 
    // ............................................ Static Methods ......................................... //
 
    // Static method to convert from bytes to array (this method is used inside 'ReceiveNumpyArray')
    static int[] ConvertBytesToIntArray(byte[] bytes, int startIndex)
    {
        // Create an integer array, called 'result', by dividing the length of the vector 'bytes' by 4
 
        int[] result = new int[bytes.Length / 4];
 
        for (int i = 0; i < result.Length; i++) // Loop over all the elements of 'result'
        {
            result[i] = BitConverter.ToInt32(bytes, startIndex + i * 4); // convert a segment of 4 bytes inside 'bytes' into an integer
        }
        return result;
    }
    // Static method to receive a NumPy array from a Python server over a TCP connection
    static int[,] ReceiveNumpyArray(TcpClient client)
    {
        // Obtain the stream to read and write data over the network
 
        NetworkStream stream = client.GetStream();
        /* Receive the shape and data type of the array
         * It's assumed that the shape is represented by two integers, each of 4 bytes (N° rows, N°columns)
         * It's assumed that the data type information is represented by a 4-byte value
        */
 
        
        byte[] shapeBytes = new byte[8]; // create a variable for the two integers defining the shape
        stream.Read(shapeBytes, 0, shapeBytes.Length); // read the shape
        int[] shape = ConvertBytesToIntArray(shapeBytes, 0); // Convert the received shape bytes into an integer array
        // Receive the actual array data. It's important that 'SizeOf' contains the same type (int, in my case) defined besides 'static'
 
        byte[] arrayBytes = new byte[Marshal.SizeOf(typeof(int)) * shape[0] * shape[1]]; // Create a byte array to receive data
        stream.Read(arrayBytes, 0, arrayBytes.Length); // Read data from the network stream
 
        // Convert the received bytes back to a NumPy array. Again, the type (int) must be the same as above
 
        int[,] receivedArray = new int[shape[0], shape[1]]; // Create a 2D array with the received shape
        Buffer.BlockCopy(arrayBytes, 0, receivedArray, 0, arrayBytes.Length); // Copy the received data to 'receivedArray'
 
        // Return the array
 
        return receivedArray;
    }
    // Static method to convert an array into a string
    static string ArrayToString<T>(T[,] array)
    {
 
        // Define number of rows and columns
 
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
 
        // Loop to transform each element into a string
 
        string result = "";
        for (int i = 0; i < rows; i++) // Scan all the rows
        {
            for ( int j = 0; j < cols; j++) // Scan all the columns
            {
                result += array[i, j].ToString() + "\t"; // separate each element with a tab ('\t') with respect to the previous
            }
            result += Environment.NewLine; // Aftre scanning all the elements in the columns, start displaying in the row below
        }
        return result;
    }
}
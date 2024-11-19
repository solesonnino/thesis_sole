using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Tecnomatix.Engineering;

class Manipulability
{
    static public void Main()
    {   
        TcpListener server = null;
        try
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 12345;
            server = new TcpListener(ipAddress, port);
            server.Start();
 
            // Display the message saying that the server is waiting for the client
			output.Write("The C# client is waiting the Python server to connect ...\n");
            TcpClient client = server.AcceptTcpClient();
 
            // If the client successfully connected, print a message
 
			output.Write("Connection successfully established with the Python server!.\n");
            
            client.Close();
        }
            
        
        catch (Exception e)
        {
 
            // If necessary, write the type of exception found
 
            output.Write("Error: {e.Message}");
        }
    }
}

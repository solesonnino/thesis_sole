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

        try
        {
            int max_iterations = 5;
            var ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 12345;
            server = new TcpListener(ipAddress, port);
            server.Start();

            output.Write("The C# client is waiting the Python server to connect ...\n");
            TcpClient client = server.AcceptTcpClient();
            output.Write("Connection successfully established with the Python server!\n");

            for (int ii = 1; ii <= max_iterations; ii++)
            {
                // a) Invio dei KPI
                int[] kpis = { 1, 2 };
                string data = string.Join(",", kpis);
                NetworkStream stream1 = client.GetStream();
                byte[] kpi_vec = Encoding.ASCII.GetBytes(data);
                stream1.Write(kpi_vec, 0, kpi_vec.Length);
                output.Write("The Key Performance Indicator(s) sent to Python are:\n" + data + "\n");

                // b) Ricezione del nuovo layout
                output.Write("sto aspettando le particelle\n");
                var receivedArray = ReceiveNumpyArray(client);
                output.Write("The particles are:\n" + ArrayToString(receivedArray) + "\n");

                // c) Calcolo e invio della fitness
                output.Write("computing fitness\n");
                int Num = ii;
                int[] fitness = { Num, 2, 3, 4, 5, Num };
                string fitnes_s = string.Join(",", fitness);
                NetworkStream stream2 = client.GetStream();
                byte[] fitness_Vec = Encoding.ASCII.GetBytes(fitnes_s);
                stream2.Write(fitness_Vec, 0, fitness_Vec.Length);
                output.Write("The fitness is:\n" + fitnes_s + "\n");

                var aspetta= ReceiveNumpyArray(client);
            }

            client.Close();
        }
        catch (Exception e)
        {
            output.Write("Error: " + e.Message);
        }
    }

    // ............................................ Static Methods ......................................... //

    // Funzione helper per leggere il numero esatto di byte richiesti
    static void ReadExact(NetworkStream stream, byte[] buffer, int length)
    {
        int offset = 0;
        while (offset < length)
        {
            int bytesRead = stream.Read(buffer, offset, length - offset);
            if (bytesRead == 0)
                throw new EndOfStreamException("Connection closed before all bytes were received.");
            offset += bytesRead;
        }
    }

    // Metodo statico per convertire byte in array di int
    static int[] ConvertBytesToIntArray(byte[] bytes, int startIndex)
    {
        int[] result = new int[bytes.Length / 4];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = BitConverter.ToInt32(bytes, startIndex + i * 4);
        }
        return result;
    }

    // Metodo statico per ricevere un array NumPy da un server Python su TCP
    static int[,] ReceiveNumpyArray(TcpClient client)
    {
        NetworkStream stream = client.GetStream();

        byte[] shapeBytes = new byte[8];
        ReadExact(stream, shapeBytes, shapeBytes.Length);
        int[] shape = ConvertBytesToIntArray(shapeBytes, 0);

        byte[] arrayBytes = new byte[Marshal.SizeOf(typeof(int)) * shape[0] * shape[1]];
        ReadExact(stream, arrayBytes, arrayBytes.Length);

        int[,] receivedArray = new int[shape[0], shape[1]];
        Buffer.BlockCopy(arrayBytes, 0, receivedArray, 0, arrayBytes.Length);

        return receivedArray;
    }

    // Metodo statico per convertire un array 2D in stringa per la visualizzazione
    static string ArrayToString<T>(T[,] array)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        string result = "";
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result += array[i, j].ToString() + "\t";
            }
            result += Environment.NewLine;
        }
        return result;
    }
}

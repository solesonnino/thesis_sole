using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class FittingServer
{
    static void Main()
    {
        // Impostazioni di rete per il server
        var ipAddress = IPAddress.Parse("127.0.0.1");
        int port = 12345;  // Porta su cui il server ascolta (modificata rispetto all'esempio originale)
        TcpListener server = new TcpListener(ipAddress, port);

        server.Start();
        Console.WriteLine("The C# server is waiting for the Python client to connect ...");

        // Accetta la connessione del client
        TcpClient client = server.AcceptTcpClient();

        // Conferma connessione stabilita
        Console.WriteLine("Connection successfully established with the Python client!");

        using (NetworkStream stream = client.GetStream())
        using (StreamWriter output = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
        using (StreamReader input = new StreamReader(stream, Encoding.UTF8))
        {
            while (true)
            {
                // Riceve i dati dal client
                string receivedData = input.ReadLine();
                
                // Definisce una variabile locale per memorizzare il valore di x
                if (double.TryParse(receivedData, out double x))
                {
                    Console.WriteLine("Posizione x ricevuta dal client Python: " + x);

                    // Calcola la misura di fitting (esempio: distanza quadratica da un target)
                    double targetPosition = 10.0;  // Puoi cambiare questo valore a piacimento
                    double fittingMeasure = Math.Pow(x - targetPosition, 2);  // Funzione di fitting

                    // Invia il risultato al client Python
                    string response = fittingMeasure.ToString();
                    output.WriteLine(response);

                    Console.WriteLine("Misura di fitting inviata: " + fittingMeasure);
                }
                else
                {
                    Console.WriteLine("Errore: non è stato possibile interpretare la posizione x ricevuta.");
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Server
{
    internal class ServerInit
    {
        //Set port 
        const int port = 10001;

        //Start the server and listen to clients
        internal static void StartServer()
        {
            //Creates Server on Port 10001, accepts "any" ipadress
            var httpServer = new TcpListener(IPAddress.Any, port); //maybe IPAdress.Loopback
            httpServer.Start();
            Console.WriteLine($"The Server is currently running on Port: {port}");

            // Use a separate thread to listen for clients
            Thread listenerThread = new(() => Listen(httpServer));

            listenerThread.Start();
        }
        private static void Listen(TcpListener httpServer)
        {
            while (true)
            {
                var clientSocket = httpServer.AcceptTcpClient();
                // For each new client, create a new thread to handle communication
                Thread clientThread = new(() => Http.HTTPProcessor.ProcessClient(clientSocket));
                clientThread.Start();
            }
        }
    }
}
  

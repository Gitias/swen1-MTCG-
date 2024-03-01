using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using MCTG.Interfaces;

namespace MCTG.Server.Http
{
    internal class HTTPProcessor
    {
        internal static void ProcessClient(TcpClient clientSocket)
        {
            //clientSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            Console.WriteLine("\n <--------------------Start-------------------->");
            IPEndPoint clientEndPoint = (IPEndPoint)clientSocket.Client.RemoteEndPoint;
            string clientInformations = $"{clientEndPoint.Address}:{clientEndPoint.Port}";

            Console.WriteLine($"A new client has connected to the Server: {clientInformations})");

            var writer = new StreamWriter(clientSocket.GetStream()) { AutoFlush = true};
            var reader = new StreamReader(clientSocket.GetStream(), Encoding.UTF8);

            HTTPRequest request = new(reader);
            HTTPResponse response = new(writer);

            try
            {
                while(clientSocket.Connected)
                {
                    //Reads/Processes requests until disconnection of client
                    request.ParseRequ();
                    HTTPMethodHandler.HandleRequest(request, response);
                    response.SendResponse();
                    response.Reset();
                }
            }
            catch (Exception exception)
            {
                if (exception is IException selfmadeEx)
                {
                    Console.WriteLine("\nExepception status: " + selfmadeEx.StatusCode + "\n Exception message: " + selfmadeEx.ErrorMessage);
                }else if (exception.InnerException is IException selfmadeInnerEx)
                {
                    Console.WriteLine("\nExepception status: " + selfmadeInnerEx.StatusCode + "\n Exception message: " + selfmadeInnerEx.ErrorMessage);
                }
                else
                {
                    Console.WriteLine("\n" + exception.Message + "\n" + exception.Source + "\n");
                }
                response.SendResponse();
                response.Reset();

            }
            finally
            {
                clientSocket.Close();
            }
        }
    }
}


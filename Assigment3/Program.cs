using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Assigment3
{
    class Program
    {
        static void Main(string[] args)
        {
            // Start listening on port 5000
            TcpListener listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();
            Console.WriteLine("Server started on port 5000...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected...");

                // Handle client in a separate thread
                Thread thread = new Thread(() => ClientHandler.HandleClient(client));
                thread.Start();
            }
        }
    }
}

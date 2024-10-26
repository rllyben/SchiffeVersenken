using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken
{
    internal class Host
    {
        private static IPAddress localIpAddress;
        private static IPAddress clientIpAddress;
        private static int port;
        private static TcpListener listener; 
        private static TcpClient client;
        private static IPEndPoint clientPlayer;
        public static void HostSetup()
        {
            localIpAddress = IPAddress.Parse(Utils.GetLocalIPAddress());
            port = Utils.GetAvailablePort();
            IPEndPoint ipEndPoint = new(localIpAddress, port);
            listener = new(ipEndPoint);

            Program.PrintTitle();
            Console.WriteLine("Host Game");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Your host IP-Adress and Port is {ipEndPoint}");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Waiting for connection...");

            listener.Start();
            try
            {
                using TcpClient handler = listener.AcceptTcpClient();
                using NetworkStream stream = handler.GetStream();

                var message = "Connected to Host";
                var connectionTestBytes = Encoding.UTF8.GetBytes(message);
                stream.Write(connectionTestBytes);

                var buffer = new byte[1024];
                int received = stream.Read(buffer);

                var clientMessage = Encoding.UTF8.GetString(buffer, 0, received);

                IPAddress clientAddress = IPAddress.Parse(clientMessage);
                clientPlayer = new(clientAddress, port);

            }
            finally
            {
                Console.WriteLine("Player 2 connected successfully!");
                Thread.Sleep(1000);
                Program.HostPlay();
            }

        }
        public static void HostShipPlaceing(string ownPlaceing, char ownDirection)
        {
            using TcpClient handler = listener.AcceptTcpClient();
            try
            {
                using NetworkStream stream = handler.GetStream();

                string allInput = ownPlaceing + "x" + ownDirection;

                var connectionTestBytes = Encoding.UTF8.GetBytes(allInput);
                stream.Write(connectionTestBytes);
            }
            catch
            {
                Console.WriteLine("Something went wrong! Player 2 disconnected?");
                Thread.Sleep(1000);
                while (true)
                {
                    try
                    {
                        using NetworkStream stream = handler.GetStream();

                        string allInput = ownPlaceing + "x" + ownDirection;

                        var connectionTestBytes = Encoding.UTF8.GetBytes(allInput);
                        stream.Write(connectionTestBytes);
                    }
                    catch
                    {
                        Thread.Sleep(500);
                    }

                }

            }

        }
        public static void ClientPlaceing(string ownInput)
        {
            using TcpClient handler = listener.AcceptTcpClient();
            try
            {
                using NetworkStream stream = handler.GetStream();

                string allInput = ownInput;

                var connectionTestBytes = Encoding.UTF8.GetBytes(allInput);
                stream.Write(connectionTestBytes);
            }
            catch
            {
                Console.WriteLine("Something went wrong! Player 2 disconnected?");
                Thread.Sleep(1000);
                while (true)
                {
                    try
                    {
                        using NetworkStream stream = handler.GetStream();

                        string allInput = ownInput;

                        var connectionTestBytes = Encoding.UTF8.GetBytes(allInput);
                        stream.Write(connectionTestBytes);
                    }
                    catch
                    {
                        Thread.Sleep(500);
                    }

                }

            }

        }
        public static string ClientPlaying()
        {
            using TcpClient client = new();
            client.Connect(clientPlayer);
            using NetworkStream stream = client.GetStream();

            var buffer = new byte[1024];
            int received = stream.Read(buffer);

            var clientInput = Encoding.UTF8.GetString(buffer, 0, received);

            return clientInput;
        }

    }

}

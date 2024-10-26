using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken
{
    internal class Client
    {
        private static TcpClient client;
        private static TcpListener listener;
        private static bool error = false;
        private static IPEndPoint hostPlayer;
        private static IPEndPoint clientPlayer;
        public static void Connect()
        {
            Program.PrintTitle();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Join Game");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Please enter the host ip-adress and the port (192.168.0.0:12345)");
            
            string input = Console.ReadLine();
            string[] parts = input.Split(':');
            IPAddress ipAdress = IPAddress.Parse(parts[0]);
            var localIpAddress = IPAddress.Parse(Utils.GetLocalIPAddress());
            int port = int.Parse(parts[1]);
            hostPlayer = new IPEndPoint(ipAdress, port);
            clientPlayer = new IPEndPoint(localIpAddress, port);
            do
            {
                try
                {
                    using TcpClient client = new();
                    client.Connect(hostPlayer);
                    using NetworkStream stream = client.GetStream();

                    var buffer = new byte[1024];
                    int connectionClientTest = stream.Read(buffer);

                    var clientTest = Encoding.UTF8.GetString(buffer, 0, connectionClientTest);

                    if (clientTest == "Connected to Host")
                    {
                        var connectionTestBytes = Encoding.UTF8.GetBytes(localIpAddress.ToString());
                        stream.Write(connectionTestBytes);
                    }
                    else
                        throw new Exception("Connection to host failed");

                    int hostConnectionTest = stream.Read(buffer);
                    var hostTest = Encoding.UTF8.GetString(buffer, 0, hostConnectionTest);

                    if (hostTest == localIpAddress.ToString())
                    {
                        listener = new(hostPlayer);
                        Console.WriteLine("Connected successfully to Host");
                        Thread.Sleep(1000);
                    }
                    else
                        throw new Exception("Connection from host failed");

                    error = false;
                }
                catch
                {
                    Program.PrintTitle();
                    Console.WriteLine("Connection to Host failed!");
                    Thread.Sleep(1000);
                    error = true;
                }

            } while (error);
            Thread.Sleep(1000);
            Program.ClientPlay();
        }
        public static string HostPlaying()
        {
            using TcpClient client = new();
            client.Connect(hostPlayer);
            using NetworkStream stream = client.GetStream();

            var buffer = new byte[1024];
            int received = stream.Read(buffer);

            var hostInput = Encoding.UTF8.GetString(buffer,0, received);

            return hostInput;
        }
        public static void ClientPlaceing(string ownPlaceing, char ownDirection)
        {
            try
            {
                using TcpClient client = new();
                client.Connect(hostPlayer);
                using NetworkStream stream = client.GetStream();

                string allInput = ownPlaceing + "x" + ownDirection;

                var connectionTestBytes = Encoding.UTF8.GetBytes(allInput);
                stream.Write(connectionTestBytes);
            }
            catch
            {
                Console.WriteLine("Something went wrong! Host disconnected?");
                Thread.Sleep(1000);

            }

        }

        public static void ClientPlaying(string clientGuess)
        {
            try
            {
                using TcpClient client = new();
                client.Connect(hostPlayer);
                using NetworkStream stream = client.GetStream();

                var connectionTestBytes = Encoding.UTF8.GetBytes(clientGuess);
                stream.Write(connectionTestBytes);
            }
            catch
            {
                Console.WriteLine("Something went wrong! Host disconnected?");
                Thread.Sleep(1000);

            }

        }

    }

}

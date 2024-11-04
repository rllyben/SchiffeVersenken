using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken
{
    internal class Client
    {
        private static bool error = false;
        private static IPEndPoint hostPlayer;
        private static IPEndPoint clientPlayer;
        private static int playfieldSize;
        /// <summary>
        /// Handels the input of the host Address and the first connection to the host
        /// </summary>
        public static void Connect()
        {
            Program.PrintTitle();
            do
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("Join Game");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    Console.WriteLine("Please enter the host ip-adress and the port (192.168.0.0:12345)");
                    string input = Console.ReadLine();
                    string[] parts = input.Split(':');
                    IPAddress ipAdress = IPAddress.Parse(parts[0]);
                    var localIpAddress = IPAddress.Parse(Utils.GetLocalIPAddress());
                    int port = int.Parse(parts[1]);
                    hostPlayer = new IPEndPoint(ipAdress, port);
                    clientPlayer = new IPEndPoint(localIpAddress, port);
                }
                catch
                {
                    error = true;
                }

            }while(error);

            do
            {
                bool connected = false;
                try
                {
                    using TcpClient client = new();
                    client.Connect(hostPlayer);
                    using NetworkStream streaming = client.GetStream();

                    var buffer = new byte[1024];
                    int connectionClientTest = streaming.Read(buffer);

                    var clientTest = Encoding.UTF8.GetString(buffer, 0, connectionClientTest);

                    if (clientTest == "Connected to Host")
                    {
                        connected = true;
                    }
                    else
                        throw new Exception("Connection to host failed");

                    int hostConnectionTest = streaming.Read(buffer);
                    var playfield = Encoding.UTF8.GetString(buffer, 0, hostConnectionTest);
                    playfieldSize = int.Parse(playfield);
                    if (playfieldSize >= 10)
                    {
                        Console.WriteLine("Connected successfully to Host");
                        Thread.Sleep(1000);
                    }
                    else
                        throw new Exception("Connection from host failed");

                    error = false;
                }
                catch
                {
                    if (connected)
                    {
                        using TcpClient client = new();
                        client.Connect(hostPlayer);
                        using NetworkStream straming = client.GetStream();
                        var connectionTestBytes = Encoding.UTF8.GetBytes("error");
                        straming.Write(connectionTestBytes);
                    }
                    Program.PrintTitle();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Connection to Host failed!");
                    Console.ForegroundColor = ConsoleColor.White;
                    Thread.Sleep(1000);
                    error = true;
                }

            } while (error);
            using TcpClient cliente = new();
            cliente.Connect(hostPlayer);
            using NetworkStream stream = cliente.GetStream();
            var connectedBytes = Encoding.UTF8.GetBytes("fine");
            stream.Write(connectedBytes);
            Program.client.Initalitaion(playfieldSize);
            Program.host.Initalitaion(playfieldSize);
            Program.ClientPlay();
        }
        /// <summary>
        /// reads the input from the host for shipplacement and guessing
        /// </summary>
        /// <returns>the complete input</returns>
        public static string HostPlaying()
        {
            try
            {
                using TcpClient client = new();
                client.Connect(hostPlayer);
                using NetworkStream stream = client.GetStream();

                var buffer = new byte[1024];
                int received = stream.Read(buffer);

                var hostInput = Encoding.UTF8.GetString(buffer, 0, received);

                return hostInput;
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something went wrong! Host disconnected?");
                Console.ForegroundColor = ConsoleColor.White;
                Thread.Sleep(1000);
                return "error";
            }

        }
        /// <summary>
        /// Sends the client shipplaceing input to the host
        /// </summary>
        /// <param name="ownPlaceing">the coordinates the ship is placed on</param>
        /// <param name="ownDirection">the coordinates the ship is faceing</param>
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something went wrong! Host disconnected?");
                Console.ForegroundColor = ConsoleColor.White;
                Program.MainMenue();
                Thread.Sleep(1000);
            }

        }
        /// <summary>
        /// Sends the input from the clientsided guessing to the host
        /// </summary>
        /// <param name="clientGuess">the input from the client</param>
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something went wrong! Host disconnected?");
                Console.ForegroundColor = ConsoleColor.White;
                Program.MainMenue();
                Thread.Sleep(1000);
            }

        }

    }

}

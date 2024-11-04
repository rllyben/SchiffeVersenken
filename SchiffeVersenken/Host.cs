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
        private static int port;
        private static TcpListener listener;
        private static bool error = false;
        private static int playfieldSize = 0;
        /// <summary>
        /// Handels the playfield size for online games and the specific port choise (automatic porthandling in Utils.cs)
        /// </summary>
        public static void HostSetup()
        {
            do
            {
                Program.PrintTitle();
                Console.WriteLine("How long do you want the edges of the playfield to be? (At least 10)");
                try
                {
                    playfieldSize = int.Parse(Console.ReadLine());
                    if (playfieldSize < 10)
                        throw new Exception("Playfield to small");
                    error = false;

                }
                catch
                {
                    Console.WriteLine("Wrong input!");
                    error = true;
                    Thread.Sleep(1000);
                }

            } while (error);
            do
            {
                Console.WriteLine("Do you want to coose a Port? (y/N)");
                char choise = Console.ReadKey().KeyChar;
                choise = char.ToLower(choise);
                if (choise == 'y')
                {
                    Console.WriteLine("Pleader enter your Port (recomended: 49152 - 65535)");
                    try
                    {
                        port = int.Parse(Console.ReadLine());
                        error = false;
                    }
                    catch
                    {
                        error = true;
                    }
                }
                else
                {
                    port = Utils.GetAvailablePort();
                    error = false;
                }

            } while (error);
            HostConnection();
        }
        /// <summary>
        /// Handels the connecting between host and client and sends the client the playfield size
        /// </summary>
        public static void HostConnection()
        { 
            localIpAddress = IPAddress.Parse(Utils.GetLocalIPAddress());
            IPEndPoint ipEndPoint = new(localIpAddress, port);
            listener = new(ipEndPoint);

            Program.PrintTitle();
            Console.WriteLine("Host Game");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Your host IP-Adress and Port is {ipEndPoint}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            listener.Start();
            Console.WriteLine("Waiting for connection...");
            do
            {
                try
                {
                    using TcpClient handler = listener.AcceptTcpClient();
                    using NetworkStream stream = handler.GetStream();

                    var message = "Connected to Host";
                    var connectionTestBytes = Encoding.UTF8.GetBytes(message);
                    stream.Write(connectionTestBytes);

                    var buffer = new byte[1024];
                    Thread.Sleep(100);

                    string playfieldMassage = playfieldSize.ToString();
                    var playfieldbytes = Encoding.UTF8.GetBytes(playfieldMassage);
                    stream.Write(playfieldbytes);

                    using TcpClient errorHandler = listener.AcceptTcpClient();
                    using NetworkStream errorMassage = errorHandler.GetStream();
                    int received = errorMassage.Read(buffer);
                    var hostTest = Encoding.UTF8.GetString(buffer, 0, received);

                    if (hostTest == "error")
                        throw new Exception("Client threw exception");

                    error = false;
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Something went wrong! Client disconnected?");
                    Console.ForegroundColor = ConsoleColor.White;
                    error = true;
                    Thread.Sleep(1000);
                }

            } while (error);
            Console.WriteLine("Client connected successfully!");
            Thread.Sleep(1000);
            Program.host.Initalitaion(playfieldSize);
            Program.client.Initalitaion(playfieldSize);
            Program.HostPlay();
        }
        /// <summary>
        /// Handels the sending of the shipplacement input to the client
        /// </summary>
        /// <param name="ownPlaceing">the coordinates the ship was placed on</param>
        /// <param name="ownDirection">the direction of the placed ship</param>
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something went wrong! Client disconnected?");
                Console.ForegroundColor = ConsoleColor.White;
                Program.MainMenue();
                Thread.Sleep(1000);
            }

        }
        /// <summary>
        /// Sends the coordinates the host guessed to the client
        /// </summary>
        /// <param name="hostGuess"></param>
        public static void HostPlaying(string hostGuess)
        {
            using TcpClient handler = listener.AcceptTcpClient();
            try
            {
                using NetworkStream stream = handler.GetStream();

                var connectionTestBytes = Encoding.UTF8.GetBytes(hostGuess);
                stream.Write(connectionTestBytes);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something went wrong! Client disconnected?");
                Console.ForegroundColor = ConsoleColor.White;
                Program.MainMenue();
                Thread.Sleep(1000);
            }

        }
        /// <summary>
        /// reads the input from the client shipplacement
        /// </summary>
        /// <returns>the client imput</returns>
        public static string ClientPlaying()
        {
            try
            {
                using TcpClient handler = listener.AcceptTcpClient();
                using NetworkStream stream = handler.GetStream();

                var buffer = new byte[1024];
                int received = stream.Read(buffer);

                var clientInput = Encoding.UTF8.GetString(buffer, 0, received);

                return clientInput;
            }
            catch
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something went wrong! Client disconnected?");
                Console.ForegroundColor = ConsoleColor.White;
                Thread.Sleep(1000);
                return "error";
            }

        }

    }

}

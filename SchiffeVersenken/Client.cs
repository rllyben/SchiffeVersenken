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

        public static async void Connect()
        {
            Program.PrintTitle();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Join Game");
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Please enter the host ip-adress and the port (192.168.0.0:12345)");
            string input = Console.ReadLine();
            string[] parts = input.Split(':');
            int ipAdress = int.Parse(parts[0]);
            int port = int.Parse(parts[1]);

            var hostPlayer = new IPEndPoint(ipAdress, port);

            using TcpClient client = new();
            await client.ConnectAsync(hostPlayer);
            await using NetworkStream stream = client.GetStream();

            var buffer = new byte[1024];
            int received = await stream.ReadAsync(buffer);

            var message = Encoding.UTF8.GetString(buffer, 0, received);
            Console.WriteLine(message);

        }

    }

}

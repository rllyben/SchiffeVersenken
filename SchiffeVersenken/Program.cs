using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SchiffeVersenken
{
    internal class Program
    {
        public static Playfield host = new Playfield();
        public static Playfield client = new Playfield();

        public static bool gameStart = false;
        static void Main(string[] args)
        {
            PrintTitle();
            Console.WriteLine("Do You want to play on this PC (<-) or over Network (->)?");
            ConsoleKey modeSwitch = Console.ReadKey().Key;
            switch (modeSwitch)
            {
                case ConsoleKey.LeftArrow: OfflinePlay();  break;
                case ConsoleKey.RightArrow: NetwortkPlay();  break;
            }

        }

        public static void NetwortkPlay()
        {
            PrintTitle();
            Console.WriteLine("Hello, do you want to <- host a game or want to join a game ->?");
            ConsoleKey networkSwitch = Console.ReadKey().Key;
            switch (networkSwitch)
            {
                case ConsoleKey.LeftArrow: Host.HostSetup(); break;
                case ConsoleKey.RightArrow: Client.Connect(); break;
            }

        }
        public static void HostPlay()
        {
            host.ShipPlaceing(1);
            PrintTitle();
            client.RemoteShipPlaceing(2);

            while (gameStart)
            {
                PrintTitle();
                host.Guessing(client.playField, client.guessField);
                PrintTitle();
                client.RemoteGuessing(2, host.playField, host.guessField);
            }

        }
        public static void ClientPlay()
        {
            PrintTitle();
            host.RemoteShipPlaceing(1);

            while (gameStart)
            {
                PrintTitle();
                client.ShipPlaceing(2);
                PrintTitle();
                host.RemoteGuessing(1, client.playField, client.guessField);
                PrintTitle();
                client.Guessing(host.playField, client.guessField);
            }

        }

        public static void OfflinePlay()
        {
            PrintTitle();
            Playfield player1 = new Playfield();
            player1.ShipPlaceing(0);
            Console.Clear();
            PrintTitle();
            Console.WriteLine("Now Player 2");
            Console.ReadKey();
            Playfield player2 = new Playfield();
            player2.ShipPlaceing(0);
            Console.Clear();

            while (gameStart)
            {
                PrintTitle();
                Console.WriteLine("Player 1s Turn");
                Console.ReadKey();
                player1.Guessing(player2.playField, player2.guessField);
                Console.WriteLine("Player 2s Turn");
                Console.ReadKey();
                player2.Guessing(player1.playField, player1.guessField);
            }

        }

        public static void PrintTitle()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Battle Ship");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
            return;
        }

    }

}

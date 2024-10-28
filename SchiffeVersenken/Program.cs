using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace SchiffeVersenken
{
    internal class Program
    {
        public static Playfield host = new Playfield();
        public static Playfield client = new Playfield();
        public static bool gameStart = false;
        public static bool gameRun = true;
        public static bool error = false;
        public static int gamesPlayed = 0;
        public static int gamesWon = 0;
        public static int gamesLost = 0;
        static void Main(string[] args)
        {
            Console.SetWindowSize(127, 34);
            do
            {
                MainMenue();
            }while(gameRun);

        }
        public static void MainMenue()
        {
            PrintTitle();
            Console.WriteLine("Main Menue");
            Console.WriteLine();
            string ausgabe = "Playing statistics ▲ \n" +
                              "Play offline (coop over one PC) ◄ \n" +
                              "Play online (coop over network) ► \n" +
                              "Close Game ▼";
            Console.WriteLine(ausgabe);
            ConsoleKey modeSwitch = ConsoleKey.NoName;
            do
            {
                try
                {
                    modeSwitch = Console.ReadKey().Key;
                    error = false;
                }
                catch { error = true; }
            } while (error);

            switch (modeSwitch)
            {
                case ConsoleKey.LeftArrow: OfflinePlay(); break;
                case ConsoleKey.RightArrow: NetwortkPlay(); break;
                case ConsoleKey.UpArrow: PlayerStatistics(); break;
                case ConsoleKey.DownArrow: gameRun = false; break;
            }

        }
        public static void PlayerStatistics()
        {
            PrintTitle();
            Console.WriteLine("Player statistics");
            Console.WriteLine();
            Console.WriteLine($"Games Played: {gamesPlayed} \n" +
                              $"Games Won: {gamesWon} \n" +
                              $"Games Lost: {gamesLost} \n\n" +
                              "Back with any key");
            Console.ReadKey();
            MainMenue();
        }
        public static void NetwortkPlay()
        {
            PrintTitle();
            Console.WriteLine("Battleship Network");
            Console.WriteLine();
            Console.WriteLine("Host game ◄ \n" +
                              "Join game ► \n" +
                              "Back to Main Menue ▼");
            ConsoleKey networkSwitch = ConsoleKey.NoName;
            do
            {
                try
                {
                    networkSwitch = Console.ReadKey().Key;
                    error = false;
                }
                catch { error = true; }
            } while (error);

            switch (networkSwitch)
            {
                case ConsoleKey.LeftArrow: Host.HostSetup(); break;
                case ConsoleKey.RightArrow: Client.Connect(); break;
                case ConsoleKey.DownArrow: MainMenue(); break;
            }

        }
        public static void HostPlay()
        {
            host.ShipPlaceing(1);
            PrintTitle();
            client.RemoteShipPlaceing(2);
            gameStart = true;

            while (gameStart)
            {
                Console.Clear();
                host.Guessing(1, client.playField, client.guessField);
                host.PrintPlayfield(client.playField, client.guessField);
                Console.WriteLine("Client is guessing...");
                client.RemoteGuessing(1, host.playField, host.guessField);
            }

        }
        public static void ClientPlay()
        {
            PrintTitle();
            host.RemoteShipPlaceing(1);
            PrintTitle();
            client.ShipPlaceing(2);
            gameStart = true;

            while (gameStart)
            {
                host.RemoteGuessing(2, client.playField, client.guessField);
                Console.Clear();
                client.Guessing(2, host.playField, host.guessField);
                client.PrintPlayfield(host.playField, host.guessField);
                Console.WriteLine("Host is guessing...");
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
            gameStart = true;

            while (gameStart)
            {
                PrintTitle();
                Console.WriteLine("Player 1s Turn");
                Console.ReadKey();
                player1.Guessing(0, player2.playField, player2.guessField);
                Console.WriteLine("Player 2s Turn");
                Console.ReadKey();
                player2.Guessing(0, player1.playField, player1.guessField);
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

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SchiffeVersenken
{
    internal class Program
    {
        public static Playfield host = new Playfield();
        public static Playfield client = new Playfield();
        public static Program debugger = new Program();
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
            } while (gameRun);

        }
        /// <summary>
        /// Prints and handels the Main Menue
        /// with ConsoleKey.Arrow
        /// </summary>
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
            bool wait = true;
            do
            {
                ConsoleKey modeSwitch = Console.ReadKey().Key;
                switch (modeSwitch)
                {
                    case ConsoleKey.LeftArrow: wait = false; OfflinePlay(); break;
                    case ConsoleKey.RightArrow: wait = false; NetwortkPlay(); break;
                    case ConsoleKey.UpArrow: wait = false; PlayerStatistics(); break;
                    case ConsoleKey.DownArrow: wait = false; gameRun = false; break;
                    case ConsoleKey.D: wait = false; debugger.DebugingMode(); break;
                }

            } while (wait);

        }
        /// <summary>
        /// Handels the debuging mode of the game
        /// </summary>
        public void DebugingMode()
        {
            Playfield debuging = new Playfield();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Battleship Debugger\n" +
                                  "1. Initialisation\n");
                int choise = 0;
                do
                {
                    try
                    {
                        choise = int.Parse(Console.ReadLine());
                        error = false;
                    }
                    catch { error = true; }

                    switch (choise)
                    {
                        case 0: ExitDebugingMode(); error = false; break;
                        case 1: debuging.Initalitaion(0, true); error = false; break;
                    }

                } while (error);

            }

        }
        /// <summary>
        /// Resets all changes done in the debuging mode
        /// and opens the Main Menue
        /// </summary>
        private static void ExitDebugingMode()
        {
            debugger = new Program();
            error = false;
            bool gameRun = true;
            bool gameStart = false;
            gamesPlayed = 0;
            gamesWon = 0;
            gamesLost = 0;
            client = new Playfield();
            host = new Playfield();
            MainMenue();
        }
        /// <summary>
        /// Prints the Playerstatistics
        /// </summary>
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
        /// <summary>
        /// Handels the choise for network play for host or joining a game
        /// </summary>
        public static void NetwortkPlay()
        {
            PrintTitle();
            Console.WriteLine("Battleship Networkplaymode");
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
        /// <summary>
        /// Handels the hostsite gameplay (NOT for connecting between the clients(found in Host.cs))
        /// </summary>
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
                host.PrintEnemyPlayfield(client.playField, client.guessField);
                if (gameStart)
                {
                    Console.WriteLine("Client is guessing...");
                    client.RemoteGuessing(1, host.playField, host.guessField);
                }
                else
                {

                }

            }

        }
        /// <summary>
        /// Handels the clientsite gameplay (NOT for connecting between the clients(found in Client.cs))
        /// </summary>
        public static void ClientPlay()
        {
            PrintTitle();
            host.RemoteShipPlaceing(1);
            PrintTitle();
            client.ShipPlaceing(2);
            gameStart = true;

            while (gameStart)
            {
                Console.WriteLine("Host is guessing...");
                host.RemoteGuessing(2, client.playField, client.guessField);
                if (gameStart)
                {
                    Console.Clear();
                    client.Guessing(2, host.playField, host.guessField);
                    client.PrintEnemyPlayfield(host.playField, host.guessField);
                }
                else
                {

                }

            }

        }
        /// <summary>
        /// Handels the gameplay on one computer for playing offline
        /// </summary>
        public static void OfflinePlay()
        {
            Playfield player1 = new Playfield();
            Playfield player2 = new Playfield();
            do
            {
                PrintTitle();
                Console.WriteLine("How long do you want the edges of the playfield to be? (At least 10)");
                try
                {
                    int playfieldSize = int.Parse(Console.ReadLine());
                    if (playfieldSize < 10)
                        throw new Exception("Playfield to small");

                    player1.Initalitaion(playfieldSize);
                    player2.Initalitaion(playfieldSize);
                    error = false;
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Wrong input!");
                    Console.ForegroundColor = ConsoleColor.White;
                    error = true;
                    Thread.Sleep(1000);
                }

            } while (error);

            PrintTitle();
            player1.ShipPlaceing(0);
            Console.Clear();
            PrintTitle();
            Console.WriteLine("Now Player 2");
            Console.ReadKey();
            player2.ShipPlaceing(0);
            Console.Clear();
            gameStart = true;

            while (gameStart)
            {
                PrintTitle();
                Console.WriteLine("Player 1s Turn");
                Console.ReadKey();
                player1.Guessing(0, player2.playField, player2.guessField);
                PrintTitle();
                Console.WriteLine("Player 2s Turn");
                Console.ReadKey();
                player2.Guessing(0, player1.playField, player1.guessField);
            }

        }
        /// <summary>
        /// Prints the title of the game
        /// </summary>
        public static void PrintTitle()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Battle Ship");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine();
            return;
        }

    }

}

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SchiffeVersenken
{
    internal class Program
    {
        public static bool gameStart = false;
        static void Main(string[] args)
        {
            PrintTitle();
            Console.WriteLine("Hello, do you want to <- host a game or want to join a game ->?");
            ConsoleKey input = Console.ReadKey().Key;
            switch (input)
            {
                case ConsoleKey.LeftArrow: break;
                case ConsoleKey.RightArrow: break;
            }

            Playfield player1 = new Playfield();
            player1.ShipPlaceing();
            Console.Clear();
            Console.WriteLine("");

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

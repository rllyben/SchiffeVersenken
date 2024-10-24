using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NAudio.Wave;

namespace SchiffeVersenken
{
    internal class Spielfeld
    {

        public Spielfeld()
        {

        }
        Schiff battleship = new Schiff(5, "battleship");
        Schiff cruiser = new Schiff(4, "cruiser");
        Schiff destroyer = new Schiff(3, "destroyer");
        Schiff UBoat = new Schiff(2, "U-Boat");

        private Schiff[] ships = new Schiff[10];
        public ushort[,] playField = new ushort[10, 10];
        public ushort[,] guessField = new ushort[10, 10];

        private ushort[,] dumpPlayField = new ushort[10, 10];
        private void Initalitaion()
        {
            ships[0] = battleship;
            ships[1] = cruiser;
            ships[2] = cruiser;
            ships[3] = destroyer;
            ships[4] = destroyer;
            ships[5] = destroyer;
            ships[6] = UBoat;
            ships[7] = UBoat;
            ships[8] = UBoat;
            ships[9] = UBoat;

            for (int outer = 0; outer < 10; outer++)
            {
                for (int inner = 0; inner < 10; inner++)
                {
                    playField[outer, inner] = 0;
                    guessField[outer, inner] = 0;
                    dumpPlayField[outer, inner] = 0;
                }

            }

            PrintPlayfield(dumpPlayField, dumpPlayField);
            return;

        }

        public void PrintPlayfield(ushort[,] enemyPlayField, ushort[,] enemyGuessField)
        {
            Console.Clear();
            Console.WriteLine("Gegnerisches Feld");
            Console.WriteLine();
            for (int outer = -1; outer < 10; outer++)
            {
                int outerChar = outer + 65;
                char zeichen = (char)outerChar;
                if (outer >= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(zeichen + " ");
                    Console.ResetColor();
                }
                else Console.Write("  ");

                for (int inner = 0; inner < 10; inner++)
                {
                    if (outer == -1)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(inner + 1 + " ");
                        Console.ResetColor();
                    }
                    else if (guessField[outer, inner] == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.Write("  ");
                        Console.ResetColor();
                    }
                    else if (enemyPlayField[outer, inner] == 0 && guessField[outer, inner] == 1)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.Write("  ");
                        Console.ResetColor();
                    }
                    else if (enemyPlayField[outer, inner] != 0 && guessField[outer, inner] == 1)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.Write("  ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Something went wrong, reprinting Field ...");
                        Thread.Sleep(1000);
                        PrintPlayfield(enemyPlayField, enemyGuessField);
                    }

                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Eigenes Feld");
            Console.WriteLine();
            for (int outer = -1; outer < 10; outer++)
            {
                int outerChar = outer + 65;
                char zeichen = (char)outerChar;
                if (outer >= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(zeichen + " ");
                    Console.ResetColor();
                }
                else Console.Write("  ");

                for (int inner = 0; inner < 10; inner++)
                {
                    if (outer == -1)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(inner + 1 + " ");
                        Console.ResetColor();
                    }
                    else if (playField[outer, inner] == 0 && enemyGuessField[outer, inner] == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.Write("  ");
                        Console.ResetColor();
                    }
                    else if (playField[outer, inner] != 0 && enemyGuessField[outer, inner] == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.Write("  ");
                        Console.ResetColor();
                    }
                    else if (playField[outer, inner] == 0 && enemyGuessField[outer, inner] == 1)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.Write("  ");
                        Console.ResetColor();
                    }
                    else if (playField[outer, inner] != 0 && enemyGuessField[outer, inner] == 1)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("XX");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Something went wrong, reprinting Field ...");
                        Thread.Sleep(1000);
                        PrintPlayfield(enemyPlayField, enemyGuessField);
                    }

                }
                Console.WriteLine();

                if (false)
                {

                }

            }
            return;
        }

        public void ShipPlaceing()
        {
            Initalitaion();

            Console.WriteLine("Please set your Ships");
            Console.WriteLine();

            for (ushort shipCount = 0; shipCount < 10; shipCount++)
            {
                Console.WriteLine($"Set your {ships[shipCount].GetName()}");
                Console.WriteLine($"Please write the start Coordinate (Ax2)");
                string saveInput;
                ushort x = 0;
                ushort y = 0;
                bool error = true;
                while (error)
                {
                    saveInput = Console.ReadLine();
                    saveInput.ToUpper();
                    try
                    {
                        string[] parts = saveInput.Split('x');
                        char firstPart = char.Parse(parts[0]);
                        firstPart = Char.ToUpper(firstPart);
                        x = (ushort)firstPart;
                        x -= 65;
                        y = ushort.Parse(parts[1]);
                        y--;
                        error = false;
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Wrong input, have fun doing it all again after a short word from our sponsor! :P");
                        Console.ResetColor();
                        var psi = new ProcessStartInfo("C:\\Program Files\\Mozilla Firefox\\firefox.exe");
                        psi.Arguments = "https://www.youtube.com/watch?v=12doxFJo778&pp=ygUecmFpZCBzaGFkb3cgbGVnZW5kcyBzcG9uc29yIGFk";
                        Process.Start(psi);

                    }

                }

                bool innerError = true;
                while (innerError)
                {
                    Console.WriteLine("In which direction should the ship face in (N,E,S,W)?");
                    char direction = Console.ReadKey().KeyChar;
                    direction = Char.ToUpper(direction);
                    Console.WriteLine(direction);

                    if (direction == 'W' || direction == 'E' || direction == 'S' || direction == 'N')
                    {
                        switch (direction)
                        {
                            case 'W':
                                try
                                {
                                    for (short west = 0; west < ships[shipCount].GetLength(); west++)
                                    {
                                        playField[x, y + west] = (ushort)(shipCount + 1);
                                    }
                                    PrintPlayfield(dumpPlayField, dumpPlayField);
                                    innerError = false;
                                }
                                catch
                                {
                                    Console.WriteLine("Wrong input, you're getting deported to France! Redo all input! :3");
                                    Thread.Sleep(5000);
                                }

                                break;
                            case 'E':
                                try
                                {
                                    for (short east = 0; east < ships[shipCount].GetLength(); east++)
                                    {
                                        playField[x, y - east] = (ushort)(shipCount + 1);
                                    }
                                    PrintPlayfield(dumpPlayField, dumpPlayField);
                                    innerError = false;
                                }
                                catch
                                {
                                    Console.WriteLine("Wrong input, your browser data was sent to the local church, a priest is on his way to you! please redo everything!");
                                    Thread.Sleep(5000);
                                }

                                break;
                            case 'S':
                                try
                                {
                                    for (short south = 0; south < ships[shipCount].GetLength(); south++)
                                    {
                                        playField[x + south, y] = (ushort)(shipCount + 1);
                                    }
                                    PrintPlayfield(dumpPlayField, dumpPlayField);
                                    innerError = false;
                                }
                                catch
                                {
                                    Console.WriteLine("Wrong input! You went too far south, now the Pinguins are after you and you forgot your winterjacket! Redo everything! :p");
                                    Thread.Sleep(5000);
                                }

                                break;
                            case 'N':
                                try
                                {
                                    for (short north = 0; north < ships[shipCount].GetLength(); north++)
                                    {
                                        playField[x - north, y] = (ushort)(shipCount + 1);
                                    }
                                    PrintPlayfield(dumpPlayField, dumpPlayField);
                                    innerError = false;
                                }
                                catch
                                {
                                    Console.WriteLine("Wrong input, your browser data was sent to the local church, a priest is on his way to you! please redo everything!");
                                    Thread.Sleep(5000);
                                } 
                                break;
                        }

                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Wrong input, your location was leaked to Google Maps and you have to redo the entire process! :P");
                        Console.ResetColor();
                        Thread.Sleep(5000);
                        shipCount--;
                    }

                }

            }

        }

        public void Guessing(ushort[,] enemyField, ushort[,] enemyGuesses)
        {
            PrintPlayfield(enemyField, enemyGuesses);

            Console.WriteLine("Plese enter the cooridnates you want to shoot at");
            string saveInput;
            ushort x = 0;
            ushort y = 0;
            saveInput = Console.ReadLine();
            try
            {
                string[] parts = saveInput.Split('x');
                char firstPart = char.Parse(parts[0]);
                x = (ushort)firstPart;
                x -= 65;
                y = ushort.Parse(parts[1]);
                y--;
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Wrong input, have fun doing it all again after a short word from our sponsor! :P");
                Console.ResetColor();
                Thread.Sleep(5000);
                Guessing(enemyField, enemyGuesses);
            }

            guessField[x, y] = 1;

            if (false) ;
            return;
        }

    }

}

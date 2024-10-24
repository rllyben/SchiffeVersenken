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
    internal class Playfield
    {
        Ships battleship = new Ships(5, "battleship");
        Ships cruiser1 = new Ships(4, "cruiser");
        Ships cruiser2 = new Ships(4, "cruiser");
        Ships destroyer1 = new Ships(3, "destroyer");
        Ships destroyer2 = new Ships(3, "destroyer");
        Ships destroyer3 = new Ships(3, "destroyer");
        Ships UBoat1 = new Ships(2, "U-Boat");
        Ships UBoat2 = new Ships(2, "U-Boat");
        Ships UBoat3 = new Ships(2, "U-Boat");
        Ships UBoat4 = new Ships(2, "U-Boat");
        Ships Empty = new Ships(0, "Nothing");

        private Ships[] ships = new Ships[10];
        public Ships[,] playField = new Ships[10, 10];
        public ushort[,] guessField = new ushort[10, 10];
        private int sunkenShips = 0;

        private Ships[,] dumpPlayField = new Ships[10, 10];
        private void Initalitaion()
        {
            ships[0] = battleship;
            ships[1] = cruiser1;
            ships[2] = cruiser2;
            ships[3] = destroyer1;
            ships[4] = destroyer2;
            ships[5] = destroyer3;
            ships[6] = UBoat1;
            ships[7] = UBoat2;
            ships[8] = UBoat3;
            ships[9] = UBoat4;

            for (int outer = 0; outer < 10; outer++)
            {
                for (int inner = 0; inner < 10; inner++)
                {
                    playField[outer, inner] = Empty;
                    guessField[outer, inner] = 0;
                    dumpPlayField[outer, inner] = Empty;
                }

            }
            PrintPlayfield(dumpPlayField, guessField);
            return;
        }

        public void PrintPlayfield(Ships[,] enemyPlayField, ushort[,] enemyGuessField)
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
                    else if (enemyPlayField[outer, inner] == Empty && guessField[outer, inner] == 1)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.Write("  ");
                        Console.ResetColor();
                    }
                    else if (enemyPlayField[outer, inner] != Empty && guessField[outer, inner] == 1)
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
                    else if (playField[outer, inner] == Empty && enemyGuessField[outer, inner] == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.Write("  ");
                        Console.ResetColor();
                    }
                    else if (playField[outer, inner] != Empty && enemyGuessField[outer, inner] == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.Write("  ");
                        Console.ResetColor();
                    }
                    else if (playField[outer, inner] == Empty && enemyGuessField[outer, inner] == 1)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.Write("  ");
                        Console.ResetColor();
                    }
                    else if (playField[outer, inner] != Empty && enemyGuessField[outer, inner] == 1)
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
                ushort x = 0;
                ushort y = 0;
                bool error = false;
                do
                {

                    Console.WriteLine($"Set your {ships[shipCount].GetName()}");
                    Console.WriteLine($"Please write the start Coordinate (Ax2)");
                    string saveInput;
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

                        if (x > 10 || y > 10 || playField[x, y] != Empty)
                            throw new Exception("Out of bounds");
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
                        error = true;
                    }

                } while (error);

                bool innerError = false;
                do
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
                                        if (playField[x, y - west] != Empty)
                                            throw new Exception("Not an empty Space!");
                                    }
                                    for (short west = 0; west < ships[shipCount].GetLength(); west++)
                                    {
                                        playField[x, y - west] = ships[shipCount];
                                    }
                                    PrintPlayfield(dumpPlayField, guessField);
                                    innerError = false;
                                }
                                catch
                                {
                                    Console.WriteLine("Wrong input, you're getting deported to France! Redo all input! :3");
                                    Thread.Sleep(5000);
                                    innerError = true;
                                }
                                break;
                            case 'E':
                                try
                                {
                                    for (short east = 0; east < ships[shipCount].GetLength(); east++)
                                    {
                                        if (playField[x, y + east] != Empty)
                                            throw new Exception("Not an empty Space!");
                                    }
                                    for (short east = 0; east < ships[shipCount].GetLength(); east++)
                                    {
                                        playField[x, y + east] = ships[shipCount];
                                    }
                                    PrintPlayfield(dumpPlayField, guessField);
                                    innerError = false;
                                }
                                catch
                                {
                                    Console.WriteLine("Wrong input, your browser data was sent to the local church, a priest is on his way to you! please redo everything!");
                                    Thread.Sleep(5000);
                                    innerError = true;
                                }
                                break;
                            case 'S':
                                try
                                {
                                    for (short south = 0; south < ships[shipCount].GetLength(); south++)
                                    {
                                        if (playField[x + south, y] != Empty)
                                            throw new Exception("Not an empty Space!");
                                    }
                                    for (short south = 0; south < ships[shipCount].GetLength(); south++)
                                    {
                                        playField[x + south, y] = ships[shipCount];
                                    }
                                    PrintPlayfield(dumpPlayField, guessField);
                                    innerError = false;
                                }
                                catch
                                {
                                    Console.WriteLine("Wrong input! You went too far south, now the Pinguins are after you and you forgot your winterjacket! Redo everything! :p");
                                    Thread.Sleep(5000);
                                    innerError = true;
                                }
                                break;
                            case 'N':
                                try
                                {
                                    for (short north = 0; north < ships[shipCount].GetLength(); north++)
                                    {
                                        if (playField[x - north, y] != Empty)
                                            throw new Exception("Not an empty Space!");
                                    }
                                    for (short north = 0; north < ships[shipCount].GetLength(); north++)
                                    {
                                        playField[x - north, y] = ships[shipCount];
                                    }
                                    PrintPlayfield(dumpPlayField, guessField);
                                    innerError = false;
                                }
                                catch
                                {
                                    Console.WriteLine("Wrong input, your browser data was sent to the local church, a priest is on his way to you! please redo everything!");
                                    Thread.Sleep(5000);
                                    innerError = true;
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

                } while (innerError);

            }

        }

        public void Guessing(Ships[,] enemyField, ushort[,] enemyGuesses)
        {
            PrintPlayfield(enemyField, enemyGuesses);

            Console.WriteLine("Plese enter the cooridnates you want to shoot at");
            string saveInput;
            ushort x = 0;
            ushort y = 0;
            bool error = false;
            do
            {
                try
                {

                    saveInput = Console.ReadLine();
                    string[] parts = saveInput.Split('x');
                    char firstPart = char.Parse(parts[0]);
                    firstPart = Char.ToUpper(firstPart);
                    x = (ushort)firstPart;
                    x -= 65;
                    y = ushort.Parse(parts[1]);
                    y--;

                    if (guessField[x, y] != 0)
                        throw new Exception("Allready Guessed");
                    guessField[x, y] = 1;

                    enemyField[x, y].LooseHealth();
                    if (enemyField[x, y].health == 0)
                    {
                        Console.WriteLine($"You sank an {enemyField[x, y].GetName()}!");
                        Thread.Sleep(1000);
                        sunkenShips++;
                    }
                    if (sunkenShips == 10)
                    {
                        Console.WriteLine("GZ! You won the Game!");
                        Program.gameStart = false;
                        Console.ReadKey();
                    }
                    error = false;
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Wrong input, have fun doing it all again after a short word from our sponsor! :P");
                    Console.ResetColor();
                    Thread.Sleep(5000);
                    error = true;
                }

            } while (error);

        }

    }

}

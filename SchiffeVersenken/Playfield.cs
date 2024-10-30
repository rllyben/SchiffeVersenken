using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NAudio.Wave;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SchiffeVersenken
{
    internal class Playfield
    {
        Ships Empty = new Ships(0, "Nothing");

        private Ships[] ships;
        public Ships[,] playField;
        public ushort[,] guessField;
        private int sunkenShips = 0;
        private Ships[,] dumpPlayField;
        private int shipCounter = 0;
        private int fieldSize = 0;
        public void Initalitaion(int size)
        {
            fieldSize = size;
            ships = new Ships[size];
            playField = new Ships[size, size];
            guessField = new ushort[size, size];
            dumpPlayField = new Ships[size, size];

            for (int count = 0; count < size / 10; count++)
            {
                Ships battleship = new Ships(5, "battleship");
                ships[shipCounter] = battleship;
                shipCounter++;
            }
            for (int count = 0; count < size / 5; count++)
            {
                Ships cruiser = new Ships(4, "cruiser");
                ships[shipCounter] = cruiser;
                shipCounter++;
            }
            for (int count = 0; count < size / 3; count++)
            {
                Ships destroyer = new Ships(3, "destroyer");
                ships[shipCounter] = destroyer;
                shipCounter++;
            }
            for (int count = 0; count < size / 2.5; count++)
            {
                Ships UBoat = new Ships(2, "U-Boat");
                ships[shipCounter] = UBoat;
                shipCounter++;
            }

            for (int outer = 0; outer < size; outer++)
            {
                for (int inner = 0; inner < size; inner++)
                {
                    playField[outer, inner] = Empty;
                    guessField[outer, inner] = 0;
                    dumpPlayField[outer, inner] = Empty;
                }

            }
            return;
        }
        public void PrintPlayfield(Ships[,] enemyPlayField, ushort[,] enemyGuessField)
        {
            Console.Clear();
            Console.WriteLine("Gegnerisches Feld");
            Console.WriteLine();
            for (int outer = -1; outer < fieldSize; outer++)
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

                for (int inner = 0; inner < fieldSize; inner++)
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
                    else if (enemyPlayField[outer, inner].GetName() == "Nothing" && guessField[outer, inner] == 1)
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
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Something went wrong, reprinting Field ...");
                        Console.ResetColor();
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
            for (int outer = -1; outer < fieldSize; outer++)
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

                for (int inner = 0; inner < fieldSize; inner++)
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
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.Write("  ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Something went wrong, reprinting Field ...");
                        Console.ResetColor();
                        Thread.Sleep(1000);
                        PrintPlayfield(enemyPlayField, enemyGuessField);
                    }

                }
                Console.WriteLine();
            }
            return;
        }
        public void ShipPlaceing(byte mode)
        {
            PrintPlayfield(dumpPlayField, guessField);

            Console.WriteLine("Please set your Ships");
            Console.WriteLine();

            for (ushort shipCount = 0; shipCount < shipCounter; shipCount++)
            {
                ushort x = 0;
                ushort y = 0;
                bool error = false;
                bool innerError = false;
                string saveInput;
                char direction;
                do
                {
                    do
                    {

                        Console.WriteLine($"Set your {ships[shipCount].GetName()} [{ships[shipCount].GetLength()}]");
                        Console.WriteLine($"Please write the start Coordinate (Ax2)");
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

                            if (x > fieldSize || y > fieldSize || playField[x, y] != Empty)
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

                    do
                    {
                        Console.WriteLine("In which direction should the ship face in (N,E,S,W)?");
                        direction = Console.ReadKey().KeyChar;
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
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("Wrong input, you're getting deported to France! Redo all input! :3");
                                        Console.ResetColor();
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
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("Wrong input, your browser data was sent to the local church, a priest is on his way to you! please redo everything!");
                                        Console.ResetColor();
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
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("Wrong input! You went too far south, now the Pinguins are after you and you forgot your winterjacket! Redo everything! :p");
                                        Console.ResetColor();
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
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("Wrong input, your browser data was sent to the local church, a priest is on his way to you! please redo everything!");
                                        Console.ResetColor();
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
                            innerError = true;
                        }

                    } while (innerError);

                } while (error || innerError);
                if (mode == 1)
                {
                    Host.HostShipPlaceing(saveInput, direction);
                }
                else if (mode == 2)
                {
                    Client.ClientPlaceing(saveInput, direction);
                }

            }

        }
        public void RemoteShipPlaceing(byte mode)
        {
            if (mode == 1)
                Console.WriteLine("Waiting for Host to place Ships...");
            else if (mode == 2)
                Console.WriteLine("Waiting for Client to place Ships...");

            for (ushort shipCount = 0; shipCount < shipCounter; shipCount++)
            {
                string remoteInput = "";
                if (mode == 1)
                {
                    remoteInput = Client.HostPlaying();
                }
                else if (mode == 2)
                {
                    remoteInput = Host.ClientPlaying();
                }
                string[] parts = remoteInput.Split('x');

                int x;
                int y;
                char firstPart = char.Parse(parts[0]);
                firstPart = Char.ToUpper(firstPart);
                x = (ushort)firstPart;
                x -= 65;
                y = ushort.Parse(parts[1]);
                y--;

                char direction = char.Parse(parts[2]);

                switch (direction)
                {
                    case 'W':
                        for (short west = 0; west < ships[shipCount].GetLength(); west++)
                        {
                            playField[x, y - west] = ships[shipCount];
                        }
                        break;
                    case 'E':
                        for (short east = 0; east < ships[shipCount].GetLength(); east++)
                        {
                            playField[x, y + east] = ships[shipCount];
                        }
                        break;
                    case 'S':
                        for (short south = 0; south < ships[shipCount].GetLength(); south++)
                        {
                            playField[x + south, y] = ships[shipCount];
                        }
                        break;
                    case 'N':
                        for (short north = 0; north < ships[shipCount].GetLength(); north++)
                        {
                            playField[x - north, y] = ships[shipCount];
                        }
                        break;
                }

            }

        }
        public void Guessing(byte mode, Ships[,] enemyField, ushort[,] enemyGuesses)
        {
            PrintPlayfield(enemyField, enemyGuesses);

            Console.WriteLine("Plese enter the cooridnates you want to shoot at");
            string saveInput = "";
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
                    if (sunkenShips == shipCounter)
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
            if (mode == 1)
                Host.HostPlaying(saveInput);
            else if (mode == 2)
                Client.ClientPlaying(saveInput);
        }
        public void RemoteGuessing(byte mode, Ships[,] enemyField, ushort[,] enemyGuesses)
        {
            string saveInput = "";
            if (mode == 1)
            {
                saveInput = Host.ClientPlaying();
            }
            else if (mode == 2)
            {
                saveInput = Client.HostPlaying();
            }
            ushort x = 0;
            ushort y = 0;
            string[] parts = saveInput.Split('x');
            char firstPart = char.Parse(parts[0]);
            firstPart = Char.ToUpper(firstPart);
            x = (ushort)firstPart;
            x -= 65;
            y = ushort.Parse(parts[1]);
            y--;

            guessField[x, y] = 1;
            enemyField[x, y].LooseHealth();

            if (enemyField[x, y].health == 0)
            {
                Console.WriteLine($"Your {enemyField[x, y].GetName()} got sunk!");
                Thread.Sleep(1000);
                sunkenShips++;
            }
            if (sunkenShips == shipCounter)
            {
                Console.WriteLine("You lost the Game!");
                Program.gameStart = false;
                Console.ReadKey();
            }

        }

    }

}

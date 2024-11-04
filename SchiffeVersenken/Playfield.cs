using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using static System.Net.WebRequestMethods;

namespace SchiffeVersenken
{
    internal class Playfield
    {
        Ships Empty = new Ships(0, "Nothing");

        private Ships[] ships;
        public Ships[,] playField;
        public int[,] guessField;
        private int sunkenShips = 0;
        private Ships[,] dumpPlayField;
        private int shipCounter = 0;
        private int fieldSize = 0;
        public static int setShipCount = 0;
        private bool error = false;
        /// <summary>
        /// Creates the Y-Coordinate label in A - AZ and further
        /// </summary>
        /// <param name="labelNumber">the current coresponding Y-Coordinate number</param>
        /// <returns>The created label</returns>
        private string GetAlphabeticalLabel(int labelNumber)
        {
            string label = "";
            while (labelNumber >= 0)
            {
                label = (char)(labelNumber % 26 + 65) + label;
                labelNumber = (labelNumber / 26) - 1;
            }
            return label;
        }
        /// <summary>
        /// Initializes the playfield for each player
        /// </summary>
        /// <param name="size">the size of the playfield</param>
        /// <param name="debug">if the game is in debug mode</param>
        public void Initalitaion(int size, bool debug = false)
        {
            if (!debug)
            {
                fieldSize = size;
                ships = new Ships[size];
                playField = new Ships[size, size];
                guessField = new int[size, size];
                dumpPlayField = new Ships[size, size];
            }
            else 
            {
                do
                {
                    try
                    {
                        Console.WriteLine("give playfield size");
                        fieldSize = int.Parse(Console.ReadLine());
                        Console.WriteLine("give amount of ships");
                        ships = new Ships[int.Parse(Console.ReadLine())];
                        playField = new Ships[fieldSize, fieldSize];
                        guessField = new int[fieldSize, fieldSize];
                        dumpPlayField = new Ships[fieldSize, fieldSize];
                        error = false;
                    }
                    catch { error = true; }
                } while (error);

            }
                for (int count = 0; count < ships.Length / 10; count++)
                {
                    Ships battleship = new Ships(5, "battleship");
                    ships[shipCounter] = battleship;
                    shipCounter++;
                }
                for (int count = 0; count < ships.Length / 5; count++)
                {
                    Ships cruiser = new Ships(4, "cruiser");
                    ships[shipCounter] = cruiser;
                    shipCounter++;
                }
                for (int count = 0; count < ships.Length / 3; count++)
                {
                    Ships destroyer = new Ships(3, "destroyer");
                    ships[shipCounter] = destroyer;
                    shipCounter++;
                }
                for (int count = 0; count < ships.Length / 2.5; count++)
                {
                    if (shipCounter == 30)
                        break;
                    Ships sub = new Ships(2, "submarine");
                    ships[shipCounter] = sub;
                    shipCounter++;
                }

                for (int outer = 0; outer < fieldSize; outer++)
                {
                    for (int inner = 0; inner < fieldSize; inner++)
                    {
                        playField[outer, inner] = Empty;
                        guessField[outer, inner] = 0;
                        dumpPlayField[outer, inner] = Empty;
                    }

                }
            return;
        }
        /// <summary>
        /// Prints the X-Coordinate numbers
        /// </summary>
        /// <param name="debug">if the game is in debug mode</param>
        public void PrintXLine(bool debug = false)
        {
            if (!debug)
            {
                Console.Write(new string(' ', playField.GetLength(0) / 25 + 2));
                Console.ForegroundColor = ConsoleColor.Cyan;
                for (int xLine = 0; xLine < fieldSize; xLine++)
                {
                    Console.Write($"{xLine + 1} ");
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
            }
            else
            {

            }

        }
        /// <summary>
        /// Prints the enemyplayfield coverd by the guessfield
        /// </summary>
        /// <param name="enemyPlayField">gives the enemyplayfield to check for own hits</param>
        /// <param name="enemyGuessField">gives the enemyguessfield to check for enemy hits</param>
        /// <param name="debug">if the game is in debug mode</param>
        public void PrintEnemyPlayfield(Ships[,] enemyPlayField, int[,] enemyGuessField, bool debug = false)
        {
            if (!debug)
            {
                Console.Clear();
                Console.WriteLine("Gegnerisches Feld");
                Console.WriteLine();
                PrintXLine();
                for (int outer = 0; outer < fieldSize; outer++)
                {
                    string yLabel = GetAlphabeticalLabel(outer);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(yLabel + new string(' ', yLabel.Length));
                    Console.ForegroundColor = ConsoleColor.White;

                    for (int inner = 0; inner < fieldSize; inner++)
                    {
                        if (guessField[outer, inner] == 0)
                        {
                            Console.BackgroundColor = ConsoleColor.Gray;
                        }
                        else if (enemyPlayField[outer, inner].GetName() == "Nothing" && guessField[outer, inner] == 1)
                        {
                            Console.BackgroundColor = ConsoleColor.Blue;
                        }
                        else if (enemyPlayField[outer, inner] != Empty && guessField[outer, inner] == 1)
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                        }
                        Console.Write("  ");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                Console.WriteLine();
                PrintOwnPlayfield(enemyGuessField);
            }
            return;
        }
        /// <summary>
        /// Prints the own playfield with the ships and hits from the enemy
        /// </summary>
        /// <param name="enemyGuessField">gives the enemyguessfield to check is a ship got hit</param>
        /// <param name="debug">if the game is in debug mode</param>
        public void PrintOwnPlayfield(int[,] enemyGuessField, bool debug = false)
        {
            if (!debug)
            {
                Console.WriteLine("Eigenes Feld");
                Console.WriteLine();
                PrintXLine();
                for (int outer = 0; outer < fieldSize; outer++)
                {
                    string yLabel = GetAlphabeticalLabel(outer);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    int spaces = Math.Max(1, 2 - yLabel.Length);
                    Console.Write(yLabel + new string(' ', spaces));
                    Console.ForegroundColor = ConsoleColor.White;

                    for (int inner = 0; inner < fieldSize; inner++)
                    {
                        if (playField[outer, inner] == Empty && enemyGuessField[outer, inner] == 0)
                        {
                            Console.BackgroundColor = ConsoleColor.Blue;
                        }
                        else if (playField[outer, inner] != Empty && enemyGuessField[outer, inner] == 0)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                        }
                        else if (playField[outer, inner] == Empty && enemyGuessField[outer, inner] == 1)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                        }
                        else if (playField[outer, inner] != Empty && enemyGuessField[outer, inner] == 1)
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                        }
                        Console.Write("  ");
                        Console.BackgroundColor = ConsoleColor.Black;

                    }
                    Console.WriteLine();
                }
            }
            return;
        }
        /// <summary>
        /// Checks if at the positions the player wants to set a ship is allready a ship
        /// </summary>
        /// <param name="xStart">gives the X-Coordinate</param>
        /// <param name="yStart">gives the Y-Coordinate</param>
        /// <param name="direction">gives the direction the ship is suposed to face</param>
        /// <param name="shipLength">gives the length of the ship to set</param>
        /// <param name="debug">if the game is in debug mode</param>
        /// <returns></returns>
        public bool CheckForShips(int xStart, int yStart, char direction, int shipLength, bool debug = false)
        {
            if (!debug)
            {
                if (playField[yStart, xStart] != Empty) return false;

                int xValue = xStart;
                int yValue = yStart;

                for (int count = 0; count < shipLength; count++)
                {
                    switch (direction)
                    {
                        case 'N': yValue--; break;
                        case 'E': xValue++; break;
                        case 'S': yValue++; break;
                        case 'W': xValue--; break;
                    }
                    if (playField[yValue, xValue] != Empty) return false;
                }
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// Places the ships if they get trugh the ship check
        /// </summary>
        /// <param name="mode">checks if the game runs as client, host or is offline</param>
        /// <param name="debug">if the game is in debug mode</param>
        public void ShipPlaceing(byte mode, bool debug = false)
        {
            if (!debug)
            {
                for (ushort shipCount = 0; shipCount < shipCounter; shipCount++)
                {
                    Program.PrintTitle();
                    Console.WriteLine("Please set your Ships");
                    Console.WriteLine();
                    PrintOwnPlayfield(guessField);
                    int xStart = 0;
                    int yStart = 0;
                    int xValue = xStart;
                    int yValue = yStart;
                    bool error = false;
                    bool innerError = false;
                    string saveInput;
                    char direction;
                    do
                    {
                        Console.WriteLine($"Set your {ships[shipCount].GetName()} [{ships[shipCount].GetLength()}]");
                        Console.WriteLine($"Please write the start Coordinate (A2)");
                        saveInput = Console.ReadLine();
                        saveInput = saveInput.ToUpper();

                        Console.WriteLine("In which direction should the ship face in (N,E,S,W)?");
                        direction = Console.ReadKey().KeyChar;
                        direction = Char.ToUpper(direction);

                        try
                        {
                            char firstPart = saveInput[0];
                            yStart = (int)firstPart - 65;
                            xStart = int.Parse(saveInput.Substring(1)) - 1;

                            if (xStart > fieldSize || yStart > fieldSize || !CheckForShips(xStart, yStart, direction, ships[shipCount].GetLength()))
                                throw new Exception("Out of bounds");
                            if (direction != 'N' && direction != 'E' && direction != 'S' && direction != 'W')
                                throw new Exception("Not a correct direction");
                            error = false;
                        }
                        catch
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Wrong input, have fun doing it all again after a short word from our sponsor! :P");
                            Console.ResetColor();
                            Process.Start(new ProcessStartInfo("https://www.youtube.com/watch?v=12doxFJo778&pp=ygUecmFpZCBzaGFkb3cgbGVnZW5kcyBzcG9uc29yIGFk") { UseShellExecute = true });
                            error = true;
                        }

                    } while (error);
                    for (int setShip = 0; setShip < ships[shipCount].GetLength(); setShip++)
                    {
                        switch (direction)
                        {
                            case 'N':
                                playField[yValue, xStart] = ships[shipCount]; yValue--; break;
                            case 'E':
                                playField[yStart, xValue] = ships[shipCount]; xValue++; break;
                            case 'S':
                                playField[yValue, xStart] = ships[shipCount]; yValue++; break;
                            case 'W':
                                playField[yStart, xValue] = ships[shipCount]; xValue--; break;
                        }

                    }
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

        }
        /// <summary>
        /// Handels the gotten shipplacement input in networkplay and updates the enemyfield for the current client
        /// </summary>
        /// <param name="mode">checks if the game is run as client or host</param>
        /// <param name="debug">if the game is in debug mode</param>
        public void RemoteShipPlaceing(byte mode, bool debug = false)
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
                if (remoteInput == "error")
                {
                    Program.MainMenue();
                }

                int x;
                int y;
                char firstPart = remoteInput[0];
                firstPart = Char.ToUpper(firstPart);
                y = (int)firstPart - 65;
                string[] parts = remoteInput.Split('x');
                x = int.Parse(parts[0].Substring(1)) - 1;

                char direction = char.Parse(parts[1]);

                switch (direction)
                {
                    case 'W':
                        for (short west = 0; west < ships[shipCount].GetLength(); west++)
                        {
                            playField[y, x - west] = ships[shipCount];
                        }
                        break;
                    case 'E':
                        for (short east = 0; east < ships[shipCount].GetLength(); east++)
                        {
                            playField[y, x + east] = ships[shipCount];
                        }
                        break;
                    case 'S':
                        for (short south = 0; south < ships[shipCount].GetLength(); south++)
                        {
                            playField[y + south, x] = ships[shipCount];
                        }
                        break;
                    case 'N':
                        for (short north = 0; north < ships[shipCount].GetLength(); north++)
                        {
                            playField[y - north, x] = ships[shipCount];
                        }
                        break;
                }

            }

        }
        /// <summary>
        /// Handels the guessing of the current player and writes it into the guessfield
        /// </summary>
        /// <param name="mode">checks if the cleint is run in client, host or offline mode</param>
        /// <param name="enemyField">just for the printing method</param>
        /// <param name="enemyGuesses">just for the printing method</param>
        /// <param name="debug">if the game is in debug mode</param>
        public void Guessing(byte mode, Ships[,] enemyField, int[,] enemyGuesses, bool debug = false)
        {
            if (!debug)
            {
                PrintEnemyPlayfield(enemyField, enemyGuesses);

                Console.WriteLine("Plese enter the cooridnates you want to shoot at");
                string saveInput = "";
                int x = 0;
                int y = 0;
                bool error = false;
                do
                {
                    try
                    {
                        saveInput = Console.ReadLine();
                        char firstPart = saveInput[0];
                        firstPart = Char.ToUpper(firstPart);
                        y = (int)firstPart - 65;
                        x = int.Parse(saveInput.Substring(1)) - 1;

                        if (guessField[y, x] != 0)
                            throw new Exception("Allready Guessed");
                        guessField[y, x] = 1;

                        enemyField[y, x].LooseHealth();
                        if (enemyField[y, x].health == 0)
                        {
                            Console.WriteLine($"You sank an {enemyField[y, x].GetName()}!");
                            Thread.Sleep(1000);
                            sunkenShips++;
                        }
                        if (sunkenShips == shipCounter)
                        {
                            Console.WriteLine("GZ! You won the Game!");
                            Program.gamesPlayed++;
                            Program.gamesWon++;
                            Program.gameStart = false;
                            Thread.Sleep(1000);
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

        }
        /// <summary>
        /// Handels the input gotten from the other player in network play and writes it into the enemyguessfield
        /// </summary>
        /// <param name="mode">checks if the client runs as client or host</param>
        /// <param name="enemyField">just for printing</param>
        /// <param name="enemyGuesses">to write the gotten input into the enemyguessfield</param>
        /// <param name="debug">if the game is in debug mode</param>
        public void RemoteGuessing(byte mode, Ships[,] enemyField, int[,] enemyGuesses, bool debug = false)
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
            if (saveInput == "error")
            {
                Program.MainMenue();
            }

            int x = 0;
            int y = 0;
            char firstPart = saveInput[0];
            firstPart = Char.ToUpper(firstPart);
            y = (int)firstPart - 65;
            x = int.Parse(saveInput.Substring(1)) - 1;

            guessField[y, x] = 1;
            enemyField[y, x].LooseHealth();

            if (enemyField[y, x].health == 0)
            {
                Console.WriteLine($"Your {enemyField[y, x].GetName()} got sunk!");
                Thread.Sleep(1000);
                sunkenShips++;
            }
            if (sunkenShips == shipCounter)
            {
                Console.WriteLine("You lost the Game!");
                Program.gamesPlayed++;
                Program.gamesLost++;
                Program.gameStart = false;
                Thread.Sleep(1000);
            }

        }

    }

}

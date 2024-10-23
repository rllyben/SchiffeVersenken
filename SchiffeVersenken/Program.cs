namespace SchiffeVersenken
{
    internal class Program
    {
        public static bool gameStart = false;
        static void Main(string[] args)
        {

            Spielfeld player1 = new Spielfeld();
            Spielfeld player2 = new Spielfeld();
            player1.ShipPlaceing();
            Console.Clear();
            Console.WriteLine("Now Player 2");
            Console.WriteLine("press any Key to continue");
            Console.ReadKey();

            player2.ShipPlaceing();
            gameStart = true;

            while (gameStart)
            {
                Console.Clear();
                Console.WriteLine("Now Player 1 starts");
                Console.WriteLine("press any Key to continue");
                Console.ReadKey();
                player1.Guessing(player2.playField, player2.guessField);
                Console.Clear();
                Console.WriteLine("Now Player 2 starts");
                Console.WriteLine("press any Key to continue");
                Console.ReadKey();
                player2.Guessing(player1.playField, player1.guessField);
            }

        }

    }

}

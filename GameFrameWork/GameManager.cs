namespace GameFrameWork
{
    public class GameManager
    {
        // The game is restarted or not
        private bool IsRestarted = false;

        public void Run()
        {
            DisplayIntro();
            while (!IsRestarted)
            {
                SelectStartOptions();
            }
        }

        public void DisplayIntro()
        {
            // Display the intro of the game
            string[] introduction = {
                "=======================================",
                "|| Welcome to Numerical Tic Tac Toe! ||",
                "||   Play Tic Tac Toe with numbers!  ||",
                "=======================================",
                "||  Written by: [N12159069] Alex Yoo ||",
                "======================================="
            };
            foreach (string line in introduction)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }

        public void DisplayMainMenu()
        {
            Console.WriteLine("=========================================");
            Console.WriteLine("|| Welcome to the World of Board Games  ||");
            Console.WriteLine("=========================================");
            Console.WriteLine("1. Numerical Tic-Tac-Toe");
            Console.WriteLine("2. Notakto (coming soon)");
            Console.WriteLine("3. Gomoku (coming soon)");
            Console.WriteLine("4. Exit");
            Console.Write("\nSelect a game to play: ");
        }

        public static void PlayGame(string gameType)
        {
            AbstractGame game = GameFactory.CreateGame(gameType);
            game.Play();
            
            Console.WriteLine("\nGame over! Would you like to play again? (Y/N)");
            string playAgain = Console.ReadLine();
            
            if (playAgain.ToUpper() == "Y")
            {
                PlayGame(gameType);
            }
        }

        public void SelectStartOptions()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                DisplayMainMenu();

                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            PlayGame("NumericalTicTacToe");
                            break;
                        case "2":
                            Console.WriteLine("Notakto will be available in a future update!");
                            PressAnyKeyToContinue();
                            break;
                        case "3":
                            Console.WriteLine("Gomoku will be available in a future update!");
                            PressAnyKeyToContinue();
                            break;
                        case "4":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            PressAnyKeyToContinue();
                            break;
                    }
                }
                catch (NotImplementedException ex)
                {
                    Console.WriteLine($"Game not available yet: {ex.Message}");
                    PressAnyKeyToContinue();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    PressAnyKeyToContinue();
                }
            }
        }
        static void PressAnyKeyToContinue()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

    }
}
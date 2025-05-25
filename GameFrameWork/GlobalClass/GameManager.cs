namespace GameFrameWork
{
    public class GameManager
    {
        private bool IsRestarted = false;

        public void Run()
        {
            DisplayIntro();
            while (!IsRestarted)
            {
                SelectGameOptions();
            }
        }

        public void DisplayIntro()
        {
            // Display the intro of the game
            string[] introduction = {
                "=======================================",
                "|| Welcome to Board Game World!       ||",
                "|| 3 Different board games in one!    ||",
                "=======================================",
                "||  Written by:                       ||",
                "||         [N12159069] Alex Yoo       ||",
                "||         [N12102725] Yen-Ling, Liu  ||",
                "||         [N11691611] Norfatini      ||",
                "||         [N11725575] Wan            ||",
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
            Console.WriteLine("2. Notakto");
            Console.WriteLine("3. Gomoku");
            Console.WriteLine("4. Exit");
            Console.Write("\nSelect a game to play >> ");
        }

        public static void PlayGame(string gameType)
        {
            AbstractGame game = GameFactory.CreateGame(gameType);

            if (!SelectStartOptions(game, gameType))
            {
                // Return to main menu if SelectStartOptions returns false
                return;
            }

            game.Play();

            Console.WriteLine("\nGame over! Would you like to play again? (Y/N)");
            string playAgain = Console.ReadLine();

            // Validate input
            while (string.IsNullOrWhiteSpace(playAgain) ||
                (playAgain.Trim().ToLower() != "y" && playAgain.Trim().ToLower() != "n"))
            {
                Console.WriteLine("Invalid input. Please enter 'Y' or 'N'.");
                playAgain = Console.ReadLine();
            }

            if (playAgain.Trim().ToLower() == "y")
            {
                PlayGame(gameType);
            }
            else // playAgain == "n"
            {
                // Return to game selection
                return;
            }
        }

        public void SelectGameOptions()
        {
            bool gameOptionsExit = false;

            while (!gameOptionsExit)
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
                            PlayGame("Notakto");
                            break;
                        case "3":
                            PlayGame("Gomoku");
                            break;
                        case "4":
                            gameOptionsExit = true;
                            Console.WriteLine("Exiting the Game... Thank you for playing!");
                            Environment.Exit(0);
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

        public static bool SelectStartOptions(AbstractGame game, string gameType)
        {
            bool startOptionsExit = false;
            
            // Determine game display name
            string gameDisplayName = "Game";
            if (gameType != null)
            {
                switch (gameType.ToLower())
                {
                    case "numericaltictactoe":
                        gameDisplayName = "Numerical Tic-Tac-Toe";
                        break;
                    case "gomoku":
                        gameDisplayName = "Gomoku";
                        break;
                    case "notakto":
                        gameDisplayName = "Notakto";
                        break;
                    default:
                        gameDisplayName = "Game";
                        break;
                }
            }
            else
            {
                // fallback action - Detect game type from game object
                if (game is TicTacToeGame)
                    gameDisplayName = "Numerical Tic-Tac-Toe";
                else if (game is NotaktoGame)
                    gameDisplayName = "Notakto";
                else if (game is GomokuGame)
                    gameDisplayName = "Gomoku";
                else
                    gameDisplayName = "Game";
            }
            
            while (!startOptionsExit)
            {
                Console.Clear();
                Console.WriteLine($"{gameDisplayName} Options:");
                Console.WriteLine("1. Start a new game");
                Console.WriteLine("2. Load a saved game");
                Console.WriteLine("3. Back to game selection");
                Console.Write("\nSelect an option >> ");
                string startOptionInput = Console.ReadLine();
                
                try
                {
                    int startOption = int.Parse(startOptionInput);
                    switch (startOption)
                    {
                        case 1:
                            Console.WriteLine("\nStarting a new game...");
                            startOptionsExit = true;
                            // Return true to indicate we should play the game
                            return true; 
                        case 2:
                            Console.Write("\nEnter file name to load >> ");
                            string filename = Console.ReadLine();
                            
                            // Use polymorphic LoadGame method
                            if (game.LoadGame(filename))
                            {
                                // Game loaded successfully, start playing
                                game.StartGame();
                                // After game is done, come back to this menu
                                startOptionsExit = false;
                            }
                            else
                            {
                                PressAnyKeyToContinue();
                            }
                            break;
                        case 3:
                            Console.WriteLine("\nReturning to game selection...");
                            return false; 
                        default:
                            Console.WriteLine("\nInvalid option selected. Please try again.");
                            PressAnyKeyToContinue();
                            break;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("\nInvalid input. Please enter a number.");
                    PressAnyKeyToContinue();
                }
            }
            return true;
        }

        public static void PressAnyKeyToContinue()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

    }
}
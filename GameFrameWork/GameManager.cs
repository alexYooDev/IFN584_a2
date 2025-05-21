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
                SelectGameOptions();
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

            if (!SelectStartOptions(game))
            {
                return; // Return to main menu if SelectStartOptions returns false
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
                            Console.WriteLine("Notakto will be available in a future update!");
                            PressAnyKeyToContinue();
                            break;
                        case "3":
                            Console.WriteLine("Gomoku will be available in a future update!");
                            PressAnyKeyToContinue();
                            break;
                        case "4":
                            gameOptionsExit = true;
                            Console.WriteLine("Exiting the Game... Thank You");
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

        public static bool SelectStartOptions(AbstractGame game)
        {
            bool startOptionsExit = false;
            while (!startOptionsExit)
            {
                Console.Clear();
                Console.WriteLine($"Numerical Tic-Tac-Toe Options:");
                Console.WriteLine("1. Start a new game");
                Console.WriteLine("2. Load a saved game");
                Console.WriteLine("3. Back to game selection");
                Console.Write("\nSelect an option >> ");
                string startOptionInput = Console.ReadLine();
                int startOption;
                try
                {
                    startOption = int.Parse(startOptionInput);
                    switch (startOption)
                    {
                        case 1:
                            Console.WriteLine("\nStarting a new game...");
                            startOptionsExit = true;
                            return true; // Return true to indicate we should play the game
                        case 2:
                            Console.WriteLine("\nEnter file name to load >> ");
                            string filename = Console.ReadLine();
                            TicTacToeGame loadedGame = (TicTacToeGame)game;
                            if (loadedGame.LoadGame(filename))
                            {
                                // Game loaded successfully, start playing
                                loadedGame.StartGame();
                                // After game is done, come back to this menu
                                startOptionsExit = false;
                            }
                            else
                            {
                                GameManager.PressAnyKeyToContinue();
                            }
                            break;
                        case 3:
                            Console.WriteLine("\nReturning to game selection...");
                            return false; // Return false to indicate we want to go back to game selection
                        default:
                            Console.WriteLine("\nInvalid option selected. Please try again.");
                            GameManager.PressAnyKeyToContinue();
                            break;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("\nInvalid input. Please enter a number.");
                    GameManager.PressAnyKeyToContinue();
                }
            }
            return true; // Default case - should play the game
        }

        public static void PressAnyKeyToContinue()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

    }
}
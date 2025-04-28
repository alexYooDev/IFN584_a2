
namespace TicTacToe
{
    class GameManager 
    {
        private Game Game;

        // The game is restarted or not
        private bool IsRestarted = false;

        public void Run()
        {
            DisplayIntro();
            while(!IsRestarted)
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

        // Select the start option of the game
        // 1. New Game 2. Load Saved Game 3. Quit Game
        public void SelectStartOptions()
        {
            Console.WriteLine("\n|| +++ Select the start option of the game +++ ||\n"); 
            Console.WriteLine("1. New Game");
            Console.WriteLine("2. Load Saved Game");
            Console.WriteLine("3. Quit Game");
            Console.Write("\nEnter your choice >> ");

            try 
            {
                int startOption = Convert.ToInt32(Console.ReadLine());

                switch(startOption)
                {
                    case 1:
                        // Start a new game
                        Console.WriteLine("\nStarting a new game...");
                        StartNewGame();
                        break;
                    case 2:
                        // Load a saved game
                        LoadSavedGame();
                        
                        break;
                    case 3:
                        // Quit the game
                        Console.WriteLine("\nQuitting the game...");
                        // StartOption = "Quit Game";
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("\nInvalid option selected. Please try again.");
                        SelectStartOptions();
                        break;
                }
            } 
            catch(FormatException)
            {
                Console.WriteLine("\nInvalid input. Please enter a number.");
                SelectStartOptions();
            }
        }

        // Starts a new game
        private void StartNewGame()
        {
            Game = new Game();
            Game.Play();
            // Prompt the user to play again on game over
            PromptRestart();
        }

        // Loads a saved game
        private void LoadSavedGame()
        {
            Game = new Game();
            Game.LoadGame();
            // Prompt the user to play again on game over
            PromptRestart();
        }

        // On game over, prompt the user to play again or quit
        public void PromptRestart()
        {
            Console.Write("\nDo you want to play again? (Y/N) >> ");
            string input = Console.ReadLine();
            if (input == "Y" || input == "y")
            {
                IsRestarted = true;
                SelectStartOptions();
            }
            else if (input == "N" || input == "n")
            {
                Console.WriteLine("Thank you for playing!");
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter Y or N.");
                PromptRestart();
            }
            return;
        }

        // Initially selects the game mode and returns the mode
        public string SelectMode()
        {
            Console.WriteLine("\n|| +++ Select the mode of the game +++ ||"); 
            Console.WriteLine("\n1. HvH (Human vs Human)");
            Console.WriteLine("2. HvC (Human vs Computer)");
            Console.Write("\nEnter your choice >> ");

            int input = Convert.ToInt32(Console.ReadLine());
            string mode = string.Empty;

            switch(input)
            {
                case 1:
                    // Human vs Human
                    Console.WriteLine("\nYou selected Human vs Human mode.");
                    mode = "HvH";
                    break;
                case 2:
                    // Human vs Computer
                    Console.WriteLine("\nYou selected Human vs Computer mode.");
                    mode = "HvC";
                    break;
                default:
                    Console.WriteLine("\nInvalid mode selected. Please try again.");
                    SelectMode();
                    break;
            }

            return mode;
        }

        // Configures the initial size of the board and returns the size
        public int SelectBoardSize()
        {
            int boardSize = 0;
                
            bool validInput = false;

            Console.WriteLine("\n|| +++ Size of the board +++ ||");
            while (!validInput)
            {
                Console.Write("\nSelect the size of the board (3 => 3X3/ 4 => 4X4/ 5 => 5X5/ etc.) >> ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out boardSize) && boardSize >= 3)
                {
                    validInput = true;
                }
                else
                {
                    Console.WriteLine("\nInvalid board size. Please enter a number greater or equals to 3.");
                }
            }

            return boardSize;
        }
    }
}
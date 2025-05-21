// game flow


namespace Notakto
{
    class NotaktoGameManager : AbstractGameManager
    {
        private NotaktoGame Game;

        // The game is restarted or not
        //private bool IsRestarted = false;

        public void Run()
        {
            DisplayIntro();
            while (true)
            {
                SelectStartOptions();
            }
        }

        public void DisplayIntro()
        {
            // Display the intro of the game
            string[] introduction = {
                "============================================",
                "||          Welcome to Notakto!          ||",
                "============================================",
                "||  Written by: [N12102725] Yen-Ling Liu ||",
                "||              [N12159069] Alex Yoo     ||",
                "============================================",
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

                switch (startOption)
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
            catch (FormatException)
            {
                Console.WriteLine("\nInvalid input. Please enter a number.");
                SelectStartOptions();
            }
        }

        // Starts a new game
        private void StartNewGame()
        {
            Game = new NotaktoGame();
            Game.Play();
            // Prompt the user to play again on game over
            PromptRestart();
        }

        // Loads a saved game
        private void LoadSavedGame()
        {
            Game = new NotaktoGame();
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

    }
}
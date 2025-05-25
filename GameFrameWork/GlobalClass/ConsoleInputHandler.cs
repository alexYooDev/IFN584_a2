namespace GameFrameWork
{
    /* Implementation of InputHandler interface
       Handles all user input with validation and error handling
     */

    public class ConsoleInputHandler : IInputHandler
    {
        public string GetUserInput(string prompt)
        {
            Console.Write($"\n{prompt} >> ");
            /* return the users input */
            return Console.ReadLine() ?? string.Empty;
        }

        public int GetUserIntInput(string prompt, int min, int max)
        {
            while (true)
            {
                try
                {
                    Console.Write($"\n{prompt} ({min}-{max}) >> ");
                    int value = int.Parse(Console.ReadLine() ?? "0");

                    if (value >= min && value <= max)
                    {
                        return value;
                    }

                    Console.WriteLine($"Please enter a number between {min} and {max}.");
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}. Please try again.");
                }
            }
        }

        public bool GetUserConfirmation(string message)
        {
            while (true)
            {
                Console.Write($"{message} (y/n) >> ");
                string response = Console.ReadLine()?.ToLower().Trim() ?? "";

                if (response == "y" || response == "yes")
                    return true;
                if (response == "n" || response == "no")
                    return false;

                Console.WriteLine("Please enter 'y' or 'n'.");
            }
        }

        public PlayerChoice GetPlayerChoice()
        {
            string input = GetUserInput("\nEnter your choice");

            switch (input)
            {
                case "1":
                    return PlayerChoice.MakeMove;
                case "2":
                    return PlayerChoice.Undo;
                case "3":
                    return PlayerChoice.Save;
                case "4":
                    return PlayerChoice.Help;
                case "5":
                    return PlayerChoice.Quit;
                default:
                    return PlayerChoice.Invalid;
            }
        }
     }
}
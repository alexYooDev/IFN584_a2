namespace GameFrameWork
{
    public class ConsoleGameRenderer : IGameRenderer
    {
        public void DisplayBoard(AbstractBoard board)
        {
            board.DisplayBoard();
        }
        
        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void DisplayGameStatus(string currentPlayer, int moveCount)
        {
            Console.WriteLine($"\nCurrent Turn: {currentPlayer}");
            Console.WriteLine($"Move: #{moveCount}");
        }

        public void DisplayTurnOptions()
        {
            Console.WriteLine("\n|| +++ Options +++ ||");
            Console.WriteLine("\nSelect your option for this turn:\n");
            Console.WriteLine("1. Make a move");
            Console.WriteLine("2. Undo previous moves");
            Console.WriteLine("3. Save the game");
            Console.WriteLine("4. View help menu");
            Console.WriteLine("5. Quit the game");
        }

        public void DisplayHelpMenu()
        {
            Console.WriteLine("\n|| +++ Help Menu +++ ||");
            Console.WriteLine("\nHow can I help you?");
            Console.WriteLine("1. Game Rules");
            Console.WriteLine("2. Available Commands");
            Console.WriteLine("3. Return to Game");
        }

        public void DisplayRules(string rules)
        {
            Console.WriteLine(rules);
        }

        public void DisplayCommands(string commands)
        {
            Console.WriteLine(commands);
        }

        public void ClearScreen()
        {
            Console.Clear();
        }

        public void PressAnyKeyToContinue()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
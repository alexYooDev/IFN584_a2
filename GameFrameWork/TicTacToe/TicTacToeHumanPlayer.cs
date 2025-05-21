namespace GameFrameWork
{
    public class TicTacToeHumanPlayer : AbstractHumanPlayer
    {

        private HashSet<int> AvailableNumbers;

        public TicTacToeHumanPlayer(string name, HashSet<int> availableNumbers) 
        : base(name, PlayerType.Human, availableNumbers.First() % 2 == 1 ? NumberType.Odd : NumberType.Even)
        {
            AvailableNumbers = availableNumbers;
        }

        public HashSet<int> GetAvailableNumbers()
        {
            return AvailableNumbers;
        }
        
        public override object SelectMove(AbstractBoard board)
        {
            return SelectNumber();
        }
        
        /* Numerical TicTacToe specific */
        public int SelectNumber()
        {
            try
            {
                Console.Write($"\nSelect the number to put on the board! ({string.Join(", ", AvailableNumbers)}) >> ");
                int selectedNumber = Convert.ToInt32(Console.ReadLine());

                // Check if the selected number is in the player's set of numbers
                if (AvailableNumbers.Contains(selectedNumber))
                {
                    AvailableNumbers.Remove(selectedNumber);
                    return selectedNumber;
                }

                // If the number is not in the set, prompt the user again
                else
                {
                    Console.WriteLine("You do not have this number! Please try again!");
                    return SelectNumber();
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("The Input should be a number! Try again!");
                return SelectNumber();
            }
        }
    }
}
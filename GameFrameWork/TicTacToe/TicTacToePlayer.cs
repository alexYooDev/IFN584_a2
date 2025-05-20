namespace GameFrameWork
{
    public class TicTacToeHumanPlayer : AbstractPlayer
    {

        private HashSet<int> AvailableNumbers;

        public TicTacToeHumanPlayer(string name, HashSet<int> availableNumbers) : base(name, PlayerType.Human)
        {
            AvailableNumbers = availableNumbers;

            if (availableNumbers.Count > 0)
            {
                NumberType = availableNumbers.First() % 2 == 1 ? GameFrameWork.NumberType.Odd : GameFrameWork.NumberType.Even;
            }
        }

        public HashSet<int> GetAvailableNumbers()
        {
            return AvailableNumbers;
        }
        
        public int SelectNumber()
        {
            try 
            {
                Console.Write($"\nSelect the number to put on the board! ({string.Join(", ", AvailableNumbers)}) >> ");
                int selectedNumber = Convert.ToInt32(Console.ReadLine());

                // Check if the selected number is in the player's set of numbers
                if(AvailableNumbers.Contains(selectedNumber))
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

    public class TicTacToeComputerPlayer : AbstractPlayer
    {

        private HashSet<int> AvailableNumbers;

        public TicTacToeComputerPlayer(HashSet<int> availableNumbers)
            : base("Computer", PlayerType.Computer)
        {
            AvailableNumbers = availableNumbers;

            // Set the NumberType based on first available number (odd/even)
            if (availableNumbers.Count > 0)
            {
                NumberType = availableNumbers.First() % 2 == 1 ? GameFrameWork.NumberType.Odd : GameFrameWork.NumberType.Even;
            }
        }

        public HashSet<int> GetAvailableNumbers()
        {
            return AvailableNumbers;
        }
        
        public int SelectRandomNumber()
        {
            // For computer, randomly select a number
            if (AvailableNumbers.Count > 0)
            {
                Random random = new Random();
                List<int> numbers = AvailableNumbers.ToList();
                int index = random.Next(numbers.Count);
                int selectedNumber = numbers[index];
                AvailableNumbers.Remove(selectedNumber);
                return selectedNumber;
            }
            
            // This should not happen in a normal game
            throw new InvalidOperationException("No numbers available for computer player");
        }
    }
}
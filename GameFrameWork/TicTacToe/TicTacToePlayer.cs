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

    public class TicTacToeComputerPlayer : AbstractComputerPlayer
    {
        private HashSet<int> AvailableNumbers;
        private TicTacToeBoard Board; // Added to access board for finding winning moves

        public TicTacToeComputerPlayer(HashSet<int> availableNumbers)
            : base("Computer", PlayerType.Computer, availableNumbers.First() % 2 == 1 ? NumberType.Odd : NumberType.Even)
        {
            AvailableNumbers = availableNumbers;
        }

        public HashSet<int> GetAvailableNumbers()
        {
            return AvailableNumbers;
        }
        
        // Implement the abstract method from AbstractComputerPlayer
        public override object FindWinningMove(AbstractBoard board)
        {
            // Store the board reference for calculating winning moves
            Board = (TicTacToeBoard)board;
            
            // Try each available number in each empty position to see if it creates a winning move
            int boardSize = Board.GetSize();
            
            foreach (int number in AvailableNumbers.ToList())
            {
                for (int row = 0; row < boardSize; row++)
                {
                    for (int col = 0; col < boardSize; col++)
                    {
                        if (Board.IsValidMove(row, col, number, 0, false))
                        {
                            // Save the current board state for undo
                            object previousState = Board.GetBoardState();

                            // Try the move
                            Board.MakeMove(row, col, number);

                            // Check if this is a winning move
                            if (IsWinningMove(row, col))
                            {
                                // Undo the test move
                                Board.SetBoardState(previousState);
                                
                                // Return the number as the winning move
                                return number;
                            }

                            // Undo the move if it's not a winning move
                            Board.SetBoardState(previousState);
                        }
                    }
                }
            }
            
            // No winning move found
            return null;
        }
        
        // Helper method to check if a move is a winning move
        private bool IsWinningMove(int row, int col)
        {
            int boardSize = Board.GetSize();
            int targetSum = boardSize * (boardSize * boardSize + 1) / 2;

            // Check row
            int rowSum = 0;
            bool rowFull = true;
            for (int c = 0; c < boardSize; c++)
            {
                int value = Board.GetValue(row, c);
                if (value == 0)
                {
                    rowFull = false;
                    break;
                }
                rowSum += value;
            }
            if (rowFull && rowSum == targetSum)
                return true;

            // Check column
            int colSum = 0;
            bool colFull = true;
            for (int r = 0; r < boardSize; r++)
            {
                int value = Board.GetValue(r, col);
                if (value == 0)
                {
                    colFull = false;
                    break;
                }
                colSum += value;
            }
            if (colFull && colSum == targetSum)
                return true;

            // Check diagonal if applicable
            if (row == col)
            {
                int diagSum = 0;
                bool diagFull = true;
                for (int i = 0; i < boardSize; i++)
                {
                    int value = Board.GetValue(i, i);
                    if (value == 0)
                    {
                        diagFull = false;
                        break;
                    }
                    diagSum += value;
                }
                if (diagFull && diagSum == targetSum)
                    return true;
            }

            // Check anti-diagonal if applicable
            if (row + col == boardSize - 1)
            {
                int antiDiagSum = 0;
                bool antiDiagFull = true;
                for (int i = 0; i < boardSize; i++)
                {
                    int value = Board.GetValue(i, boardSize - 1 - i);
                    if (value == 0)
                    {
                        antiDiagFull = false;
                        break;
                    }
                    antiDiagSum += value;
                }
                if (antiDiagFull && antiDiagSum == targetSum)
                    return true;
            }

            return false;
        }

        // Implement the abstract method from AbstractComputerPlayer
        public override object SelectRandomMove()
        {
            return SelectRandomNumber();
        }
        
        // Original method to select a random number
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
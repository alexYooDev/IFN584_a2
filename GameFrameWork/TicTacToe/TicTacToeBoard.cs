namespace GameFrameWork
{
    public class TicTacToeBoard : AbstractBoard
    {
        private int[,] Slots;

        public TicTacToeBoard(int size) : base(size)
        {
            Size = size;
            Slots = new int[size, size];
            BoardsState = new List<object> { Slots };
        }


        public override void DisplayBoard(int boardIndex = 0)
        {
            Console.WriteLine("\n|| +++ Current Numerical Tic-Tac-Toe Board Status +++ ||\n");
            
            for (int i = 0; i < Size; i++)
            {
                // Print horizontal border lines
                for (int k = 0; k < Size; k++)
                {
                    Console.Write("+-----");
                }
                Console.WriteLine("+");
                
                // Print the row content
                for (int j = 0; j < Size; j++)
                {
                    int slotValue = Slots[i, j];
                    string cellContent;
                    
                    if (slotValue == 0)
                    {
                        // Empty slot - show blank
                        cellContent = "     ";
                    }
                    else if (slotValue < 10)
                    {
                        // Single digit number - center it
                        cellContent = $"  {slotValue}  ";
                    }
                    else if (slotValue < 100)
                    {
                        // Two digit number - center it
                        cellContent = $" {slotValue}  ";
                    }
                    else
                    {
                        // Three digit number - center it
                        cellContent = $" {slotValue} ";
                    }
                    
                    Console.Write($"|{cellContent}");
                }
                Console.WriteLine("|");
            }
            
            // Print bottom border line
            for (int k = 0; k < Size; k++)
            {
                Console.Write("+-----");
            }
            Console.WriteLine("+");
        }

        public override bool IsValidMove(int row, int col, object moveData, int boardIndex = 0, bool displayMessages = true)
        {
            // Check if row and column are within the boundary of the board
            if (row < 0 || row >= Size || col < 0 || col >= Size)
            {
                if (displayMessages) Console.WriteLine("Your move exceeds the board! Please try again!");
                return false;
            }
            // Check if the slot is already occupied
            if (Slots[row, col] != 0)
            {
                if (displayMessages) Console.WriteLine("This slot is already occupied! Please try again!");
                return false;
            }
            return true;
        }

        public override void MakeMove(int row, int col, object moveData = null, int boardIndex = 0)
        {
            if (moveData != null && moveData is int number)
            {
                Slots[row, col] = number;
            }
        }


        public override bool IsBoardFull(int boardIndex = 0)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (Slots[i, j] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override object GetBoardState()
        {
            // Create a deep copy of the current board state
            /* crucial for computer finning a winning move */
            int[,] state = new int[Size, Size];
            
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    state[i, j] = Slots[i, j];
                }
            }
            
            return state;
        }

        public override void SetBoardState(object state)
        {
            if (state is int[,] boardState)
            {
                // Create a deep copy - don't just assign the reference!
                int rows = boardState.GetLength(0);
                int cols = boardState.GetLength(1);
                
                // Ensure Slots is the right size
                if (Slots == null || Slots.GetLength(0) != rows || Slots.GetLength(1) != cols)
                {
                    Slots = new int[rows, cols];
                }
                
                // Copy values, not references
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        Slots[i, j] = boardState[i, j];
                    }
                }
            }
        }

        public void DisplayGridWithPositions()
        {
            Console.WriteLine("\n|| +++ Select a position to put the number +++ ||\n");
            
            int gridNum = 1;

            for (int i = 0; i < Size; i++)
            {
                // Print horizontal border lines
                for (int k = 0; k < Size; k++)
                {
                    Console.Write("+-----");
                }
                Console.WriteLine("+");
                
                // Print the row content with position numbers
                for (int j = 0; j < Size; j++)
                {
                    int slotValue = Slots[i, j];
                    string cellContent;
                    
                    // If the slot is empty, display a grid number
                    if (slotValue == 0)
                    {
                        if (gridNum < 10)
                        {
                            cellContent = $"  {gridNum}  "; // 2 spaces before, 2 after for 1 digit
                        }
                        else if (gridNum < 100)
                        {
                            cellContent = $" {gridNum}  "; // 1 space before, 2 after for 2 digits
                        }
                        else
                        {
                            cellContent = $" {gridNum} "; // 1 space before, 1 after for 3 digits
                        }
                    }
                    // If it is taken, show empty slot
                    else
                    {
                        cellContent = "     ";
                    }
                    
                    Console.Write($"|{cellContent}");
                    gridNum++;
                }
                Console.WriteLine("|");
            }
            
            // Print bottom border line
            for (int k = 0; k < Size; k++)
            {
                Console.Write("+-----");
            }
            Console.WriteLine("+");
        }

        public int[] SelectPosition()
        {
            while (true)
            {
                try
                {
                    // Display the grid with numbers
                    Console.WriteLine("\n|| +++ Select a position to put the number +++ ||\n");

                    DisplayGridWithPositions();
                    Console.Write($"\nWhere in the board are you put the number? (1 - {Size * Size}) >> ");
                    int position = Convert.ToInt32(Console.ReadLine());

                    if (position < 1 || position > Size * Size)
                    {
                        Console.WriteLine("\nYou exceed the range of the board! Try again!");
                        continue;
                    }

                    int row = (position - 1) / Size;
                    int col = (position - 1) % Size;


                    /* When the position is already taken */
                    if (Slots[row, col] != 0)
                    {
                        Console.WriteLine("This position is already taken! Try again!");
                        continue;
                    }

                    return new int[] { row, col };
                }
                catch (FormatException)
                {
                    Console.WriteLine("Your Input should be a number! Try again!");
                }
                catch (Exception)
                {
                    Console.WriteLine("Unexpected error occurred! Try again!");
                }
            }
        }

        public int GetValue(int row, int col)
        {
            return Slots[row, col];
        }

        public void SetPosition(int row, int col, int number)
        {
            Slots[row, col] = number;
        }
    }
}
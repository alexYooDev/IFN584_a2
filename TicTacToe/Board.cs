namespace TicTacToe
{
    /* 
     Actual Form of Board 
     [
     [ , , ],
     [ , , ],
     [ , , ]
     ]
  */
    public class Board
    {
        /* fields for board
           - board slots
           - what is the size of the board
         */
        private int[,] Slot;
        private int Size;

        // Constructor for new game board
        public Board(int size)
        {
            Size = size;
            Slot = new int[size, size];
        }

        // Constructor for loading a saved game board
        public Board(int[,] slot, int size)
        {
            Size = size;
            Slot = slot;
        }

        public void GetBoardStatus()
        {
            Console.WriteLine("\n|| +++ Current Board Status +++ ||\n");
            for (int i = 0; i < Size; i++)
            {
                for (int k = 0; k < Size; ++k)
                {
                    Console.Write("----");
                }
                Console.WriteLine();
                for (int j = 0; j < Size; j++)
                {
                    if (Slot[i, j] < 10)
                    {
                        Console.Write(Slot[i, j] == 0 ? $"|   " : $"| {Slot[i, j]} ");
                    }
                    else
                    {
                        Console.Write($"| {Slot[i, j]}");
                    }
                }
                Console.Write("|");
                Console.WriteLine();
            }
            for (int k = 0; k < Size; ++k)
            {
                Console.Write("----");
            }
            Console.WriteLine();
        }

        public void SetBoardSize(int size)
        {
            Size = size;
            Slot = new int[size, size];
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    Slot[i, j] = 0;
                }
            }
        }

        // returns the square board perimeter
        public int GetBoardSize()
        {
            return Size;
        }

        public bool IsValidMove(int row, int col)
        {
            // Check if row and column are within the boundary of the board
            if (row < 0 || row > Size || col < 0 || col > Size)
            {
                Console.WriteLine("Your move exceeds the board! Please try again!");
                return false;
            }
            // Check if the slot is already occupied
            if (Slot[row, col] != 0)
            {
                Console.WriteLine("This slot is already occupied! Please try again!");
                return false;
            }
            return true;
        }

        // Display the grid with numbers
        public void DisplayGrid()
        {
            int gridNum = 1;

            // Display the grid with numbers
            for (int i = 0; i < Size; ++i)
            {
                for (int k = 0; k < Size; ++k)
                {
                    Console.Write("-----");
                }
                Console.WriteLine();
                Console.Write($"|");
                for (int j = 0; j < Size; ++j)
                {
                    int slotValue = GetValue(i, j);

                    // If the slot is empty, display a grid number
                    if (slotValue == 0)
                    {
                        if (gridNum < 10)
                        {
                            Console.Write($"  {gridNum} |");
                        }
                        else
                        {
                            Console.Write($" {gridNum} |");
                        }
                    }
                    // If it is taken, display black slot
                    else
                    {
                        Console.Write("    |");
                    }
                    gridNum++;
                }
                Console.WriteLine();
            }
            for (int k = 0; k < Size; ++k)
            {
                Console.Write("-----");
            }
            Console.WriteLine();
        }



        // Prompts the user to select a position on the board
        public int[] SelectPosition()
        {
            while (true)
            {

                try
                {
                    // Display the grid with numbers
                    Console.WriteLine("\n|| +++ Select a position to put the number +++ ||\n");

                    DisplayGrid();
                    Console.Write($"\nWhere in the board are you put the number? (1 - {Size * Size}) >> ");
                    int position = Convert.ToInt32(Console.ReadLine());

                    if (position < 1 || position > Size * Size)
                    {
                        Console.WriteLine("\nYou exceed the range of the board! Try again!");
                        return SelectPosition();
                    }


                    int row = (position - 1) / Size;
                    int col = (position - 1) % Size;

                    if (GetValue(row, col) != 0) // Position already taken
                    {
                        continue;
                    }

                    return [row, col];

                }
                catch (FormatException)
                {
                    Console.WriteLine("Your Input should be a number! Try again!");
                    return SelectPosition();
                }
                catch (Exception)
                {
                    Console.WriteLine("Unexpected error occured! Try again!");
                    return SelectPosition();
                }
            }
        }

        public int GetValue(int row, int col)
        {
            return Slot[row, col];
        }

        public void SetPosition(int row, int col, int number)
        {
            Slot[row, col] = number;
        }


        // Check if all slots are full
        public bool IsBoardFull()
        {
            for (int i = 0; i < Size; ++i)
            {
                for (int j = 0; j < Size; ++j)
                {
                    if (Slot[i, j] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public int[,] GetBoardData()
        {
            return Slot;
        }

        public int[,] SetBoardData(int[,] slot)
        {
            Slot = slot;
            return Slot;
        }
    }
}

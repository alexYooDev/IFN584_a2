using System.IO.Compression;

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
                    if (Slots[i, j] < 10)
                    {
                        Console.Write(Slots[i, j] == 0 ? $"|   " : $"| {Slots[i, j]} ");
                    }
                    else
                    {
                        Console.Write($"| {Slots[i, j]}");
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
            // Create a deep copy of the board state
            int[,] currentBoardState = new int[Size, Size];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    currentBoardState[i, j] = Slots[i, j];
                }
            }
            return currentBoardState;
        }

        public override void SetBoardState(object state)
        {
            if (state is int[,] boardState)
            {
                Slots = boardState;
            }
        }

        public void DisplayGridWithPositions()
        {
            int gridNum = 1;

            // Display the grid with numbers
            for (int i = 0; i < Size; ++i)
            {
                for (int k = 0; k < Size; ++k)
                {
                    Console.Write("------");
                }
                Console.WriteLine();
                Console.Write($"|");
                for (int j = 0; j < Size; ++j)
                {
                    int slotValue = Slots[i, j];

                    // If the slot is empty, display a grid number
                    if (slotValue == 0)
                    {

                        /* Grid layout up to 3 digits numbers */
                        if (gridNum < 10)
                        {
                            Console.Write($"  {gridNum}  |"); // 2 spaces before, 2 after for 1 digit
                        }
                        else if (gridNum < 100)
                        {
                            Console.Write($" {gridNum}  |"); // 1 space before, 2 after for 2 digits
                        }
                        else
                        {
                            Console.Write($"{gridNum}  |"); // no space before, 2 after for 3 digits
                        }
                    }
                    // If it is taken, blank slot is displayed instead
                    else
                    {
                        Console.Write("     |");
                    }
                    gridNum++;
                }
                Console.WriteLine();
            }
            for (int k = 0; k < Size; ++k)
            {
                Console.Write("------");
            }
            Console.WriteLine();
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
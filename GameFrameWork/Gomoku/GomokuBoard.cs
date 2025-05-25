namespace GameFrameWork
{
    using System;
    using System.Collections.Generic;

    public class GomokuBoard : AbstractBoard
    {
        private char[,] Slots;

        public GomokuBoard(int size, int boardCount) : base(size, boardCount = 1)
        {
            Size = size;
            Slots = new char[size, size];
            BoardsState = new List<object> { Slots };

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Slots[i, j] = '.';
                }
            }
        }

        public override void DisplayBoard(int boardIndex = 0)
        {
            Console.WriteLine("\n|| +++ Current Gomoku Board Status +++ ||\n");
            
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
                    char slotValue = Slots[i, j];
                    string cellContent;
                    
                    if (slotValue == '.')
                    {
                        // Empty slot - show as dot centered
                        cellContent = "  .  ";
                    }
                    else
                    {
                        // Stone (X or O) - center it
                        cellContent = $"  {slotValue}  ";
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
            if (row < 0 || row >= Size || col < 0 || col >= Size)
            {
                if (displayMessages) Console.WriteLine("Your move exceeds the board! Please try again!");
                return false;
            }
            
            if (Slots[row, col] != '.')
            {
                if (displayMessages) Console.WriteLine("This position is already occupied! Please try again!");
                return false;
            }
            
            return true;
        }

        public override void MakeMove(int row, int col, object moveData = null, int boardIndex = 0)
        {
            if (moveData is char symbol)
            {
                Slots[row, col] = symbol;
            }
        }

        public override bool IsBoardFull(int boardIndex = 0)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (Slots[i, j] == '.')
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        public override object GetBoardState()
        {
            char[,] currentBoardState = new char[Size, Size];

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
            if (state is char[,] newState)
            {
                Slots = newState;
                BoardsState[0] = Slots;
            }
        }

        public char GetValue(int row, int col)
        {
            return Slots[row, col];
        }

        public void SetPosition(int row, int col, char symbol)
        {
            Slots[row, col] = symbol;
        }

        public int[] SelectPosition()
        {
            while (true)
            {
                try
                {
                    // Display the grid with numbers
                    Console.WriteLine("\n|| +++ Select a position to place your stone +++ ||\n");

                    DisplayGridWithPositions();  // NEW: Show numbered grid
                    Console.Write($"\nWhere on the board do you want to place your stone? (1 - {Size * Size}) >> ");
                    int position = Convert.ToInt32(Console.ReadLine());

                    if (position < 1 || position > Size * Size)  // NEW: Range validation
                    {
                        Console.WriteLine("\nYou exceed the range of the board! Try again!");
                        continue;
                    }

                    int row = (position - 1) / Size;  // NEW: Convert number to coordinates
                    int col = (position - 1) % Size;  // NEW: Convert number to coordinates

                    /* When the position is already taken */
                    if (Slots[row, col] != '.')
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

        public void DisplayGridWithPositions()
        {
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
                    char slotValue = Slots[i, j];
                    string cellContent;
                    
                    // If the slot is empty, display a grid number
                    if (slotValue == '.')
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
                    // If it is taken, display the piece (X or O)
                    else
                    {
                        cellContent = $"  {slotValue}  ";
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

        public List<(int, int)> CheckForWinningCondition(int row, int col, char symbol)
        {
            /* row, col, diagonal, anti-diagonal directions */
            int[,] directions = { { 0, 1 }, { 1, 0 }, { 1, 1 }, { 1, -1 } };
            const int WIN_ROW_NUMBER = 5;

            for (int d = 0; d < 4; d++)
            {
                int dx = directions[d, 0];
                int dy = directions[d, 1];

                List<(int, int)> line = new List<(int, int)> { (row, col) };

                for (int i = 1; i < WIN_ROW_NUMBER; i++)
                {
                    int newRow = row + i * dx;
                    int newCol = col + i * dy;

                    if (newRow >= 0 && newRow < Size && newCol >= 0 && newCol < Size && Slots[newRow, newCol] == symbol)
                    {
                        line.Add((newRow, newCol));
                    }
                    else break;
                }

                /* Checkig negative direction */

                for (int i = 1; i < WIN_ROW_NUMBER; i++)
                {
                    int newRow = row - i * dx;
                    int newCol = col - i * dy;

                    if (newRow >= 0 && newRow < Size && newCol >= 0 && newCol < Size && Slots[newRow, newCol] == symbol)
                    {
                        line.Insert(0, (newRow, newCol));
                    }
                    else break;
                }

                if (line.Count >= WIN_ROW_NUMBER)
                {
                    // Return 5 positions
                    return line.GetRange(0, WIN_ROW_NUMBER);
                }
            }
            return null;
        }
    }
}
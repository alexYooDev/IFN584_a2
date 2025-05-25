namespace GameFrameWork
{
    public class NotaktoBoard : AbstractBoard
    {
        private List<char[,]> Boards;
        private List<int> DeadBoards;

        public NotaktoBoard() : base(3, 3) // 3x3 size, 3 boards
        {
            Size = 3;
            BoardCount = 3;
            Boards = new List<char[,]>();
            DeadBoards = new List<int>();
            BoardsState = new List<object>();

            InitializeBoards();
        }

        private void InitializeBoards()
        {
            for (int i = 0; i < BoardCount; i++)
            {
                char[,] board = new char[Size, Size];
                for (int row = 0; row < Size; row++)
                {
                    for (int col = 0; col < Size; col++)
                    {
                        board[row, col] = ' ';
                    }
                }
                Boards.Add(board);
                BoardsState.Add(board);
            }
        }

        /* Oriented to landscape Better view in a single glance */

        public override void DisplayBoard(int boardIndex = 0)
        {
            Console.WriteLine("\n|| +++ Current Notakto Board Status +++ ||\n");

            int cellWidth = 6;
            // Print board headers
            for (int b = 0; b < BoardCount; ++b)
            {
                int totalBoardWidth = Size * cellWidth + 1; // +1 for the starting '|'
                string header = $"Board {b + 1}{(DeadBoards.Contains(b) ? " [DEAD]" : "")}";
                Console.Write(header.PadRight(totalBoardWidth));
                if (b < BoardCount - 1) Console.Write("   ");
            }
            Console.WriteLine();

            for (int i = 0; i < Size; ++i)
            {
                // Print horizontal lines for all boards
                for (int b = 0; b < BoardCount; ++b)
                {
                    for (int k = 0; k < Size; ++k)
                    {
                        Console.Write("+-----");
                    }
                    Console.Write("+");
                    if (b < BoardCount - 1) Console.Write("   ");
                }
                Console.WriteLine();

                // Print row for all boards
                for (int b = 0; b < BoardCount; ++b)
                {
                    for (int j = 0; j < Size; ++j)
                    {
                        char cell = GetValue(b, i, j);
                        string cellStr = cell == ' ' ? "     " : $"  {cell}  ";
                        Console.Write($"|{cellStr}");
                    }
                    Console.Write("|");
                    if (b < BoardCount - 1) Console.Write("   ");
                }
                Console.WriteLine();
            }
            // Print bottom lines for all boards
            for (int b = 0; b < BoardCount; ++b)
            {
                for (int k = 0; k < Size; ++k)
                {
                    Console.Write("+-----");
                }
                Console.Write("+");
                if (b < BoardCount - 1) Console.Write("   ");
            }
            Console.WriteLine();
        }

        public override bool IsValidMove(int row, int col, object moveData, int boardIndex = 0, bool displayMessages = true)
        {
            // Check if board number is valid (1 to 3) 
            if (boardIndex < 0 || boardIndex >= BoardCount)
            {
                if (displayMessages)
                {
                     Console.WriteLine("The board you selected is out of range! Please select a valid one!");
                }
                return false;
            }

            // Check if row and column are within the boundary of the board
            if (row < 0 || row >= Size || col < 0 || col >= Size)
            {
                if (displayMessages)
                {
                     Console.WriteLine("Your move exceeds the board! Please try again!");
                }
                return false;
            }

             // Check if the board is dead
            if (DeadBoards.Contains(boardIndex))
            {
                if (displayMessages)
                {
                    Console.WriteLine("The board you selected is already dead! Please select other board!");
                }
                return false;
            }

            // Check if the cell is already occupied
            if (Boards[boardIndex][row, col] != ' ')
            {
                if (displayMessages)
                {
                    Console.WriteLine("This slot is already occupied! Please try again!");
                }
                return false;
            }

            return true;
        }

        public override void MakeMove(int row, int col, object moveData = null, int boardIndex = 0)
        {
            if (IsValidMove(row, col, moveData, boardIndex, false))
            {
                Boards[boardIndex][row, col] = 'X';

                if (CheckThreeInARow(boardIndex) && !DeadBoards.Contains(boardIndex))
                {
                    DeadBoards.Add(boardIndex);
                }
            }
        }

        public override bool IsBoardFull(int boardIndex = 0)
        {
            //notakto never get full
            return false;
        }

        public bool AreAllBoardsDead()
        {
            return DeadBoards.Count == BoardCount;
        }

        public override object GetBoardState()
        {
            List<char[,]> currentBoardState = new List<char[,]>(); // a list to store three boards
            for (int b = 0; b < BoardCount; ++b)
            {
                char[,] saved = new char[Size, Size];
                for (int i = 0; i < Size; ++i)
                {
                    for (int j = 0; j < Size; ++j)
                    {
                        saved[i, j] = Boards[b][i, j];
                    }
                }
                currentBoardState.Add(saved);
            }
            List<int> currentDeadBoards = new List<int>(DeadBoards);
            return Tuple.Create(currentBoardState, currentDeadBoards);
        }

        public override void SetBoardState(object state)
        {
            if (state is Tuple<List<char[,]>, List<int>> boardState)
            {
                // Use current boards if provided boards are invalid
                if (boardState.Item1 != null && boardState.Item1.Count == BoardCount)
                {
                    Boards = boardState.Item1;
                }
                
                // Always update dead boards (with validation)
                DeadBoards.Clear();
                if (boardState.Item2 != null)
                {
                    foreach (int deadBoard in boardState.Item2)
                    {
                        if (deadBoard >= 0 && deadBoard < BoardCount)
                        {
                            DeadBoards.Add(deadBoard);
                        }
                    }
                }

                // Update BoardsState
                BoardsState.Clear();
                foreach (var board in Boards)
                {
                    BoardsState.Add(board);
                }
            }
        }

        // pass index for chcekcing if the 3 line is present in the corresponding boardIndex - Alex
        public bool CheckThreeInARow(int boardIndex)
        {
            char[,] board = Boards[boardIndex];

            for (int i = 0; i < Size; i++)
            {
                // Check horizontal and vertical
                if (board[i, 0] == 'X' && board[i, 1] == 'X' && board[i, 2] == 'X') return true;
                if (board[0, i] == 'X' && board[1, i] == 'X' && board[2, i] == 'X') return true;
            }

            // Check diagonals
            if (board[0, 0] == 'X' && board[1, 1] == 'X' && board[2, 2] == 'X') return true;
            if (board[0, 2] == 'X' && board[1, 1] == 'X' && board[2, 0] == 'X') return true;

            return false;
        }

        public bool IsBoardDead(int boardIndex)
        {
            return DeadBoards.Contains(boardIndex);
        }

        public char GetValue(int boardIndex, int row, int col)
        {
            return Boards[boardIndex][row, col];
        }

        public int[] SelectPosition()
        {
            while (true)
            {
                try
                {
                    // Display the grid with numbers
                    DisplayGridWithPositions();

                    Console.Write($"\nWhich board do you want to place X? (1 - {BoardCount}) >> ");
                    int selectedBoard = Convert.ToInt32(Console.ReadLine()) - 1;

                    if (selectedBoard < 0 || selectedBoard >= BoardCount)
                    {
                        Console.WriteLine("Invalid board number! Try again!");
                        continue;
                    }

                    if (IsBoardDead(selectedBoard))
                    {
                        Console.WriteLine("The board you selected is already dead! Please select another board!");
                        continue;
                    }

                    Console.Write($"\nWhere in board {selectedBoard + 1} do you want to place X? (1 - {Size * Size}) >> ");
                    int position = Convert.ToInt32(Console.ReadLine());

                    if (position < 1 || position > Size * Size)
                    {
                        Console.WriteLine("\nYou exceed the range of the board! Try again!");
                        continue;
                    }

                    int row = (position - 1) / Size;
                    int col = (position - 1) % Size;


                    /* When the position is already taken */
                    if (Boards[selectedBoard][row, col] != ' ')
                    {
                        Console.WriteLine("This position is already taken! Try again!");
                        continue;
                    }

                    return new int[] { selectedBoard, row, col };
                }
                catch (FormatException)
                {
                    Console.WriteLine("Your input should be a number! Try again!");
                }
                catch (Exception)
                {
                    Console.WriteLine("Unexpected error occurred! Try again!");
                }
            }
        }

        /* Boards Display orientation to landscape => better view in a single glance */

        private void DisplayGridWithPositions()
        {
            Console.WriteLine("\n|| +++ Select Board and Position +++ ||\n");

            // Print board headers
            int cellWidth = 6;
            for (int b = 0; b < BoardCount; b++)
            {
                // Calculate left padding for the board header
                int totalBoardWidth = Size * cellWidth + 1; // +1 for the starting '|'
                string header = $"Board {b + 1}{(DeadBoards.Contains(b) ? " [DEAD]" : "")}";
                Console.Write(header.PadRight(totalBoardWidth));
                if (b < BoardCount - 1) Console.Write("   ");
            }
            Console.WriteLine();

            for (int i = 0; i < Size; i++)
            {
                // Print horizontal lines for all boards
                for (int b = 0; b < BoardCount; b++)
                {
                    for (int k = 0; k < Size; k++)
                    {
                        Console.Write("+-----");
                    }
                    Console.Write("+");
                    if (b < BoardCount - 1) Console.Write("   ");
                }
                Console.WriteLine();

                // Print row for all boards
                for (int b = 0; b < BoardCount; b++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        char slotValue = Boards[b][i, j];
                        int displayNum = (i * Size + j) + 1;
                        string cell;
                        if (slotValue == ' ')
                        {
                            cell = displayNum < 10 ? $"  {displayNum}  " : $" {displayNum}  ";
                        }
                        else
                        {
                            cell = "  X  ";
                        }
                        Console.Write($"|{cell}");
                    }
                    Console.Write("|");
                    if (b < BoardCount - 1) Console.Write("   ");
                }
                Console.WriteLine();
            }
            // Print bottom lines for all boards
            for (int b = 0; b < BoardCount; b++)
            {
                for (int k = 0; k < Size; k++)
                {
                    Console.Write("+-----");
                }
                Console.Write("+");
                if (b < BoardCount - 1) Console.Write("   ");
            }
            Console.WriteLine("\n");
        }
    }
}
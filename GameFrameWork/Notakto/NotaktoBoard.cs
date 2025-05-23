// define Notakto board, make and check valid move, board logic(whether a board is full)

using System;
using System.Collections.Generic;


namespace GameFrameWork
{
    public class NotaktoBoard : AbstractBoard
    {
        //properties
        private List<char[,]> Boards = new List<char[,]>();
        // present three boards in 2D array (slot)
        private List<int> DeadBoards = new List<int>();    // store dead board

        public NotaktoBoard() : base(3, 3) // 3*3, 3 boards
        {
            Boards = new List<char[,]>();
            DeadBoards = new List<int>();
            BoardsState = new List<object>();

            InitializeBoards();
        }

        // called when first time launch the Notakto game
        private void InitializeBoards()
        {
            for (int i = 0; i < BoardCount; ++i)
            {
                char[,] board = new char[Size, Size];
                for (int row = 0; row < Size; ++row)
                {
                    for (int col = 0; col < Size; ++col)
                    {
                        board[row, col] = ' ';
                    }
                }
                Boards.Add(board);
            }
        }



        public override void DisplayBoard(int boardIndex)
        {
            Console.WriteLine("\n|| +++ Current Board Status +++ ||\n");
            for (int b = 0; b < BoardCount; ++b)
            {
                Console.WriteLine($"Board {b + 1}{(DeadBoards.Contains(b) ? " [DEAD]" : "")}:");
                for (int i = 0; i < Size; ++i)
                {
                    for (int k = 0; k < Size; ++k)
                    {
                        Console.Write("----");
                    }
                    Console.WriteLine();

                    for (int j = 0; j < Size; ++j)
                    {
                        char cell = GetValue(b, i, j);
                        Console.Write(cell == ' ' ? "|   " : $"| {cell} ");
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
        }
        public override bool IsValidMove(int row, int col, object moveData, int boardIndex = 0, bool displayMessages = true)

        {
            // Check if board number is valid (1 to 3) 
            if (boardIndex < 0 || boardIndex >= BoardCount)
            {
                if (displayMessages)
                    Console.WriteLine("The board you selected is out of range! Please select a valid one!");
                return false;
            }
            // Check if row and column are within the boundary of the board
            if (row < 0 || row >= Size || col < 0 || col >= Size)
            {
                if (displayMessages)
                    Console.WriteLine("Your move exceeds the board! Please try again!");
                return false;
            }

            // Check if the board is dead
            if (DeadBoards.Contains(boardIndex))
            {
                if (displayMessages)
                    Console.WriteLine("The board you selected is already dead! Please select other board!");
                return false;
            }

            // Check if the cell is already occupied
            if (Boards[boardIndex][row, col] != ' ')
            {
                if (displayMessages)
                    Console.WriteLine("This slot is already occupied! Please try again!");
                return false;
            }

            // Check valid
            return Boards[boardIndex][row, col] == ' ';
        }

        public override void MakeMove(int row, int col, object? moveData = null, int boardIndex = 0)
        {
            if (!IsValidMove(row, col, moveData, boardIndex))
            {
                throw new InvalidOperationException("Invalid move...Please try again!");
            }

            Boards[boardIndex][row, col] = 'X';

            if (CheckThreeInARow(Boards[boardIndex]) && !DeadBoards.Contains(boardIndex))
            {
                DeadBoards.Add(boardIndex);
            }
        }


        // Add this method to implement the abstract method from AbstractBoard
        public override bool IsBoardFull(int boardIndex = 0)
        {
            return false; //notakto never get full
        }

        public override bool AreAllBoardsDead()
        {
            return DeadBoards.Count == BoardCount;
        }

        public override object GetBoardState()
        {
            // Create a deep copy of the board state
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
            int[] currentDeadBoards = DeadBoards.ToArray();

            return Tuple.Create(currentBoardState, currentDeadBoards);
        }

        public override void SetBoardState(object state)
        {
            if (state is Tuple<List<char[,]>, List<int>> boardState) // boardsState, deadBoards
            {
                Boards = boardState.Item1;
                DeadBoards = boardState.Item2;
            }
        }

        public bool CheckThreeInARow(char[,] board)
        {
            for (int i = 0; i < Size; ++i)
            {
                // check vertical and horizontal
                if (board[i, 0] == 'X' && board[i, 1] == 'X' && board[i, 2] == 'X') return true;
                if (board[0, i] == 'X' && board[1, i] == 'X' && board[2, i] == 'X') return true;
            }
            // check diagonals
            if (board[0, 0] == 'X' && board[1, 1] == 'X' && board[2, 2] == 'X') return true;
            if (board[0, 2] == 'X' && board[1, 1] == 'X' && board[2, 0] == 'X') return true;

            return false;
        }

        public bool IsBoardDead(int boardIndex)
        {
            return DeadBoards.Contains(boardIndex);
        }



        public void DisplayGridWithPositions()
        {
            for (int b = 0; b < BoardCount; ++b)
            {
                Console.WriteLine($"\nBoard {b + 1}{(DeadBoards.Contains(b) ? " [DEAD]" : "")}:");
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
                        char slotValue = Boards[b][i, j];

                        // If the slot is empty, display a grid number
                        if (slotValue == ' ')
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
                            Console.Write("  X  |");
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
        }

        public int[] SelectPosition()
        {
            while (true)
            {
                try
                {
                    // Display the grid with numbers
                    Console.WriteLine("\n|| +++ Select a board and position to put the number +++ ||\n");

                    DisplayGridWithPositions();
                    Console.Write($"\nWhich board do you wat to put the number? (1 - {BoardCount}) >> ");
                    int selectedBoard = Convert.ToInt32(Console.ReadLine()) - 1;

                    while (IsBoardDead(selectedBoard))
                    {
                        Console.WriteLine("The board you selected is already dead! Please select other board!");
                        Console.Write($"\nWhich board do you wat to put the number? (1 - {BoardCount}) >> ");
                        selectedBoard = Convert.ToInt32(Console.ReadLine()) - 1;
                    }

                    Console.Write($"\nWhere in the board {selectedBoard + 1}are you put the number? (1 - {Size * Size}) >> ");
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
                    Console.WriteLine("Your Input should be a number! Try again!");
                }
                catch (Exception)
                {
                    Console.WriteLine("Unexpected error occurred! Try again!");
                }
            }
        }

        public char GetValue(int board, int row, int col)
        {
            return Boards[board][row, col];
        }

        public void SetPosition(int board, int row, int col)
        {
            Boards[board][row, col] = 'X';
        }

        public char[,] GetBoard(int index)
        {
            return Boards[index];
        }


    }
}
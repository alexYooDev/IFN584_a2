// define Notakto board, make and check valid move, board logic(whether a board is full)

using System;
using System.Collections.Generic;
using GameFrameWork;

namespace Notakto
{
    public class NotaktoBoard : AbstractBoard
    {
        private const int BoardIndex = 3; // number of boards
        private const int Size = 3;       // board size, always 3*3
        private List<char[,]> Boards;     // present three boards in 2D array (slot)
        private List<int> DeadBoards;     // store dead board

        public NotaktoBoard() : base(Size)
        {
            Boards = new List<char[,]>();
            DeadBoards = new List<int>();
            BoardsState = new List<object> { Boards };
            InitializeBoards();
        }

        // called when first time launch the Notakto game
        protected override void InitializeBoards()
        {
            for (int i = 0; i < BoardIndex; ++i)
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


        public override void DisplayBoard()
        {
            Console.WriteLine("\n|| +++ Current Board Status +++ ||\n");
            for (int b = 0; b < Boards.Count; b++)
            {
                Console.WriteLine($"Board {b + 1}{(DeadBoards.Contains(b) ? " [DEAD]" : "")}:");
                for (int i = 0; i < Size; i++)
                {
                    for (int k = 0; k < Size; ++k)
                    {
                        Console.Write("----");
                    }
                    Console.WriteLine();

                    for (int j = 0; j < Size; j++)
                    {
                        char cell = Boards[b][i, j];
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
        public override bool IsValidMove(int row, int col, object moveData, int boardIndex = 0)
        {
            // Check if row and column are within the boundary of the board
            if (row < 0 || row >= Size || col < 0 || col >= Size)
            {
                Console.WriteLine("Your move exceeds the board! Please try again!");
                return false;
            }
            // Check if board number is valid (1 to 3) 
            if (boardIndex < 0 || boardIndex >= Boards.Count)
            {
                Console.WriteLine("The board you selected is out of range! Please select a valid one!");
                return false;
            }
            // Check if the board is dead
            if (DeadBoards.Contains(boardIndex))
            {
                Console.WriteLine("The board you selected is already dead! Please select other board!");
                return false;
            }

            // Check if the cell is already occupied
            if (Boards[boardIndex][row, col] != 0)
            {
                Console.WriteLine("This slot is already occupied! Please try again!");
                return false;
            }

            // Check valid
            return Boards[boardIndex][row, col] == ' ';
        }

        public override void MakeMove(int row, int col, object moveData = null, int boardIndex = 0)
        {
            if (!IsValidMove(row, col, moveData, boardIndex))
            {
                throw new InvalidOperationException("Invalid move...Please try again!");
            }

            BoardsState.Add(GetBoardState());
            boards[boardIndex][row, col] = 'X';

            if (CheckThreeInARow(Boards[boardIndex]) && !DeadBoards.Contains(boardIndex))
            {
                deadBoards.Add(boardIndex);
            }
        }


        public bool AreAllBoardsDead()
        {
            return deadBoards.Count == BoardIndex;
        }

        public override object GetBoardState()
        {
            // Create a deep copy of the board state
            char[][,] currentBoardState = new char[Boards.Count][,];
            for (int b = 0; b < Boards.Count; ++b)
            {
                currentBoardState[b] = new char[Size, Size];
                for (int i = 0; i < Size; ++i)
                {
                    for (int j = 0; j < Size; ++j)
                    {
                        currentBoardState[b][i, j] = Boards[b][i, j];
                    }
                }
            }
            int[] currentDeadBoards = DeadBoards.ToArray();

            return Tuple.Create(currentBoardState, currentDeadBoards);
        }

        public override void SetBoardState(object state)
        {
            if (state is Tuple<char[][,], int[]> saved)
            {
                char[][,] loadedBoards = saved.Item1;
                int[] deadBoardIndices = saved.Item2;

                for (int b = 0; b < Boards.Count; ++b)
                {
                    for (int i = 0; i < Size; ++i)
                    {
                        for (int j = 0; j < Size; ++j)
                        {
                            Boards[b][i, j] = loadedBoards[b][i, j];
                        }
                    }
                }

                DeadBoards = new List<int>(deadBoardIndices);
            }
        }
        private bool CheckThreeInARow(char[,] board)
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

        public List<char[,]> GetBoards()
        {
            return Boards;
        }

        public List<int> GetDeadBoards()
        {
            return DeadBoards;
        }
    }
}
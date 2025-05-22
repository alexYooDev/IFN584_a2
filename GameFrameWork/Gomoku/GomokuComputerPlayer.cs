namespace GameFrameWork
{
    public class GomokuComputerPlayer : AbstractComputerPlayer
    {
        /* For computer player to access the Gomoku board */
        private GomokuBoard Board;

        /* move for computers' favorable place */
        private GomokuMove FavorableMove;

        public GomokuComputerPlayer(string name, char symbol) : base("Computer", PlayerType.Computer, symbol) { }

        public override object FindWinningMove(AbstractBoard board)
        {
            /* type casting */
            Board = (GomokuBoard)board;

            char mySymbol = (char)MoveSymbol;
            char opponetSymbol = (mySymbol == 'X') ? 'O' : 'X';

            /* 1. Try finding the winning move first */
            var winningMove = FindWinningMoveForSymbol(mySymbol);
            if (winningMove != null)
            {
                FavorableMove = winningMove;
                return mySymbol;
            }

            /* block opponent's move if it is winning move  */
            var blockMove = FindWinningMoveForSymbol(opponetSymbol);
            if (blockMove != null)
            {
                FavorableMove = blockMove;
                return mySymbol;
            }


            /* find move that is favorable to computers next move */
            var favorableMove = FindFavorableMove(mySymbol);
            if (favorableMove != null)
            {
                FavorableMove = favorableMove;
                return mySymbol; 
            }

            /* if no winning move is found */
            return null;
        }

        private GomokuMove FindWinningMoveForSymbol(char symbol)
        {
            for (int row = 0; row < Board.GetSize(); row++)
            {
                for (int col = 0; col < Board.GetSize(); col++)
                {
                    if (Board.IsValidMove(row, col, symbol, 0, false))
                    {
                        // Save current state
                        object previousState = Board.GetBoardState();

                        // Try the move
                        Board.MakeMove(row, col, symbol);
                        var winLine = Board.CheckForWinningCondition(row, col, symbol);

                        // Restore state
                        Board.SetBoardState(previousState);

                        if (winLine != null)
                            return new GomokuMove(row, col);
                    }
                }
            }
            return null;
        }

        private GomokuMove FindFavorableMove(char symbol)
        {
            int favorableScore = -1;
            GomokuMove favorableMove = null;

            for (int row = 0; row < Board.GetSize(); row++)
            {
                for (int col = 0; col < Board.GetSize(); col++)
                {
                    if (Board.IsValidMove(row, col, symbol, 0, false))
                    {
                        int score = EvaluatePosition(row, col, symbol);
                        if (score > favorableScore)
                        {
                            favorableScore = score;
                            favorableMove = new GomokuMove(row, col);
                        }
                    }
                }
            }

            return favorableMove;
        }

        private int EvaluatePosition(int row, int col, char symbol)
        {
            int score = 0;
            int[,] directions = { { 0, 1 }, { 1, 0 }, { 1, 1 }, { 1, -1 } };

            for (int d = 0; d < 4; d++)
            {
                int dx = directions[d, 0];
                int dy = directions[d, 1];

                int consecutive = CountConsecutive(row, col, dx, dy, symbol);
                score += consecutive * consecutive;
            }

            return score;
        }

        private int CountConsecutive(int row, int col, int dx, int dy, char symbol)
        {
            int count = 1;

            // Count consequetive moves in a row in positive direction 
            for (int i = 1; i < 5; i++)
            {
                int newRow = row + i * dx;
                int newCol = col + i * dy;
                if (newRow >= 0 && newRow < Board.GetSize() && newCol >= 0 && newCol < Board.GetSize() &&
                    Board.GetValue(newRow, newCol) == symbol)
                    count++;
                else break;
            }

            // Count consequetive moves in a row  in negative direction
            for (int i = 1; i < 5; i++)
            {
                int newRow = row - i * dx;
                int newCol = col - i * dy;
                if (newRow >= 0 && newRow < Board.GetSize() && newCol >= 0 && newCol < Board.GetSize() &&
                    Board.GetValue(newRow, newCol) == symbol)
                    count++;
                else break;
            }

            return count;
        }

        public override object SelectRandomMove()
        {
            Random random = new Random();

            // Try to find a random valid position
            for (int attempts = 0; attempts < 100; attempts++)
            {
                int row = random.Next(Board.GetSize());
                int col = random.Next(Board.GetSize());
                if (Board.IsValidMove(row, col, MoveSymbol, 0, false))
                {
                    FavorableMove = new GomokuMove(row, col);
                    return MoveSymbol;
                }
            }

            // Fallback: find first valid move
            for (int row = 0; row < Board.GetSize(); row++)
            {
                for (int col = 0; col < Board.GetSize(); col++)
                {
                    if (Board.IsValidMove(row, col, MoveSymbol, 0, false))
                    {
                        FavorableMove = new GomokuMove(row, col);
                        return MoveSymbol;
                    }
                }
            }
            return null;
        }
        
        public GomokuMove GetFavorableMove()
        {
            return FavorableMove;
        }
    }
}
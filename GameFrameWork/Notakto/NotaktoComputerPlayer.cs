namespace GameFrameWork
{
    public class NotaktoComputerPlayer : AbstractComputerPlayer
    {
        // For computer player to access board
        private NotaktoBoard Board;
        private int[] FavorableMove;

        public NotaktoComputerPlayer() : base("Computer", PlayerType.Computer, 'X')
        {
        }

        public override object FindWinningMove(AbstractBoard board)
        {
            // In Notakto, there are no "winning" moves in the traditional sense
            // Instead, we try to avoid losing moves
            Board = (NotaktoBoard)board;
            
            // Find a move that doesn't immediately lose the game
            var safeMove = FindSafeMove();
            if (safeMove != null)
            {
                FavorableMove = safeMove;
                return 'X';
            }
            
            return null;
        }


        // renamed to make it clear
        private int[] FindSafeMove()
        {

            int boardCount = Board.GetBoardCount();
            for (int b = 0; b < boardCount; ++b)
            {
                if (Board.IsBoardDead(b))
                    continue;

                for (int row = 0; row < boardCount; ++row)
                {
                    for (int col = 0; col < boardCount; ++col)
                    {
                        if (Board.IsValidMove(row, col, null, b, false))
                        {
                            // Simulate the move
                            object previousState = Board.GetBoardState();
                            Board.MakeMove(row, col, null, b);

                            // give boardIndex as argument
                            bool isLosingMove = Board.CheckThreeInARow(b);

                            // Restore state for undo
                            Board.SetBoardState(previousState);

                            if (!isLosingMove)
                                return new int[] { b, row, col };
                        }
                    }
                }
            }
            return null;
        }

        public override object SelectRandomMove()
        {
            // Try to find a safe move first
            var safeMove = FindSafeMove();
            if (safeMove != null)
            {
                FavorableMove = safeMove;
                return 'X';
            }

            // If no safe moves, find any valid move (forced losing move)
            for (int b = 0; b < 3; b++)
            {
                if (Board.IsBoardDead(b))
                    continue;

                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        if (Board.IsValidMove(row, col, null, b, false))
                        {
                            FavorableMove = new int[] { b, row, col };
                            return 'X';
                        }
                    }
                }
            }

            return null;
        }

        public void SetBoard(NotaktoBoard board)
        {
            Board = board;
        }

        public int[] GetFavorableMove()
        {
            return FavorableMove;
        }
    }
}
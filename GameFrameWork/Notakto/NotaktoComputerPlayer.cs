// Handle Notakto computer player logic

using System;
using System.Collections.Generic;


namespace GameFrameWork
{
    public abstract class NotaktoComputerPlayer : AbstractComputerPlayer
    {
        private NotaktoBoard notaktoBoard;
        public NotaktoComputerPlayer() : base("Computer", PlayerType.Computer, 'X')
        {
        }

        public void SetBoard(NotaktoBoard board)
        {
            notaktoBoard = board;
        }

        // Notakto doesn't have a winning move
        public override object? FindWinningMove(AbstractBoard board)
        {
            return null;
        }


        public override object? FindLosingMove(AbstractBoard board)
        {
            notaktoBoard = (NotaktoBoard)board;
            int boardCount = notaktoBoard.GetBoardCount();

            for (int b = 0; b < boardCount; ++b)
            {
                if (notaktoBoard.IsBoardDead(b))
                    continue;

                for (int row = 0; row < 3; ++row)
                {
                    for (int col = 0; col < 3; ++col)
                    {
                        if (notaktoBoard.IsValidMove(row, col, null, b, false))
                        {
                            // Simulate the move
                            object previousState = notaktoBoard.GetBoardState();
                            notaktoBoard.MakeMove(row, col, null, b);

                            bool isLosing = notaktoBoard.CheckThreeInARow(notaktoBoard.GetBoard(b));



                            // Undo simulation
                            notaktoBoard.SetBoardState(previousState);

                            if (isLosing)
                                return new int[] { b, row, col };
                        }
                    }
                }
            }

            return null; // No losing move found
        }




        public override object SelectRandomMove()
        {
            List<int[]> safeMoves = new List<int[]>();
            int boardCount = notaktoBoard.BoardCount;

            for (int b = 0; b < boardCount; ++b)
            {
                if (notaktoBoard.IsBoardDead(b))
                    continue;

                for (int row = 0; row < 3; ++row)
                {
                    for (int col = 0; col < 3; ++col)
                    {
                        if (!notaktoBoard.IsValidMove(row, col, null, b, false))
                            continue;

                        // Simulate
                        object previousState = notaktoBoard.GetBoardState();
                        notaktoBoard.MakeMove(row, col, null, b);

                        bool isLosing = notaktoBoard.CheckThreeInARow(notaktoBoard.GetBoard(b));



                        notaktoBoard.SetBoardState(previousState);

                        //save all moves that are not losingmove, tham random pickup one
                        if (!isLosing)
                            safeMoves.Add(new int[] { b, row, col });
                    }
                }
            }

            if (safeMoves.Count > 0)
            {
                Random rand = new Random();
                return safeMoves[rand.Next(safeMoves.Count)];
            }

            // If no safe moves, just take the first valid one (forced losing move)
            for (int b = 0; b < boardCount; ++b)
            {
                if (notaktoBoard.IsBoardDead(b))
                    continue;

                for (int row = 0; row < 3; ++row)
                {
                    for (int col = 0; col < 3; ++col)
                    {
                        if (notaktoBoard.IsValidMove(row, col, null, b, false))
                            return new int[] { b, row, col };
                    }
                }
            }

            throw new InvalidOperationException("There is no valid moves available for computer player.");
        }
    }
}

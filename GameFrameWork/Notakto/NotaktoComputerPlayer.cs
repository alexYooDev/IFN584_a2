// Handle Notakto computer player logic

using System;
using System.Collections.Generic;
using GameFrameWork;


namespace Notakto
{
    public abstract class NotaktoComputerPlayer : AbstractComputerPlayer
    {
        // player type : Human or Computer
        public string Type { get; }

        // For computer player to access board
        private NotaktoBoard Board;


        public NotaktoComputerPlayer(string name, PlayerType type) : base("Computer", PlayerType.Computer, 'X')
        {
        }

        // Notakto doesn't have a winning move
        public override object FindWinningMove(AbstractBoard board)
        {
            return null;
        }


        public override object FindLosingMove(AbstractBoard board)
        {
            NotaktoBoard notaktoBoard = (NotaktoBoard)board;
            int boardCount = board.GetBoardCount();

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

                            bool isLosing = notaktoBoard.CheckThreeInARow(b);

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
            var notaktoBoard = (NotaktoBoard)NotaktoReference;

            List<int[]> safeMoves = new List<int[]>();

            for (int b = 0; b < 3; ++b)
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

                        bool isLosing = notaktoBoard.CheckThreeInARow(b);

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

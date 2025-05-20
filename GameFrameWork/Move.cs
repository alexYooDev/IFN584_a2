namespace GameFrameWork
{
    public class Move
    {
        /* For navigating multiple boards in Notakto */
        public int BoardIndex { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        
        /* stored player information */
        public AbstractPlayer Player { get; set; }
        // game specific move data
        public object MoveData { get; set; }
        // Necessary for undoing / redoing move
        public object PreviousBoardState { get; set; }

        public Move(int boardIndex, int row, int col, AbstractPlayer player, object moveData, object previousBoardState)
        {
            BoardIndex = boardIndex;
            Row = row;
            Col = col;
            Player = player;
            MoveData = moveData;
            PreviousBoardState = previousBoardState;
        }
    }
}
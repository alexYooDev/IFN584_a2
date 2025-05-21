namespace GameFrameWork
{
    public class Move
    {
        public int BoardIndex { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public AbstractPlayer Player { get; set; }
        public object MoveData { get; set; }  // Can be a number, symbol, etc.
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
namespace GameFrameWork
{
    public class Move
    {
        public int BoardIndex { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public AbstractPlayer Player { get; set; }
        // numbers = Numerical TTT , symbol = Gomoku, Notakto, etc.
        public object MoveData { get; set; }
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
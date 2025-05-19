namespace GameFrameWork
{
    public class Move
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public object Value { get; set; }
        // game specific move data
        public object MoveData { get; set; }
        // Necessary for undoing / redoing move
        public object PreviousBoardState { get; set; }

        public Move(int row, int col, object value, object moveData, object previousBoardState)
        {
            Row = row;
            Col = col;
            Value = value;
            MoveData = moveData;
            PreviousBoardState = previousBoardState;
        }
    }
}
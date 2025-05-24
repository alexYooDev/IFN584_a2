namespace GameFrameWork
{
    public class TicTacToeMove
    {
        public int BoardIndex { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }

        public TicTacToeMove(int boardIndex, int row, int col)
        {
            BoardIndex = boardIndex;
            Row = row;
            Col = col;
        }
    }
}
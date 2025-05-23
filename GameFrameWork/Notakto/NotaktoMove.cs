namespace GameFrameWork
{
    public class NotaktoMove
    {
        public int BoardIndex { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }

        public NotaktoMove(int boardIndex, int row, int col)
        {
            BoardIndex = boardIndex;
            Row = row;
            Col = col;
        }
    }
}
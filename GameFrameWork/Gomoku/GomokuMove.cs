namespace GameFrameWork
{
    public class GomokuMove
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public GomokuMove(int row, int col)
        {
            Row = row;
            Col = col;
        }        
    }
}
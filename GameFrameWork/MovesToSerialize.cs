namespace GameFrameWork
{
    // Helping class for formating data to be serialized
    public class MovesToSerialize
    {
        public int BoardIndex { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public string PlayerName { get; set; }
        public int MoveData { get; set; }
        public int[][] PreviousBoardState { get; set; }
    }
}
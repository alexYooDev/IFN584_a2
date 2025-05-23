namespace GameFrameWork
{
    public class NotaktoMovesToSerialize
    {
        public int BoardIndex { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public string PlayerName { get; set; }
        public char MoveData { get; set; }
        public List<string[][]> PreviousBoardState { get; set; }
    }
}
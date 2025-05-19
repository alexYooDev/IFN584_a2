namespace GameFrameWork
{
    public abstract class GameData
    {
        public int BoardSize { get; set; }
        public string GameMode { get; set; }
        public Player CurrentPlayer { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public string GameType { get; set; }
        public bool IsGameOver { get; set; }
        public object BoardState { get; set; }
    }
}
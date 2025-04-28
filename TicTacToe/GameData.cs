namespace TicTacToe
{
    public class GameData
    {
        public int BoardSize { get; set; }
        public string GameMode { get; set; }
        public bool Turn { get; set; }
        public int[][] BoardState {get; set;}
        public string CurrentPlayer1Name { get; set; }
        public string CurrentPlayer2Name { get; set; }
        public bool IsGameOver { get; set; }
        public HashSet<int> RemainingOdds { get; set; }
        public HashSet<int> RemainingEvens { get; set; }
        public int TargetSum { get; set; }
        public bool CurrentTurn { get; set;}
    }
}
namespace GameFrameWork
{
    public class TicTacToeGameData : GameData
    {
        public int TargetSum { get; set; }
        public HashSet<int> Player1Numbers { get; set; }
        public HashSet<int> Player2Numbers { get; set; }
    }
}
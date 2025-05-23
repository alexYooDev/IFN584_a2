namespace GameFrameWork
{
    public interface IGameData
    {
        string GameType { get; set; }
        string GameMode { get; set; }
        string CurrentPlayerName { get; set; }
        string Player1Name { get; set; }
        string Player2Name { get; set; }
        bool IsGameOver { get; set; }
        List<MovesToSerialize> MoveHistory { get; set; }
        List<MovesToSerialize> RedoHistory { get; set; }
    }
}
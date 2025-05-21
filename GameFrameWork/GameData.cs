namespace GameFrameWork
{
    using System.Collections.Generic;

    public abstract class GameData
    {
        public int BoardSize { get; set; }
        public int BoardCount { get; set; } = 1;  // For multi-board games like Notakto
        public string GameMode { get; set; }
        public string CurrentPlayerName { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public List<object> Player1Moves { get; set; } // Available moves for player 1
        public List<object> Player2Moves { get; set; } // Available moves for player 2
        public string GameType { get; set; }
        public bool IsGameOver { get; set; }
        public object BoardState { get; set; }
        public List<MovesToSerialize> MoveHistory { get; set; }
        public List<MovesToSerialize> RedoHistory { get; set; }
        public int? UndoneMovesCount { get; set; }

        // Game-specific data
        public Dictionary<string, object> GameSpecificData { get; set; }
    }
}
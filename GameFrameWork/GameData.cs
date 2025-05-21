namespace GameFrameWork
{
    using System.Collections.Generic;
    
    public abstract class GameData
    {
        public int BoardSize { get; set; }
        public string GameMode { get; set; }
        public AbstractPlayer CurrentPlayer { get; set; }
        public AbstractPlayer Player1 { get; set; }
        public AbstractPlayer Player2 { get; set; }
        public string GameType { get; set; }
        public bool IsGameOver { get; set; }
        public object BoardState { get; set; }
        public List<object> UndoStack { get; set; }
        public List<object> RedoStack { get; set; }
    }
}
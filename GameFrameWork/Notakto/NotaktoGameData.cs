// handle save/load 

namespace Notakto
{
    public class NotaktoGameData : AbstractGameData
    {
        public string GameMode { get; set; }
        public bool Turn { get; set; }
        public string CurrentPlayer1Name { get; set; }
        public string CurrentPlayer2Name { get; set; }
        public List<int> DeadBoards { get; set; }
        public List<string[][]> Boards { get; set; } // BoardState of Notakto
        public bool CurrentTurn { get; set; }
        public bool IsGameOver { get; set; }
        public bool AreAllBoardsDead { get; set; }

    }
}


namespace GameFrameWork
{
    public abstract class AbstractPlayer
    {
        public string Name { get; set; }
        public PlayerType Type { get; set; }

        // Support different move symbols for different games
        // - TicTacToe: Number type (odd/even)
        // - Notakto/Gomoku: Symbol (X or O)
        public object MoveSymbol { get; set; }

        public AbstractPlayer(string name, PlayerType type, object moveSymbol)
        {
            Name = name;
            Type = type;
            MoveSymbol = moveSymbol;
        }
    }

    public enum PlayerType
    {
        Human,
        Computer
    }

    public enum NumberType
    {
        Even,
        Odd
    }
}
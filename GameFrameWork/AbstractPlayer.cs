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

    public abstract class AbstractHumanPlayer : AbstractPlayer
    {
        public AbstractHumanPlayer(string name, PlayerType type, object moveSymbol) : base(name, type, moveSymbol) { }
        
        // Human player -  method for selecting moves
        public abstract object SelectMove(AbstractBoard board);
    }

    public abstract class AbstractComputerPlayer : AbstractPlayer
    {
        public AbstractComputerPlayer(string name, PlayerType type, object moveSymbol) : base(name, type, moveSymbol) { }

        // For computer player - find winning moves
        public abstract object FindWinningMove(AbstractBoard board);

        // For computer player - select random move when no winning move
        public abstract object SelectRandomMove();
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
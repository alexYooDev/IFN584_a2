namespace GameFrameWork
{
    public abstract class AbstractComputerPlayer : AbstractPlayer
    {
        public AbstractComputerPlayer(string name, PlayerType type, object moveSymbol) : base(name, type, moveSymbol) { }

        // For computer player - find winning moves
        public abstract object FindWinningMove(AbstractBoard board);
        // For computer player - select random move when no winning move (for Notakto, select random move when no losing move )
        public abstract object SelectRandomMove();
    }
}
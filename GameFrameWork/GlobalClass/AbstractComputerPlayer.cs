namespace GameFrameWork
{
    public abstract class AbstractComputerPlayer : AbstractPlayer
    {
        public AbstractComputerPlayer(string name, PlayerType type, object moveSymbol) : base(name, type, moveSymbol) { }

        public abstract object FindWinningMove(AbstractBoard board);
        public abstract object SelectRandomMove();
    }
}
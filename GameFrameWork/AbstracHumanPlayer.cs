namespace GameFrameWork
{
    public abstract class AbstractHumanPlayer : AbstractPlayer
    {
        public AbstractHumanPlayer(string name, PlayerType type, object moveSymbol) : base(name, type, moveSymbol) { }

        // Human player -  method for selecting moves
        public abstract object SelectMove(AbstractBoard board);
    }
}
namespace GameFrameWork
{
    public class GomokuHumanPlayer : AbstractHumanPlayer
    {
        public GomokuHumanPlayer(string name, char symbol) : base(name, PlayerType.Human, symbol) { }

        public override object SelectMove(AbstractBoard board)
        {
            return MoveSymbol;
        }
    }
}
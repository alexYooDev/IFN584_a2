namespace GameFrameWork
{
    public class NotaktoHumanPlayer : AbstractHumanPlayer
    {
        public NotaktoHumanPlayer(string name) : base(name, PlayerType.Human, 'X')
        {
        }

        public override object SelectMove(AbstractBoard board)
        {
            return 'X'; // Always return X for Notakto
        }
    }
}
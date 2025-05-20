namespace GameFrameWork
{
    public class HumanPlayer : AbstractPlayer
    {
        public HumanPlayer(string name, PlayerType playerType, object symbol) : base(name, playerType, symbol) { }

        public override string GetName()
        {
            return Name;
        }

        public override PlayerType GetType()
        {
            return Type;
        }
    }
}
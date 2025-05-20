namespace GameFrameWork
{
    public class HumanPlayer : AbstractPlayer
    {
        public HumanPlayer(string name, PlayerType playerType) : base(name, playerType) { }

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
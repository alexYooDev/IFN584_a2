namespace GameFrameWork
{
    public class ComputerPlayer : AbstractPlayer
    {
        public ComputerPlayer(string name, PlayerType type, object symbol) : base(name, type, symbol) { }

        public override string GetName()
        {
            return "Computer";
        }
        public override PlayerType GetType()
        {
            return Type;
        }
    }
}

namespace GameFrameWork
{
    public class ComputerPlayer : AbstractPlayer
    {
        public ComputerPlayer(string name, PlayerType type) : base(name, type) { }

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

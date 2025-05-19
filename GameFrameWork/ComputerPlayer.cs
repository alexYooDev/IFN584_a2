public class ComputerPlayer : AbstractPlayer
    {
        public HumanPlayer(PlayerType playerType, object Symbol) : base(AbstractPlayer)

        public override Move GetMove()
        {
            throw new System.NotImplementedException();
        }

        public override string GetName()
        {
            return "Computer";
        }
    }
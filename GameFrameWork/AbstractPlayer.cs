namespace GameFrameWork
{
    public abstract class AbstractPlayer
    {
        public string Name { get; set; }
        public PlayerType PlayerType { get; set; }
        public object Symbol { get; set; }
        // private HashSet<int> GivenNumbers { get; set; }

        public AbstractPlayer(string name, PlayerType playerType, object symbol)
        {
            Name = name;
            PlayerType = playerType;
            Symbol = symbol;
        }

        public abstract Move GetMove();

        public abstract string GetName();
    }

    public enum PlayerType
    {
        Human,
        Computer
    }
}
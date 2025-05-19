namespace GameFrameWork
{
    public abstract class Player
    {
        public string Name { get; set; }
        public PlayerType PlayerType { get; set; }
        public object Symbol { get; set; }
        // private HashSet<int> GivenNumbers { get; set; }

        public Player(string name, PlayerType playerType, object symbol)
        {
            Name = name;
            PlayerType = playerType;
            Symbol = symbol;
        }

        public abstract Move GetMove();
    }

    public enum PlayerType
    {
        Human,
        Computer
    }
}
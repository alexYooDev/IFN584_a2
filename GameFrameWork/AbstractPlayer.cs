namespace GameFrameWork
{
    public abstract class AbstractPlayer
    {
        public string Name { get; set; }
        public PlayerType Type { get; set; }
        public char? SymbolChar { get; set; }
        public NumberType? NumberType { get; set; }


        /* For dynamically assiging move symbols :
            Add SymbolChar for Notakto and Gomoku
            Add NumberType for numerical tictactoe
        */
        public object MoveSymbol
        {
            get
            {
                if (SymbolChar.HasValue)
                    return SymbolChar.Value;
                if (NumberType.HasValue)
                    return NumberType.Value;

                // else -> maybe replace to proper exception
                return null;
            }
        }

        public AbstractPlayer(string name, PlayerType type)
        {
            Name = name;
            Type = type;
        }

        public virtual string GetName()
        {
            return Name;
        }

        public virtual PlayerType GetType() {
            return Type;
        }
    }

    public enum PlayerType
    {
        Human,
        Computer
    }

    public enum NumberType
    {
        Even,
        Odd
    }
}
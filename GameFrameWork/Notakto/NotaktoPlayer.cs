// Handle Notakto player logic

using System;
using System.Collections.Generic;
using GameFrameWork;


namespace Notakto
{
    public abstract class NotaktoPlayer : AbstractPlayer
    {
        // player type : Human or Computer
        public string Type { get; }
        public override object MoveSymbol
        {
            public char SymbolChar { get; }
        }
        public NotaktoPlayer(string name, PlayerType type) : base(name, type)
        {
            {
                SymbolChar = 'X';
            }
        }
    }

    public class NotaktoHumanPlayer : NotaktoPlayer
{
    // users name
    public string Name { get; }
    public NotaktoHumanPlayer(string name, string type) : base(name, type)
    {
        Name = name;
    }

    public override string GetName()
    {
        return Name;
    }
}


public class NotaktoComputerPlayer : NotaktoPlayer
{
    public NotaktoComputerPlayer(string name, string type) : base(name, type)
    {
            public override string GetName()
    {
        return 'Computer';
    }
}    
    }
}



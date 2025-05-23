// Handle Notakto Human player logic

using System;
using System.Collections.Generic;
using GameFrameWork;


namespace Notakto
{
    public abstract class NotaktoHumanPlayer : AbstractHumanPlayer
    {
        // player type : Human or Computer
        public NotaktoHumanPlayer(string name)
            : base(name, PlayerType.Human, 'X')
        {
        }


        public override object SelectMove(AbstractBoard board)
        {
            Board notaktoBoard = (NotaktoBoard)board;
            Console.WriteLine($"\n{name}, it's your turn now");

            int[] position = SelectPosition(notaktoBoard, selectedBoard);
            return new int[] { selectedBoard, position[0], position[1] };
        }
    }
}
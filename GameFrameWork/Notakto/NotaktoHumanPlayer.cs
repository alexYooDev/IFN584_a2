// Handle Notakto Human player logic

using System;
using System.Collections.Generic;



namespace GameFrameWork
{
    public class NotaktoHumanPlayer : AbstractHumanPlayer
    {
        // player type : Human or Computer
        public NotaktoHumanPlayer(string name)
            : base(name, PlayerType.Human, 'X')
        {
        }


        public override object SelectMove(AbstractBoard board)
        {
            NotaktoBoard notaktoBoard = (NotaktoBoard)board;

            Console.WriteLine($"\n{this.Name}, it's your turn now");


            int[] move = notaktoBoard.SelectPosition();


            return move;
        }
    }
}
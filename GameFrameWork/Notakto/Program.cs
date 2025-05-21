//entry point

using System;
using System.Collections.Generic;
using Notakto;

internal class Program
{
    private static void Main(string[] args)
    {
        NotaktoGameManager notaktoGame = new NotaktoGameManager();
        notaktoGame.Run();
    }
}
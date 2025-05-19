namespace GameFrameWork
{
    public static class GameFactory
    {
        public static AbstractGame CreateGame(string gameType)
        {
            switch (gameType.ToLower())
            {
                case "numericaltictactoe":
                case "numerical tictactoe":
                    return new NumericalTicTacToe(3); // new NumericalTicTacToe
                case "notakto":
                    return; // new NoTakTo
                case "gomoku":
                    return; // new Gomoku
                default:
                    throw new ArgumentException($"Game Type is not valid. ${gameType}, Try again");
            } 
        }
    }
}
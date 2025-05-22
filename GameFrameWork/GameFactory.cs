namespace GameFrameWork
{
    public static class GameFactory
    {
        public static AbstractGame CreateGame(string gameType)
        {
            switch (gameType.ToLower())
            {
                case "numericaltictactoe":
                    return new TicTacToeGame();
                case "notakto":
                    // This will be implemented in a future update
                    throw new NotImplementedException("Notakto game is not implemented yet");
                case "gomoku":
                    // This will be implemented in a future update
                    return new GomokuGame();
                default:
                    throw new ArgumentException($"Game Type is not valid: {gameType}. Try again");
            } 
        }
    }
}
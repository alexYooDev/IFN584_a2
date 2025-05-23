namespace GameFrameWork
{
    public static class GameFactory
    {
        public static AbstractGame CreateGame(string gameType)
        {
            var renderer = new ConsoleGameRenderer();
            var inputHandler = new ConsoleInputHandler();
            var dataPersistence = new JsonGameDataPersistence();

            switch (gameType.ToLower())
            {
                case "numericaltictactoe":
                    return new TicTacToeGame(renderer, inputHandler, dataPersistence);
                case "notakto":
                    return new NotaktoGame(renderer, inputHandler, dataPersistence);
                case "gomoku":
                    // This will be implemented in a future update
                    return new GomokuGame(renderer, inputHandler, dataPersistence);
                default:
                    throw new ArgumentException($"Game Type is not valid: {gameType}. Try again");
            } 
        }
    }
}
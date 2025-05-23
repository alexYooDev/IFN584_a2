namespace GameFrameWork
{
    public class TicTacToeGameData : GameData
    {
        public int TargetSum { get; set; }
        public List<int> Player1Moves { get; set; } = new List<int>();
        public List<int> Player2Moves { get; set; } = new List<int>();
        public int[][] BoardState { get; set; }



        public TicTacToeGameData()
        {
            GameType = "NumericalTicTacToe";
            Player1Moves = new List<int>();
            Player2Moves = new List<int>();
        }

        // POPULATE DATA FROM GAME
        public override void PopulateFromGame(AbstractGame game)
        {
            var ticTacToeGame = (TicTacToeGame)game;
            
            // Common data 
            BoardSize = ticTacToeGame.Board.GetSize();
            GameMode = ticTacToeGame.GetGameMode();
            CurrentPlayerName = ticTacToeGame.GetCurrentPlayerName();
            Player1Name = ticTacToeGame.GetPlayer1Name();
            Player2Name = ticTacToeGame.GetPlayer2Name(); 
            IsGameOver = ticTacToeGame.GetIsGameOver(); 
            
            // TicTacToe-specific data
            TargetSum = ticTacToeGame.GetTargetSum(); 

            // Board state
            int[,] boardArray = (int[,])ticTacToeGame.Board.GetBoardState();
             Console.WriteLine("=== DEBUG BoardState ===");
             Console.WriteLine($"Board size: {boardArray.GetLength(0)}x{boardArray.GetLength(1)}");
            BoardState = ConvertTo2DJaggedArray(boardArray);
            
            // Player numbers - need public access methods
            Player1Moves = ticTacToeGame.GetPlayer1Numbers().ToList();
            Player2Moves = ticTacToeGame.GetPlayer2Numbers().ToList();
            
            // Serialize move histories - need public access methods
            SerializeMoveHistory(ticTacToeGame.GetMoveHistory());
        }

        // RESTORE DATA TO GAME
        public override void RestoreToGame(AbstractGame game)
        {
            var ticTacToeGame = (TicTacToeGame)game;
            
            // Restore board
            ticTacToeGame.SetTicTacToeBoard(new TicTacToeBoard(BoardSize));
            ticTacToeGame.Board = ticTacToeGame.GetTicTacToeBoard();
            
            int[,] boardArray = ConvertToArray2D(BoardState);
            ticTacToeGame.Board.SetBoardState(boardArray);
            
            // Restore game properties - need public setter methods
            ticTacToeGame.SetGameMode(GameMode);
            ticTacToeGame.SetIsGameOver(IsGameOver);
            ticTacToeGame.SetTargetSum(TargetSum);
            
            // Restore players
            ticTacToeGame.RestorePlayersFromData(GameMode, Player1Name, Player2Name, Player1Moves, Player2Moves);
            
            // Set current player
            ticTacToeGame.SetCurrentPlayerByName(CurrentPlayerName);
            
            // Restore move histories
            ticTacToeGame.SetMoveHistory(DeserializeMoveHistory(ticTacToeGame.GetPlayer1(), ticTacToeGame.GetPlayer2()));
        }

        // GAME-SPECIFIC SERIALIZATION METHODS
        protected override int SerializeMoveData(object moveData)
        {
            return (int)moveData;
        }

        protected override object DeserializeMoveData(int serializedData)
        {
            return serializedData;
        }

        protected override int[][] SerializeBoardState(object boardState)
        {
            return ConvertTo2DJaggedArray((int[,])boardState);
        }

        protected override object DeserializeBoardState(int[][] serializedState)
        {
            return ConvertToArray2D(serializedState);
        }

        // CONVERSION UTILITIES
        public static int[][] ConvertTo2DJaggedArray(int[,] array2D)
        {
            int rows = array2D.GetLength(0);
            int cols = array2D.GetLength(1);
            int[][] jaggedArray = new int[rows][];

            for (int i = 0; i < rows; i++)
            {
                jaggedArray[i] = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    jaggedArray[i][j] = array2D[i, j];
                }
            }

            return jaggedArray;
        }

        public static int[,] ConvertToArray2D(int[][] jaggedArray)
        {
            int rows = jaggedArray.Length;
            int cols = jaggedArray[0].Length;
            int[,] array2D = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    array2D[i, j] = jaggedArray[i][j];
                }
            }

            return array2D;
        }
    }
}
namespace GameFrameWork
{
    public class GomokuGameData : GameData
    {
        public string[][] BoardState { get; set; }

        public GomokuGameData()
        {
            GameType = "Gomoku";
        }

        // POPULATE DATA FROM GAME
        public override void PopulateFromGame(AbstractGame game)
        {
            var gomokuGame = (GomokuGame)game;
            
            // Common data
            BoardSize = gomokuGame.Board.GetSize();
            GameMode = gomokuGame.GetGameMode();
            CurrentPlayerName = gomokuGame.GetCurrentPlayerName();
            Player1Name = gomokuGame.GetPlayer1Name();
            Player2Name = gomokuGame.GetPlayer2Name();
            IsGameOver = gomokuGame.GetIsGameOver();
            
            // Board state
            char[,] boardArray = (char[,])gomokuGame.Board.GetBoardState();
            BoardState = ConvertTo2DJaggedArray(boardArray);
            
            // Serialize move histories
            SerializeMoveHistory(gomokuGame.GetMoveHistory());
            SerializeRedoHistory(gomokuGame.GetRedoHistory());
        }

        // RESTORE DATA TO GAME
        public override void RestoreToGame(AbstractGame game)
        {
            var gomokuGame = (GomokuGame)game;
            
            // Restore board
            gomokuGame.SetGomokuBoard(new GomokuBoard(BoardSize, 1));
            gomokuGame.Board = gomokuGame.GetGomokuBoard();
            
            char[,] boardArray = ConvertToArray2D(BoardState);
            gomokuGame.Board.SetBoardState(boardArray);
            
            // Restore game properties
            gomokuGame.SetGameMode(GameMode);
            gomokuGame.SetIsGameOver(IsGameOver);
            
            // Restore players
            gomokuGame.RestorePlayersFromData(GameMode, Player1Name, Player2Name);
            
            // Set current player
            gomokuGame.SetCurrentPlayerByName(CurrentPlayerName);
            
            // Restore move histories
            gomokuGame.SetMoveHistory(DeserializeMoveHistory(gomokuGame.GetPlayer1(), gomokuGame.GetPlayer2()));
            gomokuGame.SetRedoHistory(DeserializeRedoHistory(gomokuGame.GetPlayer1(), gomokuGame.GetPlayer2()));
        }

        // GAME-SPECIFIC SERIALIZATION METHODS
        protected override int SerializeMoveData(object moveData)
        {
            // Convert char to int: 'X' = 1, 'O' = 2, '.' = 0
            char symbol = (char)moveData;
            return symbol switch
            {
                'X' => 1,
                'O' => 2,
                '.' => 0,
                _ => 0
            };
        }

        protected override object DeserializeMoveData(int serializedData)
        {
            // Convert int back to char
            return serializedData switch
            {
                1 => 'X',
                2 => 'O',
                0 => '.',
                _ => '.'
            };
        }

        protected override int[][] SerializeBoardState(object boardState)
        {
            return ConvertTo2DJaggedArrayInt((char[,])boardState);
        }

        protected override object DeserializeBoardState(int[][] serializedState)
        {
            return ConvertToArray2DChar(serializedState);
        }

        // CONVERSION UTILITIES
        public static string[][] ConvertTo2DJaggedArray(char[,] array2D)
        {
            int rows = array2D.GetLength(0);
            int cols = array2D.GetLength(1);
            string[][] jaggedArray = new string[rows][];

            for (int i = 0; i < rows; i++)
            {
                jaggedArray[i] = new string[cols];
                for (int j = 0; j < cols; j++)
                {
                    jaggedArray[i][j] = array2D[i, j].ToString();
                }
            }

            return jaggedArray;
        }

        public static char[,] ConvertToArray2D(string[][] jaggedArray)
        {
            int rows = jaggedArray.Length;
            int cols = jaggedArray[0].Length;
            char[,] array2D = new char[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    array2D[i, j] = string.IsNullOrEmpty(jaggedArray[i][j]) ? '.' : jaggedArray[i][j][0];
                }
            }

            return array2D;
        }

        // Additional conversion methods for int serialization
        private static int[][] ConvertTo2DJaggedArrayInt(char[,] array2D)
        {
            int rows = array2D.GetLength(0);
            int cols = array2D.GetLength(1);
            int[][] jaggedArray = new int[rows][];

            for (int i = 0; i < rows; i++)
            {
                jaggedArray[i] = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    char symbol = array2D[i, j];
                    jaggedArray[i][j] = symbol switch
                    {
                        'X' => 1,
                        'O' => 2,
                        '.' => 0,
                        _ => 0
                    };
                }
            }

            return jaggedArray;
        }

        private static char[,] ConvertToArray2DChar(int[][] jaggedArray)
        {
            int rows = jaggedArray.Length;
            int cols = jaggedArray[0].Length;
            char[,] array2D = new char[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    array2D[i, j] = jaggedArray[i][j] switch
                    {
                        1 => 'X',
                        2 => 'O',
                        0 => '.',
                        _ => '.'
                    };
                }
            }

            return array2D;
        }
    }
}
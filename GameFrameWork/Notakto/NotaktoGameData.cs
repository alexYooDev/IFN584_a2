namespace GameFrameWork
{
    public class NotaktoGameData : GameData
    {
        public List<int> DeadBoards { get; set; }
        public List<string[][]> Boards { get; set; }

        public NotaktoGameData()
        {
            GameType = "Notakto";
            DeadBoards = new List<int>();
            Boards = new List<string[][]>();
        }

        // POPULATE DATA FROM GAME
        public override void PopulateFromGame(AbstractGame game)
        {
            var notaktoGame = (NotaktoGame)game;
            
            // Common data
            BoardSize = 3; // Notakto always uses 3x3 boards
            BoardCount = 3; // Notakto always uses 3 boards
            GameMode = notaktoGame.GetGameMode();
            CurrentPlayerName = notaktoGame.GetCurrentPlayerName();
            Player1Name = notaktoGame.GetPlayer1Name();
            Player2Name = notaktoGame.GetPlayer2Name();
            IsGameOver = notaktoGame.GetIsGameOver();
            
            // Notakto-specific data
            var boardState = (Tuple<List<char[,]>, List<int>>)notaktoGame.Board.GetBoardState();
            DeadBoards = new List<int>(boardState.Item2);
            
            // Convert boards to serializable format
            Boards = new List<string[][]>();
            foreach (var board in boardState.Item1)
            {
                Boards.Add(ConvertCharArrayToJagged(board));
            }
            
            // Serialize move histories
            SerializeMoveHistory(notaktoGame.GetMoveHistory());
        }

        // RESTORE DATA TO GAME
        public override void RestoreToGame(AbstractGame game)
        {
            var notaktoGame = (NotaktoGame)game;
            
            // Restore board
            notaktoGame.SetNotaktoBoard(new NotaktoBoard());
            notaktoGame.Board = notaktoGame.GetNotaktoBoard();
            
            // Restore board state
            List<char[,]> boards = new List<char[,]>();
            foreach (var board in Boards)
            {
                boards.Add(ConvertJaggedToCharArray(board));
            }
            
            var boardStateToRestore = Tuple.Create(boards, DeadBoards);
            notaktoGame.Board.SetBoardState(boardStateToRestore);
            
            // Restore game properties
            notaktoGame.SetGameMode(GameMode);
            notaktoGame.SetIsGameOver(IsGameOver);
            
            // Restore players
            notaktoGame.RestorePlayersFromData(GameMode, Player1Name, Player2Name);
            
            // Set current player
            notaktoGame.SetCurrentPlayerByName(CurrentPlayerName);
            
            // Restore move histories
            notaktoGame.SetMoveHistory(DeserializeMoveHistory(notaktoGame.GetPlayer1(), notaktoGame.GetPlayer2()));
        }

        // GAME-SPECIFIC SERIALIZATION METHODS
        protected override int SerializeMoveData(object moveData)
        {
            // For Notakto, moveData is always 'X' (char), convert to int
            return (char)moveData == 'X' ? 1 : 0;
        }

        protected override object DeserializeMoveData(int serializedData)
        {
            // Convert back to char
            return serializedData == 1 ? 'X' : ' ';
        }

        protected override int[][] SerializeBoardState(object boardState)
        {
            // Notakto board state is complex (multiple boards + dead boards)
            // For now, return empty array - board state is handled separately
            return new int[0][];
        }

        protected override object DeserializeBoardState(int[][] serializedState)
        {
            // Board state is handled separately in Notakto
            return null;
        }

        // CONVERSION UTILITIES
        public static string[][] ConvertCharArrayToJagged(char[,] board)
        {
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);
            string[][] jaggedArray = new string[rows][];
            
            for (int i = 0; i < rows; i++)
            {
                jaggedArray[i] = new string[cols];
                for (int j = 0; j < cols; j++)
                {
                    jaggedArray[i][j] = board[i, j].ToString();
                }
            }
            return jaggedArray;
        }

        public static char[,] ConvertJaggedToCharArray(string[][] jagged)
        {
            int rows = jagged.Length;
            int cols = jagged[0].Length;
            char[,] board = new char[rows, cols];
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    board[i, j] = string.IsNullOrEmpty(jagged[i][j]) ? ' ' : jagged[i][j][0];
                }
            }
            return board;
        }
    }
}
using System.Text.Json;

namespace GameFrameWork
{
    public class NotaktoGameData : GameData
    {
        public List<int> DeadBoards { get; set; }

        public NotaktoGameData()
        {
            GameType = "Notakto";
            BoardState = new object[0][];
            DeadBoards = new List<int>();
        }

        public override void PopulateFromGame(AbstractGame game)
        {
            if (!(game is NotaktoGame notaktoGame)) return;

            // Common data
            BoardSize = 3; // Notakto always uses 3x3 boards
            BoardCount = 3; // Notakto always uses 3 boards
            GameMode = notaktoGame.GetGameMode();
            CurrentPlayerName = notaktoGame.GetCurrentPlayerName();
            Player1Name = notaktoGame.GetPlayer1Name();
            Player2Name = notaktoGame.GetPlayer2Name();
            IsGameOver = notaktoGame.GetIsGameOver();
            GameType = "Notakto";

            // Notakto-specific board state
            var boardState = (Tuple<List<char[,]>, List<int>>)notaktoGame.Board.GetBoardState();
            DeadBoards = new List<int>(boardState.Item2);

            // Convert multiple boards to flattened object array
            BoardState = ConvertNotaktoBoardsToObjectArray(boardState.Item1);

            // Serialize move histories
            SerializeMoveHistory(notaktoGame.GetMoveHistory());
        }

        public override void RestoreToGame(AbstractGame game)
        {
            if (!(game is NotaktoGame notaktoGame)) return;

            // Restore board
            notaktoGame.SetNotaktoBoard(new NotaktoBoard());
            notaktoGame.Board = notaktoGame.GetNotaktoBoard();

            // Restore complex board state
            var restoredBoards = ConvertObjectArrayToNotaktoBoards(BoardState);
            var boardStateToRestore = Tuple.Create(restoredBoards, DeadBoards);
            notaktoGame.Board.SetBoardState(boardStateToRestore);

            // Restore game properties
            notaktoGame.SetGameMode(GameMode);
            notaktoGame.SetIsGameOver(IsGameOver);

            // Restore players
            notaktoGame.RestorePlayersFromData(GameMode, Player1Name, Player2Name);

            // Set current player and move history
            notaktoGame.SetCurrentPlayerByName(CurrentPlayerName);
            notaktoGame.SetMoveHistory(DeserializeMoveHistory(notaktoGame.GetPlayer1(), notaktoGame.GetPlayer2()));
            notaktoGame.SetRedoHistory(new Stack<Move>());
        }

        // Notakto-specific conversion methods
        private object[][] ConvertNotaktoBoardsToObjectArray(List<char[,]> boards)
        {
            if (boards == null || boards.Count == 0) return new object[0][];

            // Flatten all boards: 9 rows total (3 boards × 3 rows each)
            int totalRows = boards.Count * 3;
            object[][] result = new object[totalRows][];

            for (int boardIndex = 0; boardIndex < boards.Count; boardIndex++)
            {
                var board = boards[boardIndex];
                for (int row = 0; row < 3; row++)
                {
                    int targetRow = boardIndex * 3 + row;
                    result[targetRow] = new object[3];
                    for (int col = 0; col < 3; col++)
                    {
                        result[targetRow][col] = board[row, col].ToString();
                    }
                }
            }

            return result;
        }

        private List<char[,]> ConvertObjectArrayToNotaktoBoards(object[][] objectArray)
        {
            var result = new List<char[,]>();

            if (objectArray == null || objectArray.Length == 0)
            {
                // Return 3 empty boards
                for (int i = 0; i < 3; i++)
                {
                    result.Add(new char[3, 3]);
                }
                return result;
            }

            // Extract 3 boards from flattened format
            for (int boardIndex = 0; boardIndex < 3; boardIndex++)
            {
                char[,] board = new char[3, 3];
                for (int row = 0; row < 3; row++)
                {
                    int sourceRow = boardIndex * 3 + row;
                    if (sourceRow < objectArray.Length)
                    {
                        for (int col = 0; col < 3; col++)
                        {
                            string value;
                            if (objectArray[sourceRow][col] is JsonElement jsonElement)
                                value = jsonElement.GetString() ?? "";
                            else
                                value = objectArray[sourceRow][col]?.ToString() ?? "";

                            board[row, col] = string.IsNullOrEmpty(value) ? ' ' : value[0];
                        }
                    }
                }
                result.Add(board);
            }

            return result;
        }

        // Serialization methods
        protected override int SerializeMoveData(object moveData)
        {
            return (moveData is char symbol && symbol == 'X') ? 1 : 0;
        }

        protected override object DeserializeMoveData(int serializedData)
        {
            return serializedData == 1 ? 'X' : ' ';
        }

        protected override int[][] SerializeBoardState(object boardState)
        {
            try
            {
                if (boardState is Tuple<List<char[,]>, List<int>> notaktoState)
                {
                    // Rows 0-2: boards data (flattened)
                    // Row 3: dead board indicators
                    int[][] result = new int[4][];
                    
                    // Flatten all 3 boards into 3 rows of 9 columns each
                    for (int boardIndex = 0; boardIndex < 3; boardIndex++)
                    {
                        result[boardIndex] = new int[9]; // 3x3 = 9 cells per board
                        
                        if (notaktoState.Item1 != null && boardIndex < notaktoState.Item1.Count)
                        {
                            var board = notaktoState.Item1[boardIndex];
                            for (int i = 0; i < 3; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    int flatIndex = i * 3 + j;
                                    result[boardIndex][flatIndex] = board[i, j] == 'X' ? 1 : 0;
                                }
                            }
                        }
                    }
                    
                    // Store dead boards in row 3
                    result[3] = new int[3];
                    if (notaktoState.Item2 != null)
                    {
                        for (int i = 0; i < Math.Min(notaktoState.Item2.Count, 3); i++)
                        {
                            if (notaktoState.Item2[i] >= 0 && notaktoState.Item2[i] < 3)
                            {
                                result[3][notaktoState.Item2[i]] = 1; // Mark as dead
                            }
                        }
                    }
                    
                    return result;
                }
                
                // Fallback: return empty 4x3 array
                return new int[4][] { new int[9], new int[9], new int[9], new int[3] };
            }
            catch
            {
                return new int[4][] { new int[9], new int[9], new int[9], new int[3] };
            }
        }

        protected override object DeserializeBoardState(int[][] serializedState)
        {
            try
            {
                if (serializedState == null || serializedState.Length < 4)
                {
                    // Return empty state
                    return CreateEmptyNotaktoState();
                }
                
                // Reconstruct the 3 boards
                var boards = new List<char[,]>();
                for (int boardIndex = 0; boardIndex < 3; boardIndex++)
                {
                    char[,] board = new char[3, 3];
                    
                    if (serializedState[boardIndex] != null && serializedState[boardIndex].Length >= 9)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                int flatIndex = i * 3 + j;
                                board[i, j] = serializedState[boardIndex][flatIndex] == 1 ? 'X' : ' ';
                            }
                        }
                    }
                    boards.Add(board);
                }
                
                // Reconstruct dead boards
                var deadBoards = new List<int>();
                if (serializedState[3] != null)
                {
                    for (int i = 0; i < Math.Min(serializedState[3].Length, 3); i++)
                    {
                        if (serializedState[3][i] == 1)
                        {
                            deadBoards.Add(i);
                        }
                    }
                }
                
                return Tuple.Create(boards, deadBoards);
            }
            catch
            {
                return CreateEmptyNotaktoState();
            }
        }

        private object CreateEmptyNotaktoState()
        {
            var boards = new List<char[,]>();
            for (int i = 0; i < 3; i++)
            {
                char[,] board = new char[3, 3];
                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        board[row, col] = ' ';
                    }
                }
                boards.Add(board);
            }
            return Tuple.Create(boards, new List<int>());
        }

    }
}
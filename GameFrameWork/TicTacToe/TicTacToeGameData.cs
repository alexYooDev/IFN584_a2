using System.Text.Json;

namespace GameFrameWork
{
    public class TicTacToeGameData : GameData
    {
        public int TargetSum { get; set; }


        public TicTacToeGameData()
        {
            GameType = "NumericalTicTacToe";
            Player1Moves = new List<object>();
            Player2Moves = new List<object>();
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
            GameType = "NumericalTicTacToe";
            
            // TicTacToe-specific data
            TargetSum = ticTacToeGame.GetTargetSum();

            // Board state
            int[,] boardArray = (int[,])ticTacToeGame.Board.GetBoardState();
            BoardState = ExtractBoardState(ticTacToeGame.Board);
            
            // Player numbers - need public access methods
            Player1Moves = ConvertToObjectList(ticTacToeGame.GetPlayer1Numbers());
            Player2Moves = ConvertToObjectList(ticTacToeGame.GetPlayer2Numbers());
            
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
            
            RestoreBoardState(ticTacToeGame.Board);
            
            // Restore game properties - need public setter methods
            ticTacToeGame.SetGameMode(GameMode);
            ticTacToeGame.SetIsGameOver(IsGameOver);
            ticTacToeGame.SetTargetSum(TargetSum);

            // Restore players
            ticTacToeGame.RestorePlayersFromData(GameMode, Player1Name, Player2Name, ConvertToIntList(Player1Moves), ConvertToIntList(Player2Moves));
            
            // Set current player
            ticTacToeGame.SetCurrentPlayerByName(CurrentPlayerName);
            
            // Restore move histories
            ticTacToeGame.SetMoveHistory(DeserializeMoveHistory(ticTacToeGame.GetPlayer1(), ticTacToeGame.GetPlayer2()));
        }
        
        private static object[][] ExtractBoardState(AbstractBoard board)
        {
            if (board is TicTacToeBoard tttBoard)
            {
                // Get the internal board state
                var internalBoard = tttBoard.GetBoardState() as int[,];
                
                if (internalBoard == null)
                    return new object[0][];

                int size = board.GetSize();
                object[][] serializedBoard = new object[size][];
                
                for (int i = 0; i < size; i++)
                {
                    serializedBoard[i] = new object[size];
                    for (int j = 0; j < size; j++)
                    {
                        serializedBoard[i][j] = internalBoard[i, j];
                    }
                }
                
                return serializedBoard;
            }
            
            return new object[0][];
        }

        private void RestoreBoardState(AbstractBoard board)
        {
            if (board is TicTacToeBoard tttBoard && BoardState != null)
            {
                int size = BoardState.Length;
                int[,] restoredBoard = new int[size, size];
                
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (BoardState[i][j] is JsonElement jsonElement)
                        {
                            restoredBoard[i, j] = jsonElement.GetInt32();
                        }
                        else
                        {
                            restoredBoard[i, j] = Convert.ToInt32(BoardState[i][j]);
                        }
                    }
                }
                
                tttBoard.SetBoardState(restoredBoard);
            }
        }

        private static List<object> ConvertToObjectList(HashSet<int> numbers)
        {
            return numbers.Cast<object>().ToList();
        }

        private static List<int> ConvertToIntList(List<object> objects)
        {
            var result = new List<int>();
            
            foreach (var obj in objects)
            {
                if (obj is JsonElement jsonElement)
                {
                    result.Add(jsonElement.GetInt32());
                }
                else
                {
                    result.Add(Convert.ToInt32(obj));
                }
            }
            
            return result;
        }
        // GAME-SPECIFIC DATA SERIALIZATION METHODS

        protected override int SerializeMoveData(object moveData)
        {
            return moveData is int number ? number : 0;
        }

        protected override object DeserializeMoveData(int serializedData)
        {
            return serializedData;
        }

        protected override int[][] SerializeBoardState(object boardState)
        {
            if (boardState is int[,] board)
            {
                int rows = board.GetLength(0);
                int cols = board.GetLength(1);
                int[][] serialized = new int[rows][];
                
                for (int i = 0; i < rows; i++)
                {
                    serialized[i] = new int[cols];
                    for (int j = 0; j < cols; j++)
                    {
                        serialized[i][j] = board[i, j];
                    }
                }
                
                return serialized;
            }
            
            return new int[0][];
        }

        protected override object DeserializeBoardState(int[][] serializedState)
        {
            if (serializedState == null || serializedState.Length == 0)
                return new int[0, 0];

            int rows = serializedState.Length;
            int cols = serializedState[0].Length;
            int[,] board = new int[rows, cols];
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    board[i, j] = serializedState[i][j];
                }
            }
            
            return board;
        }
    }
}
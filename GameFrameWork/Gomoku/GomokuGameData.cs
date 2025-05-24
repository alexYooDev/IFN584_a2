using System.Text.Json;

namespace GameFrameWork
{
    public class GomokuGameData : GameData
    {
        public GomokuGameData()
        {
            GameType = "Gomoku";
            BoardState = new object[0][];
        }

        public override void PopulateFromGame(AbstractGame game)
        {
            if (!(game is GomokuGame gomokuGame)) return;
            
            // Common data
            BoardSize = gomokuGame.Board.GetSize();
            BoardCount = gomokuGame.Board.GetBoardCount();
            GameMode = gomokuGame.GetGameMode();
            CurrentPlayerName = gomokuGame.GetCurrentPlayerName();
            Player1Name = gomokuGame.GetPlayer1Name();
            Player2Name = gomokuGame.GetPlayer2Name();
            IsGameOver = gomokuGame.GetIsGameOver();
            GameType = "Gomoku";

            // Board state
            char[,] boardArray = (char[,])gomokuGame.Board.GetBoardState();
            BoardState = ConvertCharBoardToObjectArray(boardArray);
            
            // Serialize move histories
            SerializeMoveHistory(gomokuGame.GetMoveHistory());
        }

        public override void RestoreToGame(AbstractGame game)
        {
            if (!(game is GomokuGame gomokuGame)) return;
            
            // Restore board
            gomokuGame.SetGomokuBoard(new GomokuBoard(BoardSize, BoardCount));
            gomokuGame.Board = gomokuGame.GetGomokuBoard();
            
            // Restore board state
            char[,] boardArray = ConvertObjectArrayToCharBoard(BoardState);
            gomokuGame.Board.SetBoardState(boardArray);
            
            // Restore game properties
            gomokuGame.SetGameMode(GameMode);
            gomokuGame.SetIsGameOver(IsGameOver);

            // Restore players
            gomokuGame.RestorePlayersFromData(GameMode, Player1Name, Player2Name);
            
            // Set current player and move history
            gomokuGame.SetCurrentPlayerByName(CurrentPlayerName);
            gomokuGame.SetMoveHistory(DeserializeMoveHistory(gomokuGame.GetPlayer1(), gomokuGame.GetPlayer2()));
            gomokuGame.SetRedoHistory(new Stack<Move>());
        }

        // Gomoku-specific conversion methods
        private object[][] ConvertCharBoardToObjectArray(char[,] board)
        {
            if (board == null) return new object[0][];
            
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);
            object[][] result = new object[rows][];
            
            for (int i = 0; i < rows; i++)
            {
                result[i] = new object[cols];
                for (int j = 0; j < cols; j++)
                {
                    result[i][j] = board[i, j].ToString();
                }
            }
            
            return result;
        }

        private char[,] ConvertObjectArrayToCharBoard(object[][] objectArray)
        {
            if (objectArray == null || objectArray.Length == 0) 
                return new char[0, 0];
            
            int rows = objectArray.Length;
            int cols = objectArray[0].Length;
            char[,] result = new char[rows, cols];
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    string value;
                    if (objectArray[i][j] is JsonElement jsonElement)
                        value = jsonElement.GetString() ?? "";
                    else
                        value = objectArray[i][j]?.ToString() ?? "";
                    
                    result[i, j] = string.IsNullOrEmpty(value) ? '.' : value[0];
                }
            }
            
            return result;
        }

        // Serialization methods
        protected override int SerializeMoveData(object moveData)
        {
            if (moveData is char symbol)
            {
                return symbol switch
                {
                    'X' => 1,
                    'O' => 2,
                    '.' => 0,
                    ' ' => 0,
                    _ => 0
                };
            }
            return 0;
        }

        protected override object DeserializeMoveData(int serializedData)
        {
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
            if (boardState is char[,] board)
            {
                int rows = board.GetLength(0);
                int cols = board.GetLength(1);
                int[][] result = new int[rows][];
                
                for (int i = 0; i < rows; i++)
                {
                    result[i] = new int[cols];
                    for (int j = 0; j < cols; j++)
                    {
                        char symbol = board[i, j];
                        result[i][j] = symbol switch
                        {
                            'X' => 1,
                            'O' => 2,
                            '.' => 0,
                            _ => 0
                        };
                    }
                }
                
                return result;
            }
            
            return new int[0][];
        }

        protected override object DeserializeBoardState(int[][] serializedState)
        {
            if (serializedState == null || serializedState.Length == 0)
                return new char[0, 0];

            int rows = serializedState.Length;
            int cols = serializedState[0].Length;
            char[,] board = new char[rows, cols];
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    board[i, j] = serializedState[i][j] switch
                    {
                        1 => 'X',
                        2 => 'O',
                        0 => '.',
                        _ => '.'
                    };
                }
            }
            
            return board;
        }
    }
}
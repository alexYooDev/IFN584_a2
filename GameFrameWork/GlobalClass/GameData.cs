namespace GameFrameWork
{
    using System.Collections.Generic;
    using System.Text.Json;
    
    public abstract class GameData: IGameData
    {
        public int BoardSize { get; set; }
        public int BoardCount { get; set; } = 1;
        public string GameMode { get; set; }
        public string CurrentPlayerName { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public List<object> Player1Moves { get; set; }
        public List<object> Player2Moves { get; set; }
        public string GameType { get; set; }
        public bool IsGameOver { get; set; }
        public object[][] BoardState { get; set; }
        public List<MovesToSerialize> MoveHistory { get; set; }

        protected GameData()
        {
            Player1Moves = new List<object>();
            Player2Moves = new List<object>();
            MoveHistory = new List<MovesToSerialize>();
            BoardState = new object[0][];
        }

        public virtual void SaveToFile(string filename)
        {
            try
            {
                string saveDirectory = Path.Combine(Directory.GetCurrentDirectory(), "saveData");
                if (!Directory.Exists(saveDirectory))
                {
                    Directory.CreateDirectory(saveDirectory);
                }

                string jsonString = JsonSerializer.Serialize(this, GetJsonOptions());
                string saveFilePath = Path.Combine(saveDirectory, filename + ".json");
                File.WriteAllText(saveFilePath, jsonString);
                
                OnSaveSuccess(filename);
            }
            catch (Exception e)
            {
                OnSaveError(e);
            }
        }

        public static Type LoadFromFile<Type>(string filename) where Type : GameData
        {
            try
            {
                string saveDirectory = Path.Combine(Directory.GetCurrentDirectory(), "saveData");
                string saveFilePath = Path.Combine(saveDirectory, filename + ".json");

                if (!File.Exists(saveFilePath))
                {
                    OnFileNotFound();
                    return null;
                }

                string jsonString = File.ReadAllText(saveFilePath);
                var gameData = JsonSerializer.Deserialize<Type>(jsonString, GetJsonOptions());
                
                if (gameData != null)
                {
                    OnLoadSuccess(filename);
                }
                
                return gameData;
            }
            catch (Exception e)
            {
                OnLoadError(e);
                return null;
            }
        }

        public abstract void PopulateFromGame(AbstractGame game);

        public abstract void RestoreToGame(AbstractGame game);
        
        public virtual void SerializeMoveHistory(Stack<Move> moveHistory)
        {
            MoveHistory.Clear();
            
            foreach (Move move in moveHistory)
            {
                var serializedMove = new MovesToSerialize
                {
                    BoardIndex = move.BoardIndex,
                    Row = move.Row,
                    Col = move.Col,
                    PlayerName = move.Player.Name,
                    MoveData = SerializeMoveData(move.MoveData),
                    PreviousBoardState = SerializeBoardState(move.PreviousBoardState)
                };
                MoveHistory.Add(serializedMove);
            }
        }

        public virtual Stack<Move> DeserializeMoveHistory(AbstractPlayer player1, AbstractPlayer player2)
        {
            var moveStack = new Stack<Move>();
            
            if (MoveHistory != null)
            {
                // Restore in reverse order to maintain stack order
                for (int i = MoveHistory.Count - 1; i >= 0; i--)
                {
                    var serializedMove = MoveHistory[i];
                    AbstractPlayer player = serializedMove.PlayerName == player1.Name ? player1 : player2;
                    
                    Move move = new Move(
                        serializedMove.BoardIndex,
                        serializedMove.Row,
                        serializedMove.Col,
                        player,
                        DeserializeMoveData(serializedMove.MoveData),
                        DeserializeBoardState(serializedMove.PreviousBoardState)
                    );
                    
                    moveStack.Push(move);
                }
            }
            
            return moveStack;
        }

        protected abstract int SerializeMoveData(object moveData);
        protected abstract object DeserializeMoveData(int serializedData);
        protected abstract int[][] SerializeBoardState(object boardState);
        protected abstract object DeserializeBoardState(int[][] serializedState);

        protected static JsonSerializerOptions GetJsonOptions()
        {
            return new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        protected virtual void OnSaveSuccess(string filename)
        {
            Console.WriteLine($"\nGame saved successfully as {filename}");
        }

        protected virtual void OnSaveError(Exception e)
        {
            Console.WriteLine($"\nError saving game: {e.Message}");
        }

        protected static void OnLoadSuccess(string filename)
        {
            Console.WriteLine($"\nGame loaded successfully from {filename}");
        }

        protected static void OnLoadError(Exception e)
        {
            Console.WriteLine($"\nError loading game: {e.Message}");
        }

        protected static void OnFileNotFound()
        {
            Console.WriteLine("\nSave file not found. Please check the filename and try again.");
        }
    }
}
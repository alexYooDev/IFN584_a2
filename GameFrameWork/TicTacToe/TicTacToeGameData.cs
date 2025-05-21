namespace GameFrameWork
{
    public class TicTacToeGameData
    {
        public int BoardSize { get; set; }
        public string GameMode { get; set; }
        public string CurrentPlayerName { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public List<int> Player1Numbers { get; set; }
        public List<int> Player2Numbers { get; set; }
        public string GameType { get; set; }
        public bool IsGameOver { get; set; }
        public int TargetSum { get; set; }
        public int[][] BoardState { get; set; }

        // Track undone moves
        public int? UndoneMovesCount { get; set; }

        public List<MovesToSerialize> MoveHistory { get; set; }
        public List<MovesToSerialize> RedoHistory { get; set; }


        // Converting to jagged array to support json serialization
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
        
        // Converting JSON serialized jagged array back to 2D array to insert back in board / boardState
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

    // Helping class for formating data to be serialized
    public class MovesToSerialize
    {
        public int BoardIndex { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public string PlayerName { get; set; }
        public int MoveData { get; set; }
        public int[][] PreviousBoardState { get; set; }
    }
}
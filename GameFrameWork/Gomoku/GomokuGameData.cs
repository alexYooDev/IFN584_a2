namespace GameFrameWork
{
    public class GomokuGameData
    {
        public int BoardSize { get; set; }
        public string GameMode { get; set; }
        public string CurrentPlayerName { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public string GameType { get; set; }
        public bool IsGameOver { get; set; }
        public string[][] BoardState { get; set; }
        public List<GomokuMovesToSerialize> MoveHistory { get; set; }
        public List<GomokuMovesToSerialize> RedoHistory { get; set; }

        // Converting to jagged array to support json serialization
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

        // Converting JSON serialized jagged array back to 2D array to insert back in board / boardState
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
    }
}
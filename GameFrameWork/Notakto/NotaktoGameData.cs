namespace GameFrameWork
{
    public class NotaktoGameData
    {
        public int BoardSize { get; set; }
        public int BoardCount { get; set; }
        public string GameMode { get; set; }
        public string CurrentPlayerName { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public bool IsGameOver { get; set; }
        public string GameType { get; set; }

        public List<int> DeadBoards { get; set; }
        public List<string[][]> Boards { get; set; }

        // Track undone moves
        public int? UndoneMovesCount { get; set; }

        public List<MovesToSerialize> MoveHistory { get; set; }
        public List<MovesToSerialize> RedoHistory { get; set; }

        // Convert a char[,] to a string[][] to support json serialization
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

        // Convert JSON serialized jagged array back
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
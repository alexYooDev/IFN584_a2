namespace GameFrameWork
{
    public abstract class AbstractBoard
    {
        protected int Size { get; set; }

        // Default number of board = 1, NoTakTo uses 3
        protected int BoardCount { get; set; } = 1;
        protected List<object> BoardsState { get; set; }

        /* Constructor */
        public AbstractBoard(int size, int boardCount = 1)
        {
            Size = size;
            BoardCount = boardCount;
            InitializeBoards();
        }

        // Initialize boards based on board count
        protected abstract void InitializeBoards();

        public abstract void DisplayBoard();
        public abstract bool IsValidMove(int boardIndex, int row, int col, object moveData);

        /* 
            boardIndex: the number of board, added in Notakto for board navigation, set as 0 by default
            row : row number
            col : column number
            moveDate : to store / save information of the board every time player makes move
         */
        public abstract void MakeMove(int boardIndex = 0, int row, int col, object moveData = null);

        /* boardIndex: the number of board, added in Notakto for board navigation, set as 0 by default */
        public abstract bool IsBoardFull(int boardIndex = 0);

        /* For Notakto to determine if all 3 boards are full */
        public abstract bool AreAllBoardsFull();
        public abstract object GetBoardState(); // Needed for saving/loading games
        public abstract void SetBoardState(object state); // Needed for restoring games

        /* Getter for board size */
        public int GetSize()
        {
            return Size;
        }
        
        /* Getter for the number of board */
        public int GetBoardCount()
        {
            return BoardCount;
        }
    }
}
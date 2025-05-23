public abstract class AbstractBoard
{
    protected int Size { get; set; }

    // Support for multiple boards (especially for Notakto which uses 3 boards)
    protected int BoardCount { get; set; } = 1;
    protected int ActiveBoardIndex { get; set; } = 0;

    // Allow different board representations for different games
    // For TicTacToe: int[,] for numbers
    // For Notakto/Gomoku: char[,] or enum[,] for X pieces
    protected List<object> BoardsState { get; set; }

    public AbstractBoard(int size, int boardCount = 1)
    {
        Size = size;
        BoardCount = boardCount;
    }

    // // Initialize appropriate board structure for each game (Notakto)
    // protected abstract void InitializeBoards();

    // Methods every board game needs
    public abstract void DisplayBoard(int boardIndex = 0);
    public abstract bool IsValidMove(int row, int col, object moveData, int boardIndex = 0, bool displayMessages = true);
    public abstract void MakeMove(int row, int col, object moveData = null, int boardIndex = 0);

    // Board status checks
    public abstract bool IsBoardFull(int boardIndex = 0);

    public abstract bool AreAllBoardsDead(); // For Notakto

    // For the game state operations
    public abstract object GetBoardState();
    public abstract void SetBoardState(object state);

    // Allow switching between multiple boards (for Notakto)
    // public virtual void SwitchToBoard(int boardIndex)
    // {
    //     if (boardIndex >= 0 && boardIndex < BoardCount)
    //         ActiveBoardIndex = boardIndex;
    // }

    // Common method for getting the board size
    public int GetSize()
    {
        return Size;
    }

    // // Get active board index (Notakto)
    // public int GetActiveBoardIndex()
    // { 
    //     return ActiveBoardIndex;
    // } 

    // Get total number of boards (NoTakto)
    // public int GetBoardCount()
    // {
    //     return BoardCount;
    // }
}
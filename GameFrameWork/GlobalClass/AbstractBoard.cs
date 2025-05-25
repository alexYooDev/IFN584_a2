public abstract class AbstractBoard
{
    protected int Size { get; set; }
    
    // Support for multiple boards (especially for Notakto which uses 3 boards)
    protected int BoardCount { get; set; } = 1;
    protected int ActiveBoardIndex { get; set; } = 0;
    
    // Allow different board representations for different games
    // For TicTacToe: int[,] for numbers
    // For Notakto/Gomoku: char[,] or enum[,] for X/O pieces
    protected List<object> BoardsState { get; set; }

    public AbstractBoard(int size, int boardCount = 1)
    {
        Size = size;
        BoardCount = boardCount;
    }
    
    // Methods every board game needs
    public abstract void DisplayBoard(int boardIndex = 0);
    public abstract bool IsValidMove(int row, int col, object moveData, int boardIndex = 0, bool displayMessages = true);
    public abstract void MakeMove(int row, int col, object moveData = null, int boardIndex = 0);
    
    // Board status checks
    public abstract bool IsBoardFull(int boardIndex = 0);
    public abstract object GetBoardState(); 
    public abstract void SetBoardState(object state);

    public int GetSize()
    {
        return Size;
    }
    public int GetBoardCount()
    {
        return BoardCount;
    }
}
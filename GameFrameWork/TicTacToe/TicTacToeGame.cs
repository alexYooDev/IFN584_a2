using System.Data.SqlTypes;
using System.Diagnostics;
using System.Text.Json;
namespace GameFrameWork
{
    public class TicTacToeGame : AbstractGame
    {
        private int TargetSum;
        private HashSet<int> OddNumbers;
        private HashSet<int> EvenNumbers;
        private TicTacToeBoard TicTacToeBoard;
        // Temporary storage for move before confirmation
        private int UndoneMovesCount = 0; // Track number of undone moves

        public TicTacToeGame(IGameRenderer renderer, IInputHandler inputHandler, IGameDataPersistence dataPersistence) : base(renderer , inputHandler, dataPersistence)
        {
            OddNumbers = new HashSet<int>();
            EvenNumbers = new HashSet<int>();
        }

        public TicTacToeGame():this (new ConsoleGameRenderer(), new ConsoleInputHandler(), new JsonGameDataPersistence())
        {}
        
            // Getters for data access
        public string GetGameMode() => GameMode;
        public string GetCurrentPlayerName() => CurrentPlayer.Name;
        public string GetPlayer1Name() => Player1.Name;
        public string GetPlayer2Name() => Player2.Name;
        public bool GetIsGameOver() => IsGameOver;
        public int GetTargetSum() => TargetSum;
        public TicTacToeBoard GetTicTacToeBoard() => TicTacToeBoard;
        public AbstractPlayer GetPlayer1() => Player1;
        public AbstractPlayer GetPlayer2() => Player2;
        public Stack<Move> GetMoveHistory() => MoveHistory;
        public Stack<Move> GetRedoHistory() => RedoHistory;

        public HashSet<int> GetPlayer1Numbers()
        {
            return Player1 is TicTacToeHumanPlayer humanPlayer
                ? humanPlayer.GetAvailableNumbers()
                : new HashSet<int>();
        }

        public HashSet<int> GetPlayer2Numbers()
        {
            return Player2 is TicTacToeHumanPlayer humanPlayer2 ? 
                humanPlayer2.GetAvailableNumbers() :
                Player2 is TicTacToeComputerPlayer computerPlayer2 ? 
                computerPlayer2.GetAvailableNumbers() : new HashSet<int>();
        }

        // Setters for data restoration
        public void SetGameMode(string gameMode) => GameMode = gameMode;
        public void SetIsGameOver(bool isGameOver) => IsGameOver = isGameOver;
        public void SetTargetSum(int targetSum) => TargetSum = targetSum;
        public void SetTicTacToeBoard(TicTacToeBoard board) => TicTacToeBoard = board;
        public void SetMoveHistory(Stack<Move> moveHistory) => MoveHistory = moveHistory;
        public void SetRedoHistory(Stack<Move> redoHistory) => RedoHistory = redoHistory;

        public void SetCurrentPlayerByName(string playerName)
        {
            CurrentPlayer = playerName == Player1.Name ? Player1 : Player2;
        }

        public void RestorePlayersFromData(string gameMode, string player1Name, string player2Name, 
                                        List<int> player1Numbers, List<int> player2Numbers)
        {
            if (gameMode == "HvH")
            {
                Player1 = new TicTacToeHumanPlayer(player1Name, new HashSet<int>(player1Numbers));
                Player2 = new TicTacToeHumanPlayer(player2Name, new HashSet<int>(player2Numbers));
            }
            else
            {
                Player1 = new TicTacToeHumanPlayer(player1Name, new HashSet<int>(player1Numbers));
                Player2 = new TicTacToeComputerPlayer(new HashSet<int>(player2Numbers));
            }
        }

        protected override void ConfigureGame()
        {
            int boardSize = SelectBoardSize();
            TicTacToeBoard = new TicTacToeBoard(boardSize);
            Board = TicTacToeBoard;

            SelectGameMode();

            // Initialize numbers (tictactoe-specific)
            for (int i = 1; i <= boardSize * boardSize; ++i)
            {
                if (i % 2 == 1)
                    OddNumbers.Add(i);
                else
                    EvenNumbers.Add(i);
            }

            TargetSum = boardSize * (boardSize * boardSize + 1) / 2;
        }

        public int SelectBoardSize()
        {
            renderer.DisplayMessage("\n|| +++ Size of the board +++ ||");
            return inputHandler.GetUserIntInput("Select the size of the board (3 => 3X3/ 4 => 4X4/ 5 => 5X5/ etc.)", 3, 15);
        }


        protected override void ConfigurePlayer()
        {
            ConfigurePlayersWithNames();
        }
        
        protected override void CreateHumanVsHumanPlayers(string player1Name, string player2Name)
        {
            Player1 = new TicTacToeHumanPlayer(player1Name, OddNumbers);
            Player2 = new TicTacToeHumanPlayer(player2Name, EvenNumbers);
        }

        protected override void CreateHumanVsComputerPlayers(string playerName)
        {
            Player1 = new TicTacToeHumanPlayer(playerName, OddNumbers);
            Player2 = new TicTacToeComputerPlayer(EvenNumbers);
        }

        public override void StartGame()
        {
            renderer.DisplayMessage("\n============================================ Game Started!  ============================================");
            IsGameOver = false;
            IsPlayerQuit = false; // Reset the quit flag when starting

            // Offer undo after loading a game -> MoveHistory > 0 means it is a loaded game
            if (MoveHistory.Count > 0)
            {
                renderer.DisplayGameStatus(CurrentPlayer.Name, MoveHistory.Count);
                renderer.DisplayBoard(Board);
                OfferUndoAfterLoad();
            }

            while (!IsGameOver)
            {
                renderer.DisplayGameStatus(CurrentPlayer.Name, MoveHistory.Count);
                renderer.DisplayBoard(Board);

                if (CurrentPlayer.Type == PlayerType.Human)
                {
                    ProcessHumanTurn();

                    // Check if player quit during their turn
                    if (IsPlayerQuit)
                    {
                        break; // Exit the game loop immediately
                    }
                }
                else
                {
                    ProcessComputerTurn();
                }

                // Only check game over if player didn't quit
                if (!IsPlayerQuit)
                {
                    IsGameOver = CheckGameOver();

                    if (!IsGameOver)
                    {
                        SwitchCurrentPlayer();
                    }
                }
            }

            // Only announce winner and display result if player didn't quit
            if (!IsPlayerQuit)
            {
                renderer.DisplayGameStatus(CurrentPlayer.Name, MoveHistory.Count);
                renderer.DisplayBoard(Board);
                AnnounceWinner();
                DisplayGameResult();
            }
        }
        
        protected override void HandleUndoRequest()
        {
            int maxUndo = GetUndoableMoveCountForPlayer(CurrentPlayer);
            if (maxUndo > 0)
            {
                Console.Write($"How many moves to undo (up to {maxUndo})? ");
                if (int.TryParse(Console.ReadLine(), out int undoCount) && undoCount > 0 && undoCount <= maxUndo)
                {
                    // Save a reference to total move count before undoing
                    int beforeMoveCount = MoveHistory.Count;
                    
                    // Use the method which correctly filters by player
                    UndoPlayerMoves(CurrentPlayer, undoCount);
                    
                    // Calculate how many total moves were undone (could be more than undoCount
                    // if we had to skip opponent moves)
                    int movesUndone = beforeMoveCount - MoveHistory.Count;
                    
                    UndoneMovesCount += undoCount;
                    
                    // After undoing, we need to ensure turns are maintained correctly
                    // If we undid an odd number of moves, we need to switch current player
                    if (movesUndone % 2 == 1)
                    {
                        SwitchCurrentPlayer();
                        renderer.DisplayMessage($"Turn switched to {CurrentPlayer.Name}");
                    }
                    
                    renderer.DisplayBoard(Board); // Show the board after undo
                }
                else
                {
                    renderer.DisplayMessage($"Invalid input. You can undo up to {maxUndo} of your move(s).");
                }
            }
            else
            {
                renderer.DisplayMessage("No moves to undo.");
            }
        }

        protected override void OfferUndoAfterLoad()
        {
            int maxUndo = GetUndoableMoveCountForPlayer(CurrentPlayer);

            if (maxUndo > 0)
            {
                renderer.DisplayMessage($"\nYou have {maxUndo} move(s) that can be undone.");
                bool confirm = inputHandler.GetUserConfirmation("Would you like to undo any moves?");

                if (confirm)
                {
                    renderer.DisplayMessage($"How many moves to undo (up to {maxUndo})? ");
                    if (int.TryParse(Console.ReadLine(), out int undoCount) && undoCount > 0 && undoCount <= maxUndo)
                    {
                        // Use the base class method which correctly filters by player
                        UndoPlayerMoves(CurrentPlayer, undoCount);
                        UndoneMovesCount = undoCount;

                        // If it's now a computer's turn after undoing, let it play
                        if (CurrentPlayer.Type == PlayerType.Computer)
                        {
                            renderer.DisplayGameStatus(CurrentPlayer.Name, MoveHistory.Count);
                            ProcessComputerTurn();

                            // Check if the game is over after computer move
                            IsGameOver = CheckGameOver();
                            if (!IsGameOver)
                            {
                                SwitchCurrentPlayer();
                            }
                        }
                    }
                    else
                    {
                        renderer.DisplayMessage("Invalid input. No moves will be undone.");
                    }
                }
            }
        }

        public override void Play()
        {
            ConfigureGame();
            ConfigurePlayer();
            StartGame();
        }

        protected override void MakeHumanMove()
        {
            var humanPlayer = (TicTacToeHumanPlayer)CurrentPlayer;
            bool moveCompleted = false;
            
            while (!moveCompleted)
            {
                try
                {
                    // Get number from player
                    int number = (int)humanPlayer.SelectMove(Board);
                    
                    // Get position from player
                    int[] position = TicTacToeBoard.SelectPosition();
                    
                    // Validate the move
                    if (!Board.IsValidMove(position[0], position[1], number, 0, true))
                    {
                        // Return number to player and try again
                        humanPlayer.GetAvailableNumbers().Add(number);
                        renderer.DisplayMessage("Invalid move! Please try again.");
                        continue; // Go back to start of loop
                    }

                    // Save state for potential undo
                    object previousState = Board.GetBoardState();
                    
                    // Make the move temporarily
                    Board.MakeMove(position[0], position[1], number);
                    
                    // Show the result
                    renderer.DisplayBoard(Board);
                    
                    // Ask for confirmation
                    bool confirmed = inputHandler.GetUserConfirmation("Confirm this move? [ y - confirm | n - redo move ] >>");
                    
                    if (confirmed)
                    {
                        // Move is confirmed - add to history
                        var move = new Move(0, position[0], position[1], CurrentPlayer, number, previousState);
                        MoveHistory.Push(move);
                        ClearRedoStackOnNewMove();
                        
                        renderer.DisplayMessage($"{CurrentPlayer.Name} placed {number} at position ({position[0] + 1}, {position[1] + 1})");
                        moveCompleted = true; // Exit the loop
                    }
                    else
                    {
                        // Move cancelled - restore state and try again
                        Board.SetBoardState(previousState);
                        humanPlayer.GetAvailableNumbers().Add(number);
                        renderer.DisplayMessage("Move cancelled. Please make another move.");
                        // Loop continues for another attempt
                    }
                }
                catch (Exception ex)
                {
                    renderer.DisplayMessage($"Error making move: {ex.Message}. Please try again.");
                    // Loop continues for another attempt
                }
            }
        }


        protected override void ProcessComputerTurn()
        {
            renderer.DisplayMessage("\nComputer is making a move...");
            var computerPlayer = (TicTacToeComputerPlayer)CurrentPlayer;
            
            // Try to find winning move first
            object winningMove = computerPlayer.FindWinningMove(Board);
            
            if (winningMove != null)
            {
                int number = (int)winningMove;
                MakeComputerMoveWithNumber(number);
            }
            else
            {
                int number = (int)computerPlayer.SelectRandomMove();
                MakeComputerMoveWithNumber(number);
            }
        }

        // Numerical TicTacToe specific computer move
        private void MakeComputerMoveWithNumber(int number)
        {
            int boardSize = TicTacToeBoard.GetSize();
            var computerPlayer = (TicTacToeComputerPlayer)CurrentPlayer;
            
            // Find a valid position
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (Board.IsValidMove(row, col, number, 0, false))
                    {
                        object previousState = Board.GetBoardState();
                        Board.MakeMove(row, col, number);
                        
                        // Add to move history
                        var move = new Move(0, row, col, CurrentPlayer, number, previousState);
                        MoveHistory.Push(move);
                        ClearRedoStackOnNewMove();
                        
                        // Remove number from available set
                        computerPlayer.GetAvailableNumbers().Remove(number);
                        
                        renderer.DisplayMessage($"Computer placed {number} at position ({row + 1}, {col + 1})");
                        return;
                    }
                }
            }
        }

        protected override void HandleQuitRequest()
        {
            IsGameOver = true;
            IsPlayerQuit = true;
        }

        protected override bool CheckGameOver()
        {
            // Check for a winning line
            if (CheckWinningLine())
                return true;

            // Check if the board is full
            if (Board.IsBoardFull())
                return true;

            return false;
        }

        private bool CheckWinningLine()
        {
            int boardSize = Board.GetSize();

            // Check rows
            for (int row = 0; row < boardSize; row++)
            {
                if (CalculateSumInLine("row", row))
                    return true;
            }

            // Check columns
            for (int col = 0; col < boardSize; col++)
            {
                if (CalculateSumInLine("col", col))
                    return true;
            }

            // Check diagonal
            if (CalculateSumInLine("diagonal", 0))
                return true;

            // Check anti-diagonal
            if (CalculateSumInLine("anti-diagonal", 0))
                return true;

            return false;
        }

        private bool CalculateSumInLine(string line, int index)
        {
            int sum = 0;
            bool hasEmptySlot = false;
            int boardSize = Board.GetSize();

            switch (line)
            {
                case "row":
                    // Check if the line has empty slot
                    for (int i = 0; i < boardSize; i++)
                    {
                        int slotValue = ((int[,])Board.GetBoardState())[index, i];
                        if (slotValue == 0)
                        {
                            hasEmptySlot = true;
                            break;
                        }
                        sum += slotValue;
                    }
                    break;

                case "col":
                    for (int i = 0; i < boardSize; i++)
                    {
                        int slotValue = ((int[,])Board.GetBoardState())[i, index];
                        if (slotValue == 0)
                        {
                            hasEmptySlot = true;
                            break;
                        }
                        sum += slotValue;
                    }
                    break;

                case "diagonal":
                    for (int i = 0; i < boardSize; i++)
                    {
                        int slotValue = ((int[,])Board.GetBoardState())[i, i];
                        if (slotValue == 0)
                        {
                            hasEmptySlot = true;
                            break;
                        }
                        sum += slotValue;
                    }
                    break;

                case "anti-diagonal":
                    for (int i = 0; i < boardSize; i++)
                    {
                        int slotValue = ((int[,])Board.GetBoardState())[i, boardSize - 1 - i];
                        if (slotValue == 0)
                        {
                            hasEmptySlot = true;
                            break;
                        }
                        sum += slotValue;
                    }
                    break;
            }

            // Check if the line is full and sum equals target
            return !hasEmptySlot && sum == TargetSum;
        }

        private bool IsWinningMove(int row, int col)
        {
            int boardSize = Board.GetSize();

            // Check row
            if (CalculateSumInLine("row", row))
                return true;

            // Check column
            if (CalculateSumInLine("col", col))
                return true;

            // Check diagonal if applicable
            if (row == col && CalculateSumInLine("diagonal", 0))
                return true;

            // Check anti-diagonal if applicable
            if (row + col == boardSize - 1 && CalculateSumInLine("anti-diagonal", 0))
                return true;

            return false;
        }

        protected override void AnnounceWinner()
        {
            if (Board.IsBoardFull() && !CheckWinningLine())
            {
                renderer.DisplayMessage("\nGame over! It's a draw!");
                renderer.PressAnyKeyToContinue();
            }
            else
            {
                renderer.DisplayMessage($"\nGame over! {CurrentPlayer.Name} wins!");
                renderer.PressAnyKeyToContinue();
            }
        }

        private void DisplayGameResult()
        {
            renderer.DisplayMessage($"\nFinal Turn: {CurrentPlayer.Name}");
            renderer.DisplayMessage($"Target Sum: {TargetSum}");
        }

        // Save game : Serialize necessary game information -> save board, boardState, move history to JSON supported form
        protected override GameData CreateGameData()
        {
            return new TicTacToeGameData();
        }

        protected override void SaveGameData(GameData gameData, string filename)
        {
            if (gameData is TicTacToeGameData tttData)
            {
                dataPersistence.SaveGameData(tttData, filename);
            }
            else
            {
                throw new InvalidOperationException("Invalid game data type for TicTacToe");
            }
        }

        protected override GameData LoadGameData(string filename)
        {
            return dataPersistence.LoadGameData<TicTacToeGameData>(filename);
        }

        // Override for applying undo state
        protected override void ApplyUndoState(Move move)
        {
            // Restore the board state
            Board.SetBoardState(move.PreviousBoardState);

            // Return the number: ONLY to the player who made the move
            int number = (int)move.MoveData;

            // Make sure to return the number to the opponent player who made the move
            // Not to the current player who just undid
            if (move.Player is TicTacToeHumanPlayer humanPlayer)
            {
                humanPlayer.GetAvailableNumbers().Add(number);
            }
            else if (move.Player is TicTacToeComputerPlayer computerPlayer)
            {
                computerPlayer.GetAvailableNumbers().Add(number);
            }

            renderer.DisplayMessage($"\nMove undone for {move.Player.Name}");
        }

        protected override void ApplyRedoState(Move move)
        {
            // Restore the board state to the previous state before the move
            Board.SetBoardState(move.PreviousBoardState);

            // Add the number from the player's available numbers
            int number = (int)move.MoveData;

            if (move.Player is TicTacToeHumanPlayer humanPlayer)
            {
                humanPlayer.GetAvailableNumbers().Add(number);
            }
            else if (move.Player is TicTacToeComputerPlayer computerPlayer)
            {
                computerPlayer.GetAvailableNumbers().Add(number);
            }

            renderer.DisplayMessage($"\nMove redone. Current player: {CurrentPlayer.Name}");
        }

        protected override void SwitchCurrentPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
        }

        protected override string GetGameRules()
        {
        return @"
            ============================================ Numerical Tic-Tac-Toe Rules ============================================

            OBJECTIVE:
            The goal is to be the first player to get a sum equal to the target sum in any row, column, or diagonal.

            GAME SETUP:
            - The game is played on an NxN grid (you choose the size: 3x3, 4x4, 5x5, etc.)
            - Player 1 uses ODD numbers (1, 3, 5, 7, 9, ...)
            - Player 2 uses EVEN numbers (2, 4, 6, 8, 10, ...)
            - Each number can only be used once during the game

            TARGET SUM:
            The target sum is calculated as: N × (N² + 1) ÷ 2
            - For 3x3 board: Target sum = 15
            - For 4x4 board: Target sum = 34
            - For 5x5 board: Target sum = 65

            HOW TO WIN:
            - Get a complete row, column, or diagonal that adds up to exactly the target sum
            - All positions in the line must be filled (no empty spaces)

            GAME MODES:
            - HvH (Human vs Human): Two players take turns
            - HvC (Human vs Computer): Play against the computer

            GAME FLOW:
            1. Players take turns selecting a number from their available set
            2. Place the number on any empty position on the board
            3. First player to achieve the target sum in any line wins
            4. If the board fills up with no winner, the game is a draw";
        }
        
        protected override string GetGameCommands()
        {
            return @"
            ============================================ Game Commands ============================================

            DURING YOUR TURN:
            1. Make a move     - Select a number and place it on the board
            2. Undo moves      - Revert previous moves (you can undo multiple moves)
            3. Save game       - Save the current game state to a file
            4. View help       - Display these instructions and game rules
            5. Quit game       - Exit the game (with confirmation)

            MAKING A MOVE:
            1. Choose a number from your available set (odd or even numbers)
            2. Select a position on the board (1 to N²)
            3. Confirm or redo your move before ending your turn

            UNDO SYSTEM:
            - You can undo any number of YOUR moves (not opponent's moves)
            - Undoing is useful for trying different strategies
            - After loading a saved game, you can immediately undo moves

            REDO SYSTEM:
            - You can redo right after your initial move.
            - Redoing is useful for adjusting the move that has already been placed.
            - You can redo any number of times, until you confirm your move.

            SAVE/LOAD SYSTEM:
            - Save games at any point during play
            - Enter a filename (without extension) when saving
            - Use the same filename when loading
            - Saved games remember the exact game state, including move history

            TIPS:
            - Plan ahead: Consider what numbers your opponent has available
            - Watch for defensive plays: Block opponent's potential winning lines
            - Use smaller numbers early to save larger numbers for strategic plays";
        }
    }
}
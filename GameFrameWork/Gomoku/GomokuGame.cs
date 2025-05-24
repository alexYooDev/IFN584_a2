using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GameFrameWork
{
    public class GomokuGame : AbstractGame
    {
        private GomokuBoard GomokuBoard;
        private List<(int, int)> winningLine;

        // Constructor with dependency injection
        public GomokuGame(IGameRenderer renderer, IInputHandler inputHandler, IGameDataPersistence dataPersistence) 
            : base(renderer, inputHandler, dataPersistence)
        {
        }

        // Default constructor using console implementations
        public GomokuGame() : this(new ConsoleGameRenderer(), new ConsoleInputHandler(), new JsonGameDataPersistence())
        {
        }

        // Public getters for data access (following TicTacToe pattern)
        public string GetGameMode() => GameMode;
        public string GetCurrentPlayerName() => CurrentPlayer.Name;
        public string GetPlayer1Name() => Player1.Name;
        public string GetPlayer2Name() => Player2.Name;
        public bool GetIsGameOver() => IsGameOver;
        public GomokuBoard GetGomokuBoard() => GomokuBoard;
        public AbstractPlayer GetPlayer1() => Player1;
        public AbstractPlayer GetPlayer2() => Player2;
        public Stack<Move> GetMoveHistory() => MoveHistory;
        public Stack<Move> GetRedoHistory() => RedoHistory;

        // Setters for data restoration
        public void SetGameMode(string gameMode) => GameMode = gameMode;
        public void SetIsGameOver(bool isGameOver) => IsGameOver = isGameOver;
        public void SetGomokuBoard(GomokuBoard board) => GomokuBoard = board;
        public void SetMoveHistory(Stack<Move> moveHistory) => MoveHistory = moveHistory;
        public void SetRedoHistory(Stack<Move> redoHistory) => RedoHistory = redoHistory;

        public void SetCurrentPlayerByName(string playerName)
        {
            CurrentPlayer = playerName == Player1.Name ? Player1 : Player2;
        }

        public void RestorePlayersFromData(string gameMode, string player1Name, string player2Name)
        {
            if (gameMode == "HvH")
            {
                Player1 = new GomokuHumanPlayer(player1Name, 'X');
                Player2 = new GomokuHumanPlayer(player2Name, 'O');
            }
            else
            {
                Player1 = new GomokuHumanPlayer(player1Name, 'X');
                Player2 = new GomokuComputerPlayer("Computer", 'O');
            }
        }

        protected override void ConfigureGame()
        {
            int boardSize = SelectBoardSize();
            GomokuBoard = new GomokuBoard(boardSize, 1);
            Board = GomokuBoard;
            SelectGameMode();
        }

        private int SelectBoardSize()
        {
            renderer.DisplayMessage("\n|| +++ Size of the Gomoku board +++ ||");
            return inputHandler.GetUserIntInput("Select the size of the board (15 by default, minimum 5)", 5, 25);
        }

        private void SelectGameMode()
        {
            renderer.DisplayMessage("\n|| +++ Select the mode of the game +++ ||");
            renderer.DisplayMessage("1. HvH (Human vs Human)");
            renderer.DisplayMessage("2. HvC (Human vs Computer)");
            
            int modeChoice = inputHandler.GetUserIntInput("Enter your choice", 1, 2);

            switch (modeChoice)
            {
                case 1:
                    renderer.DisplayMessage("\nYou selected Human vs Human mode.");
                    GameMode = "HvH";
                    break;
                case 2:
                    renderer.DisplayMessage("\nYou selected Human vs Computer mode.");
                    GameMode = "HvC";
                    break;
            }
        }

        protected override void ConfigurePlayer()
        {
            ConfigurePlayersWithNames();
        }

        protected override void CreateHumanVsHumanPlayers(string player1Name, string player2Name)
        {
            Player1 = new GomokuHumanPlayer(player1Name, 'X');
            Player2 = new GomokuHumanPlayer(player2Name, 'O');
        }

        protected override void CreateHumanVsComputerPlayers(string playerName)
        {
            Player1 = new GomokuHumanPlayer(playerName, 'X');
            Player2 = new GomokuComputerPlayer("Computer", 'O');
        }

        public override void StartGame()
        {
            renderer.DisplayMessage("\n============================================ Game Started!  ============================================");
            IsGameOver = false;
            IsPlayerQuit = false;

            // Offer undo after loading a game
            if (MoveHistory.Count > 0)
            {
                DisplayGameStatus();
                OfferUndoAfterLoad();
            }

            while (!IsGameOver)
            {
                DisplayGameStatus();

                if (CurrentPlayer.Type == PlayerType.Human)
                {
                    ProcessHumanTurn();

                    if (IsPlayerQuit)
                    {
                        break;
                    }
                }
                else
                {
                    ProcessComputerTurn();
                }

                if (!IsPlayerQuit)
                {
                    IsGameOver = CheckGameOver();

                    if (!IsGameOver)
                    {
                        SwitchCurrentPlayer();
                    }
                }
            }

            if (!IsPlayerQuit)
            {
                DisplayGameStatus();
                AnnounceWinner();
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
            GomokuHumanPlayer humanPlayer = (GomokuHumanPlayer)CurrentPlayer;
            char symbol = (char)humanPlayer.SelectMove(Board);

            int[] position = GomokuBoard.SelectPosition();
            int row = position[0];
            int col = position[1];

            object previousState = Board.GetBoardState();

            if (Board.IsValidMove(row, col, symbol, 0, true))
            {
                Board.MakeMove(row, col, symbol);
                renderer.DisplayBoard(Board);

                bool confirmed = inputHandler.GetUserConfirmation("Confirm this move? [ y - confirm | n - redo move ] >> ");

                if (confirmed)
                {
                    var move = new Move(0, row, col, CurrentPlayer, symbol, previousState);
                    MoveHistory.Push(move);
                    ClearRedoStackOnNewMove();
                }
                else
                {
                    Board.SetBoardState(previousState);
                    renderer.DisplayMessage("Move cancelled. Try again.");
                    MakeHumanMove();
                }
            }
            else
            {
                renderer.DisplayMessage("Invalid move! Please try again.");
                MakeHumanMove();
            }
        }

        protected override void ProcessComputerTurn()
        {
            renderer.DisplayMessage("\nComputer is making a move...");
            GomokuComputerPlayer computerPlayer = (GomokuComputerPlayer)CurrentPlayer;

            object winningMove = computerPlayer.FindWinningMove(Board);

            if (winningMove != null)
            {
                GomokuMove move = computerPlayer.GetFavorableMove();
                MakeComputerMoveAtPosition(move.Row, move.Col);
            }
            else
            {
                object randomMove = computerPlayer.SelectRandomMove();
                if (randomMove != null)
                {
                    GomokuMove move = computerPlayer.GetFavorableMove();
                    MakeComputerMoveAtPosition(move.Row, move.Col);
                }
            }
        }

        private void MakeComputerMoveAtPosition(int row, int col)
        {
            object previousState = Board.GetBoardState();
            char symbol = (char)CurrentPlayer.MoveSymbol;
            Board.MakeMove(row, col, symbol);

            var move = new Move(0, row, col, CurrentPlayer, symbol, previousState);
            MoveHistory.Push(move);
            ClearRedoStackOnNewMove();

            renderer.DisplayMessage($"\nComputer placed {symbol} at position ({row + 1}, {col + 1})");
        }

        protected override void HandleUndoRequest()
        {
            int maxUndo = GetUndoableMoveCountForPlayer(CurrentPlayer);
            if (maxUndo > 0)
            {
                int undoCount = inputHandler.GetUserIntInput($"How many moves to undo (up to {maxUndo})?", 1, maxUndo);
                
                UndoPlayerMoves(CurrentPlayer, undoCount);
                
                Board.DisplayBoard(); // Show the board after undo
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
                bool wantsToUndo = inputHandler.GetUserConfirmation("Would you like to undo any moves?");

                if (wantsToUndo)
                {
                    int undoCount = inputHandler.GetUserIntInput($"How many moves to undo (up to {maxUndo})?", 1, maxUndo);
                    
                    // Use the base class method which correctly filters by player
                    UndoPlayerMoves(CurrentPlayer, undoCount);

                    // If it's now a computer's turn after undoing, let it play
                    if (CurrentPlayer.Type == PlayerType.Computer)
                    {
                        DisplayGameStatus();
                        ProcessComputerTurn();

                        // Check if the game is over after computer move
                        IsGameOver = CheckGameOver();
                        if (!IsGameOver)
                        {
                            SwitchCurrentPlayer();
                        }
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
            if (MoveHistory.Count > 0)
            {
                var lastMove = MoveHistory.Peek();
                winningLine = GomokuBoard.CheckForWinningCondition(lastMove.Row, lastMove.Col, (char)lastMove.MoveData);

                if (winningLine != null)
                {
                    return true;
                }
            }

            return Board.IsBoardFull();
        }

        protected override void AnnounceWinner()
        {
            if (winningLine != null)
            {
                renderer.DisplayMessage($"\nGame over! {CurrentPlayer.Name} wins!");
                renderer.DisplayMessage($"Winning line: {string.Join(" -> ", winningLine.Select(pos => $"({pos.Item1},{pos.Item2})"))}");
            }
            else if (Board.IsBoardFull())
            {
                renderer.DisplayMessage("\nGame over! It's a draw!");
            }
            renderer.PressAnyKeyToContinue();
        }

        protected override void DisplayGameStatus()
        {
            renderer.DisplayMessage($"\nCurrent Turn: {CurrentPlayer.Name} ({CurrentPlayer.MoveSymbol})");
            renderer.DisplayMessage($"Move #{MoveHistory.Count}");
            Board.DisplayBoard();
        }


        protected override void ApplyUndoState(Move move)
        {
            Board.SetBoardState(move.PreviousBoardState);
            renderer.DisplayMessage($"\nMove undone for {move.Player.Name}");
        }

        protected override void ApplyRedoState(Move move)
        {
            Board.MakeMove(move.Row, move.Col, move.MoveData);
            renderer.DisplayMessage($"\nMove redone for {move.Player.Name}");
        }

        protected override GameData CreateGameData()
        {
            return new GomokuGameData();
        }

        protected override void SaveGameData(GameData gameData, string filename)
        {
            if (gameData is GomokuGameData gomokuData)
            {
                dataPersistence.SaveGameData(gomokuData, filename);
            }
            else
            {
                throw new InvalidOperationException("Invalid game data type for Notakto");
            }
        }

        protected override GameData LoadGameData(string filename)
        {
            return dataPersistence.LoadGameData<GomokuGameData>(filename);
        }

        protected override string GetGameRules()
        {
            return @"
============================================ Gomoku Rules ============================================

The game is played on a grid of size NxN (typically 15x15).
Player 1 uses X stones, Player 2 uses O stones.

The goal is to be the first to get exactly 5 stones in a row.
A winning line can be horizontal, vertical, or diagonal.

The game can be played in two modes: HvH (Human vs Human) or HvC (Human vs Computer).

Players take turns placing one stone at a time on empty intersections.
The game ends when one player gets 5 in a row or the board is full (draw).";
        }

        protected override string GetGameCommands()
        {
            return @"
============================================ Game Commands ============================================

During your turn, you can choose from the following options:
1. Make a move - Place a stone on the board
2. Undo previous moves - Revert to an earlier state of the game
3. Save the game - Save the current game state
4. View help menu - Display game rules and commands
5. Quit the game - Exit the application

When making a move:
1. Enter the row coordinate (0 to board size - 1)
2. Enter the column coordinate (0 to board size - 1)
3. You can either confirm your move or redo it before ending your turn

Saving and loading games:
- When saving, enter a filename without an extension
- When loading, enter the same filename you used to save
- After loading a game, you'll have the option to undo moves";
        }
    }
}
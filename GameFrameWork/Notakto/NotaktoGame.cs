using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GameFrameWork
{
    public class NotaktoGame : AbstractGame
    {
        private NotaktoBoard NotaktoBoard;

        // Constructor with dependency injection
        public NotaktoGame(IGameRenderer renderer, IInputHandler inputHandler, IGameDataPersistence dataPersistence) 
            : base(renderer, inputHandler, dataPersistence)
        {
        }

        // Default constructor using console implementations
        public NotaktoGame() : this(new ConsoleGameRenderer(), new ConsoleInputHandler(), new JsonGameDataPersistence())
        {
        }

        // Public getters for data access (following TicTacToe pattern)
        public string GetGameMode() => GameMode;
        public string GetCurrentPlayerName() => CurrentPlayer.Name;
        public string GetPlayer1Name() => Player1.Name;
        public string GetPlayer2Name() => Player2.Name;
        public bool GetIsGameOver() => IsGameOver;
        public NotaktoBoard GetNotaktoBoard() => NotaktoBoard;
        public AbstractPlayer GetPlayer1() => Player1;
        public AbstractPlayer GetPlayer2() => Player2;
        public Stack<Move> GetMoveHistory() => MoveHistory;
        public Stack<Move> GetRedoHistory() => RedoHistory;

        // Setters for data restoration
        public void SetGameMode(string gameMode) => GameMode = gameMode;
        public void SetIsGameOver(bool isGameOver) => IsGameOver = isGameOver;
        public void SetNotaktoBoard(NotaktoBoard board) => NotaktoBoard = board;
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
                Player1 = new NotaktoHumanPlayer(player1Name);
                Player2 = new NotaktoHumanPlayer(player2Name);
            }
            else
            {
                Player1 = new NotaktoHumanPlayer(player1Name);
                Player2 = new NotaktoComputerPlayer();
            }
        }

        protected override void ConfigureGame()
        {
            NotaktoBoard = new NotaktoBoard();
            Board = NotaktoBoard;
            SelectGameMode();
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
            Player1 = new NotaktoHumanPlayer(player1Name);
            Player2 = new NotaktoHumanPlayer(player2Name);
        }

        protected override void CreateHumanVsComputerPlayers(string playerName)
        {
            Player1 = new NotaktoHumanPlayer(playerName);
            Player2 = new NotaktoComputerPlayer();
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
            NotaktoHumanPlayer humanPlayer = (NotaktoHumanPlayer)CurrentPlayer;
            char symbol = (char)humanPlayer.SelectMove(Board);

            int[] position = NotaktoBoard.SelectPosition();
            int selectedBoard = position[0];
            int row = position[1];
            int col = position[2];

            object previousState = Board.GetBoardState();

            if (Board.IsValidMove(row, col, symbol, selectedBoard, true))
            {
                Board.MakeMove(row, col, symbol, selectedBoard);
                renderer.DisplayBoard(Board);

                bool confirmed = inputHandler.GetUserConfirmation("Confirm this move? [ y - confirm | n - redo move ] >>");

                if (confirmed)
                {
                    var move = new Move(selectedBoard, row, col, CurrentPlayer, symbol, previousState);
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
            NotaktoComputerPlayer computerPlayer = (NotaktoComputerPlayer)CurrentPlayer;

            computerPlayer.SetBoard(NotaktoBoard);

            object winningMove = computerPlayer.FindWinningMove(Board);

            if (winningMove != null)
            {
                int[] move = computerPlayer.GetFavorableMove();
                MakeComputerMoveAtPosition(move[0], move[1], move[2]);
            }
            else
            {
                object randomMove = computerPlayer.SelectRandomMove();
                if (randomMove != null)
                {
                    int[] move = computerPlayer.GetFavorableMove();
                    MakeComputerMoveAtPosition(move[0], move[1], move[2]);
                }
            }
        }

        private void MakeComputerMoveAtPosition(int boardIndex, int row, int col)
        {
            object previousState = Board.GetBoardState();
            char symbol = (char)CurrentPlayer.MoveSymbol;
            Board.MakeMove(row, col, symbol, boardIndex);

            var move = new Move(boardIndex, row, col, CurrentPlayer, symbol, previousState);
            MoveHistory.Push(move);
            ClearRedoStackOnNewMove();

            renderer.DisplayMessage($"\nComputer placed {symbol} at Board {boardIndex + 1}, position ({row + 1}, {col + 1})");
        }

        protected override void HandleQuitRequest()
        {
            IsGameOver = true;
            IsPlayerQuit = true;
        }

        protected override bool CheckGameOver()
        {
            return NotaktoBoard.AreAllBoardsDead();
        }

        protected override void AnnounceWinner()
        {
            if (NotaktoBoard.AreAllBoardsDead())
            {
                renderer.DisplayMessage($"\nGame over! All boards are dead!");
                renderer.DisplayMessage($"{CurrentPlayer.Name} made the final move and loses!");

                AbstractPlayer winner = (CurrentPlayer == Player1) ? Player2 : Player1;
                renderer.DisplayMessage($"{winner.Name} wins!");
            }
            renderer.PressAnyKeyToContinue();
        }

        protected override void DisplayGameStatus()
        {
            renderer.DisplayMessage($"\nCurrent Turn: {CurrentPlayer.Name}");
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
            Board.MakeMove(move.Row, move.Col, move.MoveData, move.BoardIndex);
            renderer.DisplayMessage($"\nMove redone for {move.Player.Name}");
        }

        protected override GameData CreateGameData()
        {
            return new NotaktoGameData();
        }

        protected override void SaveGameData(GameData gameData, string filename)
        {
            if (gameData is NotaktoGameData notaktoData)
            {
                dataPersistence.SaveGameData(notaktoData, filename);
            }
            else
            {
                throw new InvalidOperationException("Invalid game data type for Notakto");
            }
        }

        protected override GameData LoadGameData(string filename)
        {
            return dataPersistence.LoadGameData<NotaktoGameData>(filename);
        }

        protected override string GetGameRules()
        {
            return @"
============================================ Notakto Rules ============================================

Notakto is played on three 3x3 boards.
Both players use the same symbol: X

Players take turns placing X on any empty cell on any live board.
When a board gets three X's in a row (horizontally, vertically, or diagonally), it becomes 'dead'.
The player who is forced to complete the three-in-a-row on the LAST live board loses!

Strategy tip: Try to avoid making moves that will force you to complete the final board.";
        }

        protected override string GetGameCommands()
        {
            return @"
============================================ Notakto Commands ============================================

During your turn, you can choose from the following options:
1. Make a move - Place an X on one of the live boards
2. Undo previous moves - Revert to an earlier state of the game
3. Save the game - Save the current game state
4. View help menu - Display game rules and commands
5. Quit the game - Exit the application

When making a move:
1. First select which board (1-3) you want to play on
2. Then select the position (1-9) where you want to place X
3. You can confirm your move or redo it before ending your turn

Remember: Avoid being the player who completes the last board!";
        }
    }
}
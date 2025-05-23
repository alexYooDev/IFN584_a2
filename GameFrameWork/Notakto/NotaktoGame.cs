using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace GameFrameWork
{
    public class NotaktoGame : AbstractGame
    {
        private NotaktoBoard NotaktoBoard;
        // Temporary storage for move before confirmation
        private Move TempMove;

        public NotaktoGame() : base() { }

        public override void ConfigureGame()
        {
            NotaktoBoard = new NotaktoBoard();
            Board = NotaktoBoard;
            SelectGameMode();
        }

        public void SelectGameMode()
        {
            Console.WriteLine("\n|| +++ Select the mode of the game +++ ||");
            Console.WriteLine("1. HvH (Human vs Human)");
            Console.WriteLine("2. HvC (Human vs Computer)");
            Console.Write("\nEnter your choice >> ");

            int modeChoice = Convert.ToInt32(Console.ReadLine());

            switch (modeChoice)
            {
                case 1:
                    Console.WriteLine("\nYou selected Human vs Human mode.");
                    GameMode = "HvH";
                    break;
                case 2:
                    Console.WriteLine("\nYou selected Human vs Computer mode.");
                    GameMode = "HvC";
                    break;
                default:
                    Console.WriteLine("\nInvalid mode selected. Defaulting to Human vs Human.");
                    GameMode = "HvH";
                    break;
            }
        }

        public override void ConfigurePlayer()
        {
            switch (GameMode)
            {
                case "HvH":
                    Console.Write("\nEnter player 1 name >> ");
                    string player1Name = Console.ReadLine();
                    Player1 = new NotaktoHumanPlayer(player1Name);

                    Console.Write("\nEnter player 2 name >> ");
                    string player2Name = Console.ReadLine();
                    Player2 = new NotaktoHumanPlayer(player2Name);
                    break;
                case "HvC":
                    Console.Write("\nEnter your name >> ");
                    string playerName = Console.ReadLine();
                    Player1 = new NotaktoHumanPlayer(playerName);
                    Player2 = new NotaktoComputerPlayer();
                    break;
            }

            CurrentPlayer = Player1;
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
                TempMove = new Move(selectedBoard, row, col, CurrentPlayer, symbol, previousState);
                Board.DisplayBoard();
                // Prompt redo/confirm options
                HandleMoveConfirmation();
            }
            else
            {
                // If the move is invalid, let the player try again
                Console.WriteLine("Invalid move! Please try again.");
                MakeHumanMove();
            }
        }

        private void HandleMoveConfirmation()
        {
            while (true)
            {
                Console.WriteLine("\nWhat would you like to do with this move?");
                Console.WriteLine("1. Redo this move (select a different position)");
                Console.WriteLine("2. Confirm and end your turn");
                Console.Write("\nEnter your choice >> ");
                string input = Console.ReadLine();

                if (input == "1")
                {
                     // Redo the move - restore the board state and return the number
                    Board.SetBoardState(TempMove.PreviousBoardState);
                    // Let the player make a new move
                    MakeHumanMove();

                    // Exit this loop after making a new move
                    return;
                }
                else if (input == "2")
                {
                    // Confirm the move - add it to move history
                    MoveHistory.Push(TempMove);
                    ClearRedoStackOnNewMove();
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }
        }

        protected override void ProcessComputerTurn()
        {
            Console.WriteLine("\nComputer is making a move...");
            NotaktoComputerPlayer computerPlayer = (NotaktoComputerPlayer)CurrentPlayer;

            // Set the board reference for the computer player
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

            Move move = new Move(boardIndex, row, col, CurrentPlayer, symbol, previousState);
            MoveHistory.Push(move);
            ClearRedoStackOnNewMove();

            Console.WriteLine($"\nComputer placed {symbol} at Board {boardIndex + 1}, position ({row + 1}, {col + 1})");
        }

        public override bool CheckGameOver()
        {
            return NotaktoBoard.AreAllBoardsDead();
        }

        // Display the result of the game
        protected override void AnnounceWinner()
        {
            // Condition with a winner
            if (NotaktoBoard.AreAllBoardsDead())
            {
                // In Notakto, the player who made the last move (filled the last board) loses
                Console.WriteLine($"\nGame over! All boards are dead!");
                Console.WriteLine($"{CurrentPlayer.Name} made the final move and loses!");

                // The winner is the other player
                AbstractPlayer winner = (CurrentPlayer == Player1) ? Player2 : Player1;
                Console.WriteLine($"{winner.Name} wins!");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        public override void DisplayGameStatus()
        {
            Console.WriteLine($"\nCurrent Turn: {CurrentPlayer.Name}");
            Console.WriteLine($"Move #{MoveHistory.Count}");
            Board.DisplayBoard();
        }

        protected override void SwithCurrentPlayer()
        {
            CurrentPlayer = (CurrentPlayer == Player1) ? Player2 : Player1;
        }

        protected override void ApplyUndoState(Move move)
        {
            Board.SetBoardState(move.PreviousBoardState);
            Console.WriteLine($"\nMove undone for {move.Player.Name}");
        }

        protected override void ApplyRedoState(Move move)
        {
            Board.MakeMove(move.Row, move.Col, move.MoveData, move.BoardIndex);
            Console.WriteLine($"\nMove redone for {move.Player.Name}");
        }

        public override void SaveGame(string filename)
        {
            try
            {
                string saveDirectory = Path.Combine(Directory.GetCurrentDirectory(), "saveData");

                if (!Directory.Exists(saveDirectory))
                {
                    Directory.CreateDirectory(saveDirectory);
                }

                var boardState = (Tuple<List<char[,]>, List<int>>)Board.GetBoardState();
                var gameData = new NotaktoGameData
                {
                    BoardSize = 3,
                    BoardCount = 3,
                    GameMode = GameMode,
                    CurrentPlayerName = CurrentPlayer.Name,
                    Player1Name = Player1.Name,
                    Player2Name = Player2.Name,
                    GameType = "Notakto",
                    IsGameOver = IsGameOver,
                    DeadBoards = boardState.Item2,
                    Boards = new List<string[][]>()
                };

                // Convert boards to serializable format
                foreach (var board in boardState.Item1)
                {
                    gameData.Boards.Add(NotaktoGameData.ConvertCharArrayToJagged(board));
                }

                // Serialize move history
                gameData.MoveHistory = new List<MovesToSerialize>();
                foreach (Move move in MoveHistory)
                {
                    gameData.MoveHistory.Add(new MovesToSerialize
                    {
                        BoardIndex = move.BoardIndex,
                        Row = move.Row,
                        Col = move.Col,
                        PlayerName = move.Player.Name,
                        MoveData = 1, // Just use 1 for X
                        PreviousBoardState = new int[0][] // Simplified for now
                    });
                }

                string jsonString = JsonSerializer.Serialize(gameData);
                string saveFilePath = Path.Combine(saveDirectory, filename + ".json");

                File.WriteAllText(saveFilePath, jsonString);
                Console.WriteLine($"\nGame saved successfully as {filename}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError saving game: {e.Message}");
            }
        }

        public override bool LoadGame(string filename)
        {
            try
            {
                string saveDirectory = Path.Combine(Directory.GetCurrentDirectory(), "saveData");
                string saveFilePath = Path.Combine(saveDirectory, filename + ".json");

                if (File.Exists(saveFilePath))
                {
                    string jsonString = File.ReadAllText(saveFilePath);
                    var gameData = JsonSerializer.Deserialize<NotaktoGameData>(jsonString);

                    NotaktoBoard = new NotaktoBoard();
                    Board = NotaktoBoard;

                    // Restore game state
                    GameMode = gameData.GameMode;
                    IsGameOver = gameData.IsGameOver;

                    // Restore players
                    if (GameMode == "HvH")
                    {
                        Player1 = new NotaktoHumanPlayer(gameData.Player1Name);
                        Player2 = new NotaktoHumanPlayer(gameData.Player2Name);
                    }
                    else
                    {
                        Player1 = new NotaktoHumanPlayer(gameData.Player1Name);
                        Player2 = new NotaktoComputerPlayer();
                    }

                    CurrentPlayer = gameData.CurrentPlayerName == Player1.Name ? Player1 : Player2;

                    // Restore board state
                    List<char[,]> boards = new List<char[,]>();
                    foreach (var board in gameData.Boards)
                    {
                        boards.Add(NotaktoGameData.ConvertJaggedToCharArray(board));
                    }
                    
                    var boardStateToRestore = Tuple.Create(boards, gameData.DeadBoards);
                    Board.SetBoardState(boardStateToRestore);

                    Console.WriteLine($"\nGame loaded successfully from {filename}");
                    return true;
                }
                else
                {
                    Console.WriteLine("\nSave file not found. Please check the filename and try again.");
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError loading game: {e.Message}");
                return false;
            }
        }

        protected override void DisplayRules()
        {
            Console.WriteLine("\n============================================ Notakto Rules ============================================");
            Console.WriteLine("\nNotakto is played on three 3x3 boards.");
            Console.WriteLine("Both players use the same symbol: X");
            Console.WriteLine("\nPlayers take turns placing X on any empty cell on any live board.");
            Console.WriteLine("When a board gets three X's in a row (horizontally, vertically, or diagonally), it becomes 'dead'.");
            Console.WriteLine("The player who is forced to complete the three-in-a-row on the LAST live board loses!");
            Console.WriteLine("\nStrategy tip: Try to avoid making moves that will force you to complete the final board.");
        }

        protected override void DisplayCommands()
        {
            Console.WriteLine("\n============================================ Notakto Commands ============================================");
            Console.WriteLine("\nDuring your turn, you can choose from the following options:");
            Console.WriteLine("1. Make a move - Place an X on one of the live boards");
            Console.WriteLine("2. Undo previous moves - Revert to an earlier state of the game");
            Console.WriteLine("3. Save the game - Save the current game state");
            Console.WriteLine("4. View help menu - Display game rules and commands");
            Console.WriteLine("5. Quit the game - Exit the application");
            
            Console.WriteLine("\nWhen making a move:");
            Console.WriteLine("1. First select which board (1-3) you want to play on");
            Console.WriteLine("2. Then select the position (1-9) where you want to place X");
            Console.WriteLine("3. You can confirm your move or redo it before ending your turn");
            
            Console.WriteLine("\nRemember: Avoid being the player who completes the last board!");
        }
    }
}
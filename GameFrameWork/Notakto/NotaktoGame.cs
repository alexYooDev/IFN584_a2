// handle game loop logic
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GameFrameWork
{
    public class NotaktoGame : AbstractGame
    {
        private NotaktoBoard notaktoBoard = new NotaktoBoard();
        private Move TempMove = null!;
        private int UndoneMovesCount = 0; // Track number of undone moves

        public override void ConfigureGame()
        {
            Board = notaktoBoard;
            SelectGameMode();
            IsGameOver = false;
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
                    Player2 = new NotaktoComputerPlayer("Computer");
                    break;
            }
            CurrentPlayer = Player1;
        }

        public override void StartGame()
        {
            Console.WriteLine("\n============================================ Game Started!  ============================================");
            IsGameOver = false;
            IsPlayerQuit = false;

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
                    if (IsPlayerQuit) break;
                }
                else
                {
                    ProcessComputerTurn();
                }

                if (!IsPlayerQuit)
                {
                    IsGameOver = CheckGameOver();
                    if (!IsGameOver) SwithCurrentPlayer();
                }
            }

            if (!IsPlayerQuit)
            {
                DisplayGameStatus();
                AnnounceWinner();
            }
        }

        protected override void MakeHumanMove()
        {
            var pos = notaktoBoard.SelectPosition();
            int board = pos[0], row = pos[1], col = pos[2];
            object previous = notaktoBoard.GetBoardState();

            if (notaktoBoard.IsValidMove(row, col, null, board))
            {
                notaktoBoard.MakeMove(row, col, null, board);
                TempMove = new Move(board, row, col, CurrentPlayer, null, previous);
                notaktoBoard.DisplayBoard(board);
                HandleMoveConfirmation();
            }
            else
            {
                Console.WriteLine("Invalid move. Try again.");
                MakeHumanMove();
            }
        }

        private void HandleMoveConfirmation()
        {
            while (true)
            {
                Console.WriteLine("\n1. Redo move\n2. Confirm move\nEnter your choice >> ");
                string input = Console.ReadLine();
                if (input == "1")
                {
                    Board.SetBoardState(TempMove.PreviousBoardState);
                    MakeHumanMove();
                    return;
                }
                else if (input == "2")
                {
                    MoveHistory.Push(TempMove);
                    UndoneMovesCount = 0;
                    RedoHistory.Clear();
                    return;
                }
                else
                    Console.WriteLine("Invalid input. Try again.");
            }
        }

        protected override void ProcessComputerTurn()
        {
            Console.WriteLine("Computer is making a move...");
            NotaktoComputerPlayer cpu = (NotaktoComputerPlayer)CurrentPlayer;
            var move = cpu.GenerateMove(notaktoBoard);
            if (move != null)
            {
                object prev = notaktoBoard.GetBoardState();
                notaktoBoard.MakeMove(move.Row, move.Col, null, move.BoardIndex);
                MoveHistory.Push(new Move(move.BoardIndex, move.Row, move.Col, CurrentPlayer, null, prev));
                RedoHistory.Clear();
                Console.WriteLine($"Computer placed X at board {move.BoardIndex + 1}, position ({move.Row + 1}, {move.Col + 1})");
            }
            else
            {
                Console.WriteLine("No valid move available. Skipping.");
            }
        }

        protected override void HandleQuitRequest()
        {
            IsGameOver = true;
            IsPlayerQuit = true;
        }

        public override bool CheckGameOver() => notaktoBoard.AreAllBoardsDead();

        public override void DisplayGameStatus()
        {
            Console.WriteLine($"\nCurrent Turn: {CurrentPlayer.Name}");
            notaktoBoard.DisplayAllBoards();
        }

        protected override void AnnounceWinner()
        {
            Console.WriteLine("\nGame over!");
            Console.WriteLine($"Winner: {(CurrentPlayer == Player1 ? Player2.Name : Player1.Name)}");
        }





        protected override void DisplayRules()
        {
            Console.WriteLine("\n============================================ Game rules  ============================================");
            Console.WriteLine("\nThe game is played on three boards with grids of size 3x3.");
            Console.WriteLine("\nBoth players play 'X'");
            Console.WriteLine("Two players alternate playing 'X' in any free cell on any live board.");
            Console.WriteLine("Once a board has three-in-a row, it is dead and remove from the game.");
            Console.WriteLine("The player that is forced to complete three-in-a-row on the last live board is the loser.");
            Console.WriteLine("\nThe game can be played in two modes: HvH (Human vs Human) or HvC (Human vs Computer).");
            Console.WriteLine("HvH: Human vs Human, HvC: Human vs Computer");
        }


        protected override void DisplayCommands()
        {
            Console.WriteLine("\n============================================ How to Play ============================================");
            Console.WriteLine("\n<<Players can proceed with the game simply by selecting option numbers provided in the terminal>>");
            Console.WriteLine("\n[Example]");
            Console.WriteLine("\nSelect your option for this turn:\n");
            Console.WriteLine("1. Make a move");
            Console.WriteLine("2. Save the game");
            Console.WriteLine("3. View help menu");
            Console.WriteLine("Enter your choice >> 1");
            Console.WriteLine("\n<<Player can view the current board status at each turn as shown below>>");
            Console.WriteLine("\n[Example]");
            Board.GetBoardStatus();
            Console.WriteLine("\n<<Player can select a number on the grid position to put the 'X' as shown below.>>");
            Console.WriteLine("\n[Example]");
            Board.DisplayGrid();
            Console.WriteLine("\nFor 'Enter the filename to save the game >>':");
            Console.WriteLine("[filename]: Save the current state of game in a file");
            Console.WriteLine("Example) game1");
            Console.WriteLine("\nFor 'Enter the filename to load the game >>':");
            Console.WriteLine("[filename]: Load a previously saved game");
            Console.WriteLine("Example) game1");
        }


        protected override void OfferUndoAfterLoad()
        {
            int maxUndo = GetUndoableMoveCountForPlayer(CurrentPlayer);

            if (maxUndo > 0)
            {
                Console.WriteLine($"\nYou have {maxUndo} move(s) that can be undone.");
                Console.WriteLine("Would you like to undo any moves? (y/n)");
                string response = Console.ReadLine().ToLower();

                if (response == "y" || response == "yes")
                {
                    Console.Write($"How many moves to undo (up to {maxUndo})? ");
                    if (int.TryParse(Console.ReadLine(), out int undoCount) && undoCount > 0 && undoCount <= maxUndo)
                    {
                        // Use the base class method which correctly filters by player
                        UndoPlayerMoves(CurrentPlayer, undoCount);
                        UndoneMovesCount = undoCount;

                        // If it's now a computer's turn after undoing, let it play
                        if (CurrentPlayer.Type == PlayerType.Computer)
                        {
                            DisplayGameStatus();
                            ProcessComputerTurn();

                            // Check if the game is over after computer move
                            IsGameOver = CheckGameOver();
                            if (!IsGameOver)
                            {
                                SwithCurrentPlayer();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Invalid input. No moves will be undone.");
                    }
                }
            }
        }

        // Load operation : load the game file and apply details to the game 
        public override bool LoadGame(string filename)
        {
            // Load the game state from a file
            Console.WriteLine("\n|| +++ Load the game +++ ||");
            Console.Write("Enter the filename to load the game >> ");
            string filename = Console.ReadLine();

            try
            {
                string saveDirectory = Path.Combine(Directory.GetCurrentDirectory(), "saveData");
                string saveFilePath = Path.Combine(saveDirectory, filename + ".json");

                if (File.Exists(saveFilePath))
                {
                    string jsonString = File.ReadAllText(saveFilePath);
                    GameData loadedGameData = JsonSerializer.Deserialize<GameData>(jsonString);

                    // Feed loaded game data to the Game's fields
                    NotaktoBoard = new NotaktoBoard(loadedGameData);

                    // Convert BoardState from JSON compatible board form int[][] to int[,] 
                    List<int[][]> boardState = new List<string[][]>();
                    List<int[,]> boards = notaktoBoard.GetBoards();

                    for (int b = 0; b < boards.Count; b++)
                    {
                        int[,] board = boards[b];
                        string[][] stringBoard = new string[3][];
                        for (int i = 0; i < boards.Count; i++)
                        {
                            stringBoard[i] = new string[3];
                            for (int j = 0; j < boards.Count; ++j)
                            {
                                Board[BoardIndex][i][j] = board[b][i, j].ToString();
                            }
                        }
                        Board.SetBoardData(boardState);

                        Mode = loadedGameData.GameMode;
                        Turn = loadedGameData.CurrentTurn;
                        Player1 = new HumanPlayer(loadedGameData.CurrentPlayer1Name, "Human", loadedGameData.RemainingOdds);
                        Player2 = loadedGameData.CurrentPlayer2Name == "Computer" ? new ComputerPlayer("Computer", loadedGameData.RemainingEvens) : new HumanPlayer(loadedGameData.CurrentPlayer2Name, "Human", loadedGameData.RemainingEvens);
                        DeadBoards = loadedGameData.DeadBoards;
                        Boards = loadedGameData.boardState;

                        Console.WriteLine("\nLoading a saved game...");
                        Console.WriteLine($"\nGame loaded successfully : {filename}");
                        Console.WriteLine("\n============================================ Loaded Game  ============================================");
                        StartGameLoop();
                    }
                }
                else
                {
                    Console.WriteLine("The specified file does not exist. Please try again.");
                    LoadGame();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading the game: {e.Message}");
            }
        }


        // Override for applying undo state
        protected override void ApplyUndoState(Move move)
        {
            // Restore the board state
            Board.SetBoardState(move.PreviousBoardState);



            Console.WriteLine($"\nMove undone for {move.Player.Name}");
        }

        protected override void ApplyRedoState(Move move)
        {
            // Restore the board state to the previous state before the move
            Board.SetBoardState(move.PreviousBoardState);

            Console.WriteLine($"\nMove redone. Current player: {CurrentPlayer.Name}");
        }


        protected override void SwithCurrentPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
        }

        // Save game : Serialize necessary game information -> save board, boardState, move history to JSON supported form
        public override void SaveGame(string filename)
        {
            // Create a GameData object to hold the game state
            try
            {
                string saveDirectory = Path.Combine(Directory.GetCurrentDirectory(), "saveData");

                // Create the save directory if it doesn't exist
                if (!Directory.Exists(saveDirectory))
                {
                    Directory.CreateDirectory(saveDirectory);
                }

                // Save the game state to a file
                Console.WriteLine("\n|| +++ Save the game +++ ||");
                Console.Write("Enter the filename to save the game >> ");

                string filename = Console.ReadLine();

                // Convert int[,] Board to int[][] => to save the board 2D Array in JSON format
                List<string[][]> boardState = new List<string[][]>();
                List<int[,]> boards = notaktoBoard.GetBoards();
                for (int b = 0; b < boards.Count; b++)
                {
                    int[,] board = boards[b];
                    string[][] stringBoard = new string[3][];
                    for (int i = 0; i < 3; i++)
                    {
                        stringBoard[i] = new string[3];
                        for (int j = 0; j < 3; j++)
                        {
                            stringBoard[i][j] = board[i, j].ToString();
                        }
                    }
                    boardState.Add(stringBoard);
                }

                boardState.Add(stringBoard);

                List<int> deadBoards = notaktoBoard.GetDeadBoards();

                GameData SaveGameData = new GameData
                {
                    GameMode = Mode,
                    BoardIndex,
                    BoardState = boardData,
                    CurrentPlayer1Name = Player1.GetName(),
                    CurrentPlayer2Name = Player2.GetName(),
                    CurrentTurn = Turn,
                    DeadBoards = deadBoards,
                    Boards = boardState
                };

                string jsonString = JsonSerializer.Serialize(SaveGameData, new JsonSerializerOptions { WriteIndented = true });
                string saveFilePath = Path.Combine(saveDirectory, filename + ".json");

                File.WriteAllText(saveFilePath, jsonString);
                Console.WriteLine($"\nGame saved successfully : {filename}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e} | Something went wrong! Try again!");
                // reprompt the user to save the game
                SaveGame();
            }
            return;
        }
    }
}


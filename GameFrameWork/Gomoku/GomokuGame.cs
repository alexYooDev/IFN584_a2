using System.Reflection.Metadata;
using System.Text.Json;

namespace GameFrameWork
{
    public class GomokuGame : AbstractGame
    {
        private GomokuBoard GomokuBoard;
        private List<(int, int)> winningLine;
        private Move TempMove; /* Temporary storage for move before confirmation */

        public GomokuGame() : base() { }

        public override void ConfigureGame()
        {
            int boardSize = SelectBoardSize();
            GomokuBoard = new GomokuBoard(boardSize, 1);
            SelectGameMode();
            Board = GomokuBoard;
        }

        public int SelectBoardSize()
        {
            Console.WriteLine("\n|| +++ Size of the Gomoku board +++ ||");
            int boardSize = 15; /* by default */
            bool validInput = false;

            while (!validInput)
            {
                Console.Write("\nSelect the size of the board (15 by default, minimum 5) >> ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out boardSize) && boardSize >= 5)
                {
                    validInput = true;
                }
                else
                {
                    Console.WriteLine("\nInvalid board size. Please enter a number greater than or equal to 5.");
                }
            }
            return boardSize;
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
                    Console.Write("\nEnter player 1 name (X) >> ");
                    string player1Name = Console.ReadLine();
                    Player1 = new GomokuHumanPlayer(player1Name, 'X');

                    Console.Write("\nEnter player 2 name (0) >> ");
                    string player2Name = Console.ReadLine();
                    Player2 = new GomokuHumanPlayer(player2Name, 'O');
                    break;
                case "HvC":
                    Console.Write("\nEnter your name (X) >> ");
                    string playerName = Console.ReadLine();
                    Player1 = new GomokuHumanPlayer(playerName, 'X');
                    Player2 = new GomokuComputerPlayer("Computer", 'O');
                    break;
            }

            CurrentPlayer = Player1;
        }

        protected override void ProcessHumanTurn()
        {
            bool turnComplete = false;
            while (!turnComplete)
            {
                DisplayTurnOptions();
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        MakeHumanMove();
                        turnComplete = true;
                        break;
                    case "2":
                        HandleUndoRequest();
                        break;
                    case "3":
                        HandleSaveRequest();
                        break;
                    case "4":
                        DisplayHelpMenu();
                        break;
                    case "5":
                        HandleQuitRequest();
                        turnComplete = true; // End the turn
                        break;
                    default:
                        Console.WriteLine("\nInvalid choice. Please try again.");
                        break;
                }
            }
        }

        protected override void MakeHumanMove()
        {
            GomokuHumanPlayer humanPlayer = (GomokuHumanPlayer)CurrentPlayer;

            char symbol = (char)humanPlayer.SelectMove(Board);

            int[] position = GomokuBoard.SelectPosition();

            object previousState = Board.GetBoardState();

            if (Board.IsValidMove(position[0], position[1], symbol, 0, true))
            {
                Board.MakeMove(position[0], position[1], symbol);

                TempMove = new Move(0, position[0], position[1], CurrentPlayer, symbol, previousState);

                Board.DisplayBoard();

                HandleMoveConfirmation();
            }
            else
            {
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
                    // Redo the move - restore the board state
                    Board.SetBoardState(TempMove.PreviousBoardState);

                    // Let the player make a new move
                    MakeHumanMove();
                    return;
                }
                else if (input == "2")
                {
                    // Confirm the move - add it to move history
                    MoveHistory.Push(TempMove);

                    // Clear redo history when a new move is confirmed
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
            GomokuComputerPlayer computerPlayer = (GomokuComputerPlayer)CurrentPlayer;


            /* First, make computer try to find a winning move */
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
            // save the board state for undo
            object previousState = Board.GetBoardState();

            char symbol = (char)CurrentPlayer.MoveSymbol;
            Board.MakeMove(row, col, symbol);

            /* Add to move history */
            Move move = new Move(0, row, col, CurrentPlayer, symbol, previousState);
            MoveHistory.Push(move);

            // Clear redo history

            ClearRedoStackOnNewMove();

            Console.WriteLine($"\nComputer placed {symbol} at position ({row}, {col})");
        }

        public override bool CheckGameOver()
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

            // check for draw situation
            return Board.IsBoardFull();
        }

        protected override void AnnounceWinner()
        {
            if (winningLine != null)
            {
                Console.WriteLine($"\nGame over! {CurrentPlayer.Name} wins!");
                Console.WriteLine($"Winning line: {string.Join(" -> ", winningLine.Select(pos => $"({pos.Item1},{pos.Item2})"))}");
            }
            else if (Board.IsBoardFull())
            {
                Console.WriteLine("\nGame over! It's a draw!");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        public override void DisplayGameStatus()
        {
            Console.WriteLine($"\nCurrent Turn: {CurrentPlayer.Name} ({CurrentPlayer.MoveSymbol})");
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
            Board.MakeMove(move.Row, move.Col, move.MoveData);
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

                char[,] boardArray = (char[,])Board.GetBoardState();
                string[][] boardJagged = GomokuGameData.ConvertTo2DJaggedArray(boardArray);

                List<GomokuMovesToSerialize> serializedMoveHistory = new List<GomokuMovesToSerialize>();
                foreach (Move move in MoveHistory)
                {
                    char[,] previousBoard = (char[,])move.PreviousBoardState;
                    serializedMoveHistory.Add(new GomokuMovesToSerialize
                    {
                        BoardIndex = move.BoardIndex,
                        Row = move.Row,
                        Col = move.Col,
                        PlayerName = move.Player.Name,
                        MoveData = (char)move.MoveData,
                        PreviousBoardState = GomokuGameData.ConvertTo2DJaggedArray(previousBoard)
                    });
                }

                List<GomokuMovesToSerialize> serializedRedoHistory = new List<GomokuMovesToSerialize>();
                foreach (Move move in RedoHistory)
                {
                    char[,] previousBoard = (char[,])move.PreviousBoardState;
                    serializedRedoHistory.Add(new GomokuMovesToSerialize
                    {
                        BoardIndex = move.BoardIndex,
                        Row = move.Row,
                        Col = move.Col,
                        PlayerName = move.Player.Name,
                        MoveData = (char)move.MoveData,
                        PreviousBoardState = GomokuGameData.ConvertTo2DJaggedArray(previousBoard)
                    });
                }

                var gameData = new GomokuGameData
                {
                    BoardSize = Board.GetSize(),
                    GameMode = GameMode,
                    CurrentPlayerName = CurrentPlayer.Name,
                    Player1Name = Player1.Name,
                    Player2Name = Player2.Name,
                    GameType = "Gomoku",
                    IsGameOver = IsGameOver,
                    BoardState = boardJagged,
                    MoveHistory = serializedMoveHistory,
                    RedoHistory = serializedRedoHistory
                };

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
                    var gameData = JsonSerializer.Deserialize<GomokuGameData>(jsonString);

                    // Create board with loaded size
                    GomokuBoard = new GomokuBoard(gameData.BoardSize, 1);
                    Board = GomokuBoard;

                    // Convert string[][] to char[,] before restoring board state
                    char[,] boardArray = GomokuGameData.ConvertToArray2D(gameData.BoardState);

                    // Restore board state
                    Board.SetBoardState(boardArray);

                    // Restore game properties
                    GameMode = gameData.GameMode;
                    IsGameOver = gameData.IsGameOver;

                    // Restore players
                    if (GameMode == "HvH")
                    {
                        Player1 = new GomokuHumanPlayer(gameData.Player1Name, 'X');
                        Player2 = new GomokuHumanPlayer(gameData.Player2Name, 'O');
                    }
                    else // HvC
                    {
                        Player1 = new GomokuHumanPlayer(gameData.Player1Name, 'X');
                        Player2 = new GomokuComputerPlayer("Computer", 'O');
                    }

                    // Set current player
                    CurrentPlayer = gameData.CurrentPlayerName == Player1.Name ? Player1 : Player2;

                    // Clear existing history 
                    MoveHistory.Clear();
                    RedoHistory.Clear();

                    // Restore move history
                    if (gameData.MoveHistory != null)
                    {
                        for (int i = gameData.MoveHistory.Count - 1; i >= 0; i--)
                        {
                            var serializedMove = gameData.MoveHistory[i];
                            AbstractPlayer player = serializedMove.PlayerName == Player1.Name ? Player1 : Player2;
                            Move move = new Move(
                                serializedMove.BoardIndex,
                                serializedMove.Row,
                                serializedMove.Col,
                                player,
                                serializedMove.MoveData,
                                GomokuGameData.ConvertToArray2D(serializedMove.PreviousBoardState)
                            );
                            MoveHistory.Push(move);
                        }
                    }

                    // Restore redo history
                    if (gameData.RedoHistory != null)
                    {
                        for (int i = gameData.RedoHistory.Count - 1; i >= 0; i--)
                        {
                            var serializedMove = gameData.RedoHistory[i];
                            AbstractPlayer player = serializedMove.PlayerName == Player1.Name ? Player1 : Player2;
                            Move move = new Move(
                                serializedMove.BoardIndex,
                                serializedMove.Row,
                                serializedMove.Col,
                                player,
                                serializedMove.MoveData,
                                GomokuGameData.ConvertToArray2D(serializedMove.PreviousBoardState)
                            );
                            RedoHistory.Push(move);
                        }
                    }

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
            Console.WriteLine("\n============================================ Gomoku Rules ============================================");
            Console.WriteLine("\nThe game is played on a grid of size NxN (typically 15x15).");
            Console.WriteLine("Player 1 uses X stones, Player 2 uses O stones.");
            Console.WriteLine("\nThe goal is to be the first to get exactly 5 stones in a row.");
            Console.WriteLine("A winning line can be horizontal, vertical, or diagonal.");
            Console.WriteLine("\nThe game can be played in two modes: HvH (Human vs Human) or HvC (Human vs Computer).");
            Console.WriteLine("\nPlayers take turns placing one stone at a time on empty intersections.");
            Console.WriteLine("The game ends when one player gets 5 in a row or the board is full (draw).");
        }

        protected override void DisplayCommands()
        {
            Console.WriteLine("\n============================================ Game Commands ============================================");
            Console.WriteLine("\nDuring your turn, you can choose from the following options:");
            Console.WriteLine("1. Make a move - Place a stone on the board");
            Console.WriteLine("2. Undo previous moves - Revert to an earlier state of the game");
            Console.WriteLine("3. Save the game - Save the current game state");
            Console.WriteLine("4. View help menu - Display game rules and commands");
            Console.WriteLine("5. Quit the game - Exit the application");
            
            Console.WriteLine("\nWhen making a move:");
            Console.WriteLine("1. Enter the row coordinate (0 to board size - 1)");
            Console.WriteLine("2. Enter the column coordinate (0 to board size - 1)");
            Console.WriteLine("3. You can either confirm your move or redo it before ending your turn");
            
            Console.WriteLine("\nSaving and loading games:");
            Console.WriteLine("- When saving, enter a filename without an extension");
            Console.WriteLine("- When loading, enter the same filename you used to save");
            Console.WriteLine("- After loading a game, you'll have the option to undo moves");
        }
    }
}
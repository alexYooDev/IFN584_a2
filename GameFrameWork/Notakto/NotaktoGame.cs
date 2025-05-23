// handle game loop logic
using System;
using System.Collections.Generic;
using System.Text.Json;
using GameFrameWork;

namespace Notakto
{
    public class NotaktoGame : AbstractGame
    {
        private NotaktoBoard Board;
        private bool IsPlayerQuit = false;
        private Move TempMove; // Temporary storage for move before confirmation
        private int UndoneMovesCount = 0; // Track number of undone moves



        public override void ConfigureGame()
        {
            Board = new NotaktoBoard();
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
                    Player2 = new NotaktoHumanPlayer();
                    break;
            }
            CurrentPlayer = Player1;
        }

        public override void StartGame()
        {
            Console.WriteLine("\n============================================ Game Started!  ============================================");
            IsGameOver = false;
            IsPlayerQuit = false; // Reset the quit flag when starting

            // Offer undo after loading a game -> MoveHistory > 0 means it is a loaded game
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
                        SwithCurrentPlayer();
                    }
                }
            }

            // Only announce winner and display result if player didn't quit
            if (!IsPlayerQuit)
            {
                DisplayGameStatus();
                AnnounceWinner();
            }
        }

        protected void HandleUndoRequest()
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
                        SwithCurrentPlayer();
                        Console.WriteLine($"Turn switched to {CurrentPlayer.Name}");
                    }

                    Board.DisplayBoard(); // Show the board after undo
                }
                else
                {
                    Console.WriteLine($"Invalid input. You can undo up to {maxUndo} of your move(s).");
                }
            }
            else
            {
                Console.WriteLine("No moves to undo.");
            }
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

        public override void Play()
        {
            ConfigureGame();
            ConfigurePlayer();
            StartGame();
        }

        // Display the options for the current turn
        /*private void DisplayOptions()
        {
            Console.WriteLine("\n|| +++ Options +++ ||");
            Console.WriteLine("\nSelect your option for this turn:\n");
            Console.WriteLine("1. Make a move");
            Console.WriteLine("2. Save the game");
            Console.WriteLine("3. View help menu");
            Console.WriteLine("4. Quit the game");
            Console.Write("\nEnter your choice >> ");
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    // Make a move
                    if (Mode == "HvC" && Turn)
                    {
                        MakePlayerMove(Player1);
                    }
                    else if (Mode == "HvC" && !Turn)
                    {
                        MakeComputerMove();
                    }
                    else if (Mode == "HvH" && Turn)
                    {
                        MakePlayerMove(Player1);
                    }
                    else if (Mode == "HvH" && !Turn)
                    {
                        MakePlayerMove(Player2);
                    }
                    break;
                case "2":
                    SaveGame();
                    Board.GetBoardStatus();
                    DisplayOptions();
                    break;
                case "3":
                    DisplayHelpMenu();
                    Board.GetBoardStatus();
                    DisplayOptions();
                    break;
                case "4":
                    Console.WriteLine("\nExiting the game...");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("\nInvalid choice. Please try again.");
                    DisplayOptions();
                    break;
            }
        }
        */

        // Make a move for the human player
        protected override void MakeHumanMove()
        {
            NotaktoHumanPlayer humanPlayer = (NotaktoHumanPlayer)CurrentPlayer;


            /*
            Console.Write("Select board (1-3): ");
            int boardIndex = int.Parse(Console.ReadLine()) - 1;
            */

            // Get player's selected board/position
            int[] position = Board.SelectPosition();
            /*
            Console.Write("Select cell (1-9): ");
            int cellNum = int.Parse(Console.ReadLine());
            int row = (cellNum - 1) / 3;
            int col = (cellNum - 1) % 3;
            */

            // Save the current board state for potential undo/redo
            object previousState = Board.GetBoardState();


            // Validate and place the move on the board
            if (Board.IsValidMove(position[0], position[1], null, selectedBoard, true))
            {
                Board.MakeMove(row, col, null, selectedBoard);

                TempMove = new Move(selectedBoard, position[0], position[1], CurrentPlayer, null, previousState);

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
                Console.WriteLine("1. Redo this move (place a different number or position)");
                Console.WriteLine("2. Confirm and end your turn");
                Console.Write("\nEnter your choice >> ");
                string input = Console.ReadLine();

                if (input == "1")
                {
                    // Redo the move - restore the board state and return the number
                    Board.SetBoardState(TempMove.PreviousBoardState);

                    // Let the player make a new move
                    MakeHumanMove();
                    return; // Exit this loop after making a new move
                }
                else if (input == "2")
                {
                    // Confirm the move - add it to move history
                    MoveHistory.Push(TempMove);

                    // When a new move is confirmed, we are no longer in an undone state
                    UndoneMovesCount = 0;

                    // Clear redo history when a new move is confirmed
                    RedoHistory.Clear();

                    return; // Exit the loop
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }
        }


        protected override void ProcessHumanTurn()
        {
            bool turnComplete = false;
            while (!turnComplete)
            {
                Console.WriteLine("\n|| +++ Options +++ ||");
                Console.WriteLine("\nSelect your option for this turn:\n");
                Console.WriteLine("1. Make a move");
                Console.WriteLine("2. Undo previous moves");
                Console.WriteLine("3. Save the game");
                Console.WriteLine("4. View help menu");
                Console.WriteLine("5. Quit the game");
                Console.Write("\nEnter your choice >> ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        MakeHumanMove();
                        turnComplete = true; // Turn is complete after move is confirmed
                        break;
                    case "2":
                        int maxUndo = GetUndoableMoveCountForPlayer(CurrentPlayer);
                        if (maxUndo > 0)
                        {
                            Console.Write($"How many moves to undo (up to {maxUndo})? ");
                            if (int.TryParse(Console.ReadLine(), out int undoCount) && undoCount > 0 && undoCount <= maxUndo)
                            {
                                // Use the base class method which correctly filters by player
                                UndoPlayerMoves(CurrentPlayer, undoCount);
                                UndoneMovesCount += undoCount;
                                Board.DisplayBoard(); // Show the board after undo
                            }
                            else
                            {
                                Console.WriteLine($"Invalid input. You can undo up to {maxUndo} of your move(s).");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No moves to undo.");
                        }
                        break;
                    case "3":
                        Console.Write("\nEnter filename to save >> ");
                        string saveFilename = Console.ReadLine();
                        SaveGame(saveFilename);
                        // Do not end turn, allow player to continue
                        break;
                    case "4":
                        DisplayHelpMenu();
                        Board.DisplayBoard();
                        // Do not end turn, allow player to continue
                        break;
                    case "5":
                        HandleQuitRequest();
                        return; // Immediately exit the method without setting turnComplete
                    default:
                        Console.WriteLine("\nInvalid choice. Please try again.");
                        break;
                }
            }
        }

        protected override void ProcessComputerTurn()
        {
            Console.WriteLine("\nComputer is making a move...");
            NotaktoComputerPlayer computerPlayer = (NotaktoComputerPlayer)CurrentPlayer;

            // First try to find a losing move using the abstract method
            object losingMove = computerPlayer.FindLosingMove(Board);

            for (int b = 0; b < BoardCount; ++i)
                if (losingMove != null)
                {
                    bool IsTaken = false;
                    // avoid these losing move
                    while (!IsTaken)
                    {
                        int number = (int)computerPlayer.SelectRandomMove();
                        if (number != [b][row, col])
                        {
                            MakeComputerMove(number);
                            IsTaken = true;
                        }
                        else
                            int number = (int)computerPlayer.SelectRandomMove();
                    }
                }
                else
                {
                    // No winning move, make a random move
                    int number = (int)computerPlayer.SelectRandomMove();
                    MakeComputerMove(number);
                }
        }


        private void MakeComputerMove()
        {
            NotaktoBoard notaktoBoard = (NotaktoBoard)Board;
            NotaktoComputerPlayer computerPlayer = (NotaktoComputerPlayer)CurrentPlayer;
            Random random = new Random();

            for (int b = 0; b < 3; b++)
            {
                if (notaktoBoard.DeadBoards.Contains(b))
                    continue;

                for (int row = 0; row < 3; ++row)
                {
                    for (int col = 0; col < 3; ++col)
                    {
                        if (notaktoBoard.IsValidMove(row, col, null, b))
                            continue;
                        bool isLosingMove = notaktoBoard.CheckThreeInARow(b);

                        if (IsLosingMove)
                        {
                            continue;
                        }
                        object previousState = Board.GetBoardState();
                        notaktoBoard.MakeMove(row, col, null, b);
                        // Add to move history
                        Move move = new Move(b, row, col, CurrentPlayer, null, previousState);
                        MoveHistory.Push(move);

                        // Reset undo/redo state
                        UndoneMovesCount = 0;
                        RedoHistory.Clear();

                        Console.WriteLine($"Computer placed X at board {b + 1}, position ({row + 1},{col + 1})");
                        return;
                    }
                }
            }
            // If only losing move available
            Console.WriteLine("No other moves, computer forced to make a losing move!");
            for (int b = 0; b < 3; ++b)
            {
                if (notaktoBoard.IsBoardDead(b))
                    continue;

                for (int i = 0; i < 3; ++i)
                {
                    for (int j = 0; j < 3; ++j)
                    {
                        if (notaktoBoard.IsValidMove(i, j, null, b))
                        {
                            object previousState = notaktoBoard.GetBoardState();
                            notaktoBoard.MakeMove(i, j, null, b);
                            MoveHistory.Push(new Move(b, i, j, Player2, null, previousState));
                            RedoHistory.Clear();
                            Console.WriteLine($"Computer placed X at board {b + 1}, position ({i + 1},{j + 1})");
                            return;
                        }
                    }
                }
            }
        }



        // if no winning move, pick random
        /*List<(int board, int row, int col)> validMoves = new();
        for (int b = 0; b < 3; ++b)
        {
            if (!notaktoBoard.DeadBoards.Contains(b))
                continue;

            for (int row = 0; row < 3; ++row)
            {
                for (int col = 0; col < 3; ++col)
                {
                    if (notaktoBoard.IsValidMove(row, col, null, b))
                        validMoves.Add((b, row, col));
                }
            }
        }


        if (validMoves.Count > 0)
        {
            var (b, row, col) = validMoves[random.Next(validMoves.Count)];
            object previousState = Board.GetBoardState();
            notaktoBoard.MakeMove(row, col, null, b);
            Move move = new Move(b, row, col, CurrentPlayer, null, previousState);
            MoveHistory.Push(move);
            ClearRedoStackOnNewMove();
            Console.WriteLine($"Computer randomly placed X at board {b + 1}, position ({row + 1},{col + 1})");
        }
        */


        private bool IsLosingMove(int boardIndex, int row, int col)
        {
            NotaktoBoard notaktoBoard = (NotaktoBoard)Board;
            return notaktoBoard.CheckThreeInARow(boardIndex);
        }

        protected override void HandleQuitRequest()
        {
            IsGameOver = true;
            IsPlayerQuit = true;
        }

        public override bool CheckGameOver()
        {
            return ((NotaktoBoard)Board).AreAllBoardsDead();
        }



        public override void DisplayGameStatus()
        {
            Console.WriteLine($"\nCurrent Turn: {CurrentPlayer.Name}");
            Board.DisplayBoard();
        }


        // Display the result of the game
        protected override void AnnounceWinner()
        {
            // Condition with a winner
            if (Board.AreAllBoardsDeadBoard())
            {
                Board.GetBoardStatus();
                Console.WriteLine($"\nGame over! {(!Turn ? Player1.GetName() : Player2.GetName())} wins!");
            }
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

    }
}


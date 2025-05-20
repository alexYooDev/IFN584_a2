using System.ComponentModel;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
namespace GameFrameWork
{
    public class TicTacToeGame : AbstractGame
    {
        private int TargetSum;
        private HashSet<int> OddNumbers;
        private HashSet<int> EvenNumbers;

        private TicTacToeBoard Board;

        public TicTacToeGame() : base()
        {
            OddNumbers = new HashSet<int>();
            EvenNumbers = new HashSet<int>();
        }

        public override void ConfigureGame()
        {

            int boardSize = SelectBoardSize();
            Board = new TicTacToeBoard(boardSize);

            // Initialize the number sets
            for (int i = 1; i <= boardSize * boardSize; ++i)
            {
                if (i % 2 == 1)
                    OddNumbers.Add(i);
                else
                    EvenNumbers.Add(i);
            }

            // Set the target sum for the game (magic square formula)
            TargetSum = boardSize * (boardSize * boardSize + 1) / 2;
        }

        public int SelectBoardSize()
        {
            Console.WriteLine("\n|| +++ Size of the board +++ ||");
            int boardSize = 0;
            bool validInput = false;

            while (!validInput)
            {
                Console.Write("\nSelect the size of the board (3 => 3X3/ 4 => 4X4/ 5 => 5X5/ etc.) >> ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out boardSize) && boardSize >= 3)
                {
                    validInput = true;
                }
                else
                {
                    Console.WriteLine("\nInvalid board size. Please enter a number greater than or equal to 3.");
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
            int mode = 0;
            bool validMode = false;

            while (!validMode)
            {
                Console.WriteLine("\n|| +++ Select the mode of the game +++ ||");
                Console.WriteLine("1. HvH (Human vs Human)");
                Console.WriteLine("2. HvC (Human vs Computer)");
                Console.Write("\nEnter your choice >> ");
                try
                {
                    string input = Console.ReadLine();
                    mode = Convert.ToInt32(input);

                    if (mode == 1 || mode == 2)
                    {
                        validMode = true;
                    }
                    else
                    {
                        Console.WriteLine("\nInvalid mode selected. Please enter 1 or 2.");
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("\nInvalid input. Please enter a number (1 or 2).");
                }
                catch (Exception)
                {
                    Console.WriteLine("\nUnexpected error occurred. Please try again.");
                }
            }

            switch (mode)
            {
                case 1:
                    GameMode = "HvH";
                    Console.Write("\nEnter player 1 name >> ");
                    string player1Name = Console.ReadLine();
                    Player1 = new TicTacToeHumanPlayer(player1Name, OddNumbers);

                    Console.Write("\nEnter player 2 name >> ");
                    string player2Name = Console.ReadLine();
                    Player2 = new TicTacToeHumanPlayer(player2Name, EvenNumbers);
                    break;
                case 2:
                    GameMode = "HvC";
                    Console.Write("\nEnter your name >> ");
                    string playerName = Console.ReadLine();
                    Player1 = new TicTacToeHumanPlayer(playerName, OddNumbers);
                    Player2 = new TicTacToeComputerPlayer(EvenNumbers);
                    break;
            }

            CurrentPlayer = Player1;
        }

        public override void StartGame()
        {
            Console.WriteLine("\n============================================ Game Started!  ============================================");
            IsGameOver = false;

            while (!IsGameOver)
            {
                DisplayGameStatus();

                if (CurrentPlayer.Type == PlayerType.Human)
                {
                    ProcessHumanTurn();
                }
                else
                {
                    ProcessComputerTurn();
                }

                IsGameOver = CheckGameOver();

                if (!IsGameOver)
                {
                    SwithCurrentPlayer();
                }
            }

            DisplayGameResult();
        }

        public override void Play()
        {
            ConfigureGame();
            ConfigurePlayer();
            StartGame();
        }

        private void MakeHumanMove()
        {
            TicTacToeHumanPlayer humanPlayer = (TicTacToeHumanPlayer)CurrentPlayer;

            // Get the player's number selection
            int number = humanPlayer.SelectNumber();

            // Get the player's position selection
            int[] position = Board.SelectPosition();

            // Save the current board state for undo
            object previousState = Board.GetBoardState();

            // Make the move
            if (Board.IsValidMove(position[0], position[1], number))
            {
                Board.MakeMove(position[0], position[1], number);

                // Record the move for undo/redo
                Move move = new Move(0, position[0], position[1], CurrentPlayer, number, previousState);
                MoveHistory.Push(move);

            }
            else
            {
                // If the move is invalid, let the player try again
                Console.WriteLine("Invalid move! Please try again.");
                humanPlayer.GetAvailableNumbers().Add(number); // Return the number to available set
                MakeHumanMove();
            }
        }

        private void ProcessHumanTurn()
        {
            bool turnComplete = false;
            while (!turnComplete)
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
                        MakeHumanMove();

                        Board.DisplayBoard();

                        // After making a move, offer undo/redo/confirm options before switching turn
                        while (true)
                        {
                            int maxUndo = GetUndoableMoveCountForPlayer(CurrentPlayer);
                            int maxRedo = GetRedoableMoveCountForPlayer(CurrentPlayer);

                            Console.WriteLine("\nWhat would you like to do next?");
                            Console.WriteLine("1. Undo your move");
                            Console.WriteLine("2. Redo a move");
                            Console.WriteLine("3. Confirm and end your turn");
                            Console.Write("\nEnter your choice >> ");
                            string postMoveInput = Console.ReadLine();

                            if (postMoveInput == "1")
                            {
                                if (maxUndo > 0)
                                {
                                    Console.Write($"How many moves to undo (1/{maxUndo})? ");
                                    if (int.TryParse(Console.ReadLine(), out int undoCount) && undoCount > 0 && undoCount <= maxUndo)
                                    {
                                        UndoPlayerMoves(CurrentPlayer, undoCount);
                                        // After undo, allow the player to make a move again
                                        MakeHumanMove();
                                        continue;
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
                            else if (postMoveInput == "2")
                            {
                                if (maxRedo > 0)
                                {
                                    Console.Write($"How many moves to redo (1/{maxRedo})? ");
                                    if (int.TryParse(Console.ReadLine(), out int redoCount) && redoCount > 0 && redoCount <= maxRedo)
                                    {
                                        RedoPlayerMoves(CurrentPlayer, redoCount);
                                        // After redo, allow the player to undo/redo/confirm again
                                        continue;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Invalid input. You can redo up to {maxRedo} of your move(s).");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No moves to redo.");
                                }
                            }
                            else if (postMoveInput == "3")
                            {
                                // Confirm and end turn
                                turnComplete = true;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid choice. Please try again.");
                            }
                        }
                        break;
                    case "2":
                        Console.Write("\nEnter filename to save: ");
                        string saveFilename = Console.ReadLine();
                        SaveGame(saveFilename);
                        // Do not end turn, allow player to continue
                        break;
                    case "3":
                        DisplayHelpMenu();
                        Board.DisplayBoard();
                        // Do not end turn, allow player to continue
                        break;
                    case "4":
                        Console.WriteLine("\nExiting the game...");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("\nInvalid choice. Please try again.");
                        break;
                }
            }
        }

        private void ProcessComputerTurn()
        {
            Console.WriteLine("\nComputer is making a move...");
            MakeComputerMove();
        }

        private void MakeComputerMove()
        {
            TicTacToeComputerPlayer computerPlayer = (TicTacToeComputerPlayer)CurrentPlayer;
            int boardSize = Board.GetSize();

            // Try to find a winning move first
            foreach (int number in computerPlayer.GetAvailableNumbers().ToList())
            {
                for (int row = 0; row < boardSize; row++)
                {
                    for (int col = 0; col < boardSize; col++)
                    {
                        if (Board.IsValidMove(row, col, number))
                        {
                            // Save the current board state for undo
                            object previousState = Board.GetBoardState();

                            // Try the move
                            Board.MakeMove(row, col, number);

                            // Check if this is a winning move
                            if (IsWinningMove(row, col))
                            {
                                // Create a Move object and add it to history
                                Move move = new Move(0, row, col, CurrentPlayer, number, previousState);
                                MoveHistory.Push(move);

                                // Clear redo history when a new move is made
                                ClearRedoStackOnNewMove();

                                Console.WriteLine($"\nComputer placed {number} at position ({row + 1}, {col + 1})");
                                return;
                            }

                            // Undo the move if it's not a winning move
                            Board.SetBoardState(previousState);
                        }
                    }
                }
            }

            // If no winning move, make a random move
            Random random = new Random();
            List<(int, int)> emptyPositions = new List<(int, int)>();

            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (Board.IsValidMove(row, col, null))
                    {
                        emptyPositions.Add((row, col));
                    }
                }
            }

            if (emptyPositions.Count > 0 && computerPlayer.GetAvailableNumbers().Count > 0)
            {
                // Select a random position and number
                int randomPositionIndex = random.Next(emptyPositions.Count);
                (int row, int col) = emptyPositions[randomPositionIndex];

                int number = computerPlayer.SelectRandomNumber();

                // Save the current board state for undo
                object previousState = Board.GetBoardState();

                // Make the move
                Board.MakeMove(row, col, number);

                // Create a Move object and add it to history
                Move move = new Move(0, row, col, CurrentPlayer, number, previousState);
                MoveHistory.Push(move);

                // Clear redo history when a new move is made
                ClearRedoStackOnNewMove();

                Console.WriteLine($"\nComputer placed {number} at position ({row + 1}, {col + 1})");
            }
        }

        public override bool CheckGameOver()
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

        public override void DisplayGameStatus()
        {
            Console.WriteLine($"\nCurrent Turn: {CurrentPlayer.Name}");
            Console.WriteLine($"Target Sum: {TargetSum}");
            Board.DisplayBoard();
        }
        private void DisplayGameResult()
        {
            Board.DisplayBoard();

            if (Board.IsBoardFull() && !CheckWinningLine())
            {
                Console.WriteLine("\nGame over! It's a draw!");
            }
            else
            {
                Console.WriteLine($"\nGame over! {CurrentPlayer.Name} wins!");
            }
        }

        public override void SaveGame(string filename)
        {
            try
            {
                string saveDirectory = Path.Combine(Directory.GetCurrentDirectory(), "saveData");

                // Create directory if it doesn't exist
                if (!Directory.Exists(saveDirectory))
                {
                    Directory.CreateDirectory(saveDirectory);
                }

                // Create a class to hold game data
                var gameData = new TicTacToeGameData
                {
                    BoardSize = Board.GetSize(),
                    GameMode = GameMode,
                    CurrentPlayer = CurrentPlayer,
                    Player1 = Player1,
                    Player2 = Player2,
                    GameType = "NumericalTicTacToe",
                    IsGameOver = IsGameOver,
                    BoardState = Board.GetBoardState()
                };

                string jsonString = System.Text.Json.JsonSerializer.Serialize(gameData);
                string saveFilePath = Path.Combine(saveDirectory, filename + ".json");

                File.WriteAllText(saveFilePath, jsonString);
                Console.WriteLine($"\nGame saved successfully as {filename}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError saving game: {e.Message}");
            }
        }

        public override void LoadGame(string filename)
        {
            try
            {
                string saveDirectory = Path.Combine(Directory.GetCurrentDirectory(), "saveData");
                string saveFilePath = Path.Combine(saveDirectory, filename + ".json");

                if (File.Exists(saveFilePath))
                {
                    string jsonString = File.ReadAllText(saveFilePath);
                    var gameData = JsonSerializer.Deserialize<TicTacToeGameData>(jsonString);

                    // Create board with loaded size
                    Board = new TicTacToeBoard(gameData.BoardSize);

                    // Restore board state
                    Board.SetBoardState(gameData.BoardState);

                    // Restore game properties
                    GameMode = gameData.GameMode;
                    IsGameOver = gameData.IsGameOver;
                    TargetSum = gameData.TargetSum;

                    // Restore players
                    if (GameMode == "HvH")
                    {
                        Player1 = new TicTacToeHumanPlayer(
                            gameData.Player1.Name, gameData.Player1Numbers);

                        Player2 = new TicTacToeHumanPlayer(
                            gameData.Player2.Name, gameData.Player2Numbers);
                    }
                    else // HvC
                    {
                        Player1 = new TicTacToeHumanPlayer(
                            gameData.Player1.Name, gameData.Player1Numbers);

                        Player2 = new TicTacToeComputerPlayer(gameData.Player2Numbers);
                    }

                    // Set current player
                    CurrentPlayer = gameData.CurrentPlayer.Name == Player1.Name ? Player1 : Player2;

                    Console.WriteLine($"\nGame loaded successfully from {filename}");
                }
                else
                {
                    Console.WriteLine("\nSave file not found. Please check the filename and try again.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError loading game: {e.Message}");
            }
        }

        protected override void ApplyUndoState(Move move)
        {
            // Restore the board state
            Board.SetBoardState(move.PreviousBoardState);

            // Return the number to the player's available numbers
            int number = (int)move.MoveData;

            if (move.Player is TicTacToeHumanPlayer humanPlayer)
            {
                humanPlayer.GetAvailableNumbers().Add(number);
            }
            else if (move.Player is TicTacToeComputerPlayer computerPlayer)
            {
                computerPlayer.GetAvailableNumbers().Add(number);
            }

            Console.WriteLine($"\nMove undone. Current player: {CurrentPlayer.Name}");
        }

        protected override void ApplyRedoState(Move move)
        {
            // Make the move again
            Board.MakeMove(move.Row, move.Col, move.MoveData);

            // Remove the number from the player's available numbers
            int number = (int)move.MoveData;

            if (move.Player is TicTacToeHumanPlayer humanPlayer)
            {
                humanPlayer.GetAvailableNumbers().Remove(number);
            }
            else if (move.Player is TicTacToeComputerPlayer computerPlayer)
            {
                computerPlayer.GetAvailableNumbers().Remove(number);
            }

            Console.WriteLine($"\nMove redone. Current player: {CurrentPlayer.Name}");
        }

        protected override void SwithCurrentPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
        }

        protected override void DisplayRules()
        {
            Console.WriteLine("\n============================================ Game rules  ============================================");
            Console.WriteLine("\nThe game is played on a grid of size NxN, where N is the size of the board.");
            Console.WriteLine("Player 1 uses odd numbers (1, 3, 5, ...)");
            Console.WriteLine("Player 2 uses even numbers (2, 4, 6, ...)");
            Console.WriteLine("\nThe goal of the game is to get a sum of (N * (N^2 + 1)) / 2 in any row, column, or diagonal.");
            Console.WriteLine("The game can be played in two modes: HvH (Human vs Human) or HvC (Human vs Computer).");
            Console.WriteLine("\nThe game is played in turns, where each player selects a number from their set of numbers.");
            Console.WriteLine("The player can place the number on the board in an empty cell.");
            Console.WriteLine("The game ends when one player reaches the target sum or when the board is full.");
            Console.WriteLine("If the board is full and no player has reached the target sum, the game is a draw.");
        }
        
        protected override void DisplayCommands()
        {
            Console.WriteLine("\n============================================ Game Commands ============================================");
            Console.WriteLine("\nDuring your turn, you can choose from the following options:");
            Console.WriteLine("1. Make a move - Place a number on the board");
            Console.WriteLine("2. Save the game - Save the current game state");
            Console.WriteLine("3. View help menu - Display game rules and commands");
            Console.WriteLine("4. Undo last move - Revert the previous move");
            Console.WriteLine("5. Redo last move - Redo a previously undone move");
            Console.WriteLine("6. Quit the game - Exit the application");
            
            Console.WriteLine("\nWhen making a move:");
            Console.WriteLine("1. First select a number from your available set");
            Console.WriteLine("2. Then select a position on the board to place the number");
            
            Console.WriteLine("\nSaving and loading games:");
            Console.WriteLine("- When saving, enter a filename without an extension");
            Console.WriteLine("- When loading, enter the same filename you used to save");
        }

    }
}
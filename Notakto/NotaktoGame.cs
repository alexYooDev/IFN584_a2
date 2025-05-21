// handle game loop logic
using System;
using System.Collections.Generic;
using GameFrameWork;

namespace Notakto
{
    public class NotaktoGame : AbstractGame
    {
        private Board Board;
        private Player Player1;
        private Player Player2;

        // true = Player1, false = Player2
        private bool Turn = true;

        // Determine if the game is ended
        private bool IsGameOver;

        // Game mode (HvH or HvC)
        private string Mode;

        public override void ConfigureGame()
        {
            Board = new NotaktoBoard();
            string gameMode = SelectGameMode();
            Mode = gameMode;
            IsGameOver = false;
        }

        public void SelectGameMode()
        {
            Console.WriteLine("\n|| +++ Select the mode of the game +++ ||");
            Console.WriteLine("1. HvH (Human vs Human)");
            Console.WriteLine("2. HvC (Human vs Computer)");
            Console.Write("\nEnter your choice >> ");

            int input = Convert.ToInt32(Console.ReadLine());
            string gameMode = string.Empty;

            switch (input)
            {
                case 1:
                    // Human vs Human
                    Console.WriteLine("\nYou selected Human vs Human mode.");
                    mode = "HvH";
                    break;
                case 2:
                    // Human vs Computer
                    Console.WriteLine("\nYou selected Human vs Computer mode.");
                    mode = "HvC";
                    break;
                default:
                    Console.WriteLine("\nInvalid mode selected. Please try again.");
                    SelectMode();
                    break;
            }

            return mode;
        }

        public override void ConfigurePlayer()
        {

            switch (Mode)
            {
                case "HvH":
                    Console.Write("\nEnter player 1 name >> ");
                    string player1Name = Console.ReadLine();
                    Player1 = new NotaktoHumanPlayer(player1Name, "Human", NumberSet1);

                    Console.Write("\nEnter player 2 name >> ");
                    string player2Name = Console.ReadLine();
                    Player2 = new NotaktoHumanPlayer(player2Name, "Human", NumberSet2);
                    break;
                case "HvC":
                    Player1 = new NotaktoHumanPlayer("player 1", "Human", NumberSet1);
                    Player2 = new NotaktoComputerPlayer("Computer", NumberSet2);
                    break;
                default:
                    Console.WriteLine("\nInvalid mode selected. Please try again.");
                    break;
            }
        }

        public void StartGameLoop()
        {
            Console.WriteLine("\n============================================ Game Started!  ============================================");
            // main game process
            while (!IsGameOver)
            {
                // Display the current player's turn
                Console.WriteLine($"\nCurrent Turn: {(Turn ? Player1.GetName() : Player2.GetName())}");

                // Display the current board status
                Board.GetBoardStatus();

                // Make a move for the current player (
                if (Mode == "HvC" && !Turn)
                {
                    MakeComputerMove();
                }
                else
                {
                    DisplayOptions();
                }

                // Check if the game is over
                CheckGameOver();

                // Switch turns
                Turn = !Turn;
            }
            DisplayGameResult();
            //Console.WriteLine($"\nGame Over! {CurrentPlayer.Name} loses (made the last move).\n");
        }

        public override void Play()
        {
            ConfigureGame();
            ConfigurePlayer();
            StartGameLoop();
        }

        // Display the options for the current turn
        private void DisplayOptions()
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
        // Make a move for the current player
        private void MakePlayerMove(Player player)
        {
            // Get player's selected position
            Console.Write("Select board (1-3): ");
            int boardIndex = int.Parse(Console.ReadLine()) - 1;

            Console.Write("Select cell (1-9): ");
            int cellNum = int.Parse(Console.ReadLine());
            int row = (cellNum - 1) / 3;
            int col = (cellNum - 1) % 3;

            // Validate and place the move on the board
            if (Board.IsValidMove(row, col, null, boardIndex))
            {
                Board.MakeMove(row, col, null, boardIndex);

                Move move = new Move(boardIndex, row, col, CurrentPlayer, null, previousState);
                MoveHistory.Push(move);
                ClearRedoStackOnNewMove();
            }
            else
            {
                // Retry on invalid input
                MakePlayerMove(player);
            }
        }


        private void MakeComputerMove()
        {
            NotaktoBoard notaktoBoard = (NotaktoBoard)Board;
            Random random = new Random();

            for (int b = 0; b < 3; b++)
            {
                if (!notaktoBoard.DeadBoards.Contains(b))
                    continue;

                for (int row = 0; row < 3; ++row)
                {
                    for (int col = 0; col < 3; ++col)
                    {
                        if (notaktoBoard.IsValidMove(row, col, null, b))
                        {
                            object previousState = Board.GetBoardState();
                            notaktoBoard.MakeMove(row, col, null, b);

                            if (IsWinningMove(b, row, col))
                            {
                                Move move = new Move(b, row, col, CurrentPlayer, null, previousState);
                                MoveHistory.Push(move);
                                ClearRedoStackOnNewMove();
                                Console.WriteLine($"Computer placed X at board {b + 1}, position ({row + 1},{col + 1})");
                                return;
                            }

                            // undo if not winning
                            notaktoBoard.SetBoardState(previousState);
                        }
                    }
                }
            }

            // if no winning move, pick random
            List<(int board, int row, int col)> validMoves = new();
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
        }

        private bool IsWinningMove(int boardIndex, int row, int col)
        {
            NotaktoBoard notaktoBoard = (NotaktoBoard)Board;
            return notaktoBoard.CheckThreeInARow(boardIndex);
        }
        public override bool CheckGameOver()
        {
            return ((NotaktoBoard)Board).AreAllBoardsDead();
        }

        // Display the result of the game
        private void DisplayGameResult()
        {
            // Condition with a winner
            if (Board.AreAllBoardsDeadBoard())
            {
                Board.GetBoardStatus();
                Console.WriteLine($"\nGame over! {(!Turn ? Player1.GetName() : Player2.GetName())} wins!");
            }
        }


        public void SaveGame()
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

        public void LoadGame()
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


        public void DisplayHelpMenu()
        {
            int helpMenuOption;

            // Display the help menu
            Console.WriteLine("\n|| +++ Help Menu +++ ||");
            Console.WriteLine("\nHow can I help you?");
            Console.WriteLine("1. Game rules");
            Console.WriteLine("2. Game commands");
            Console.WriteLine("3. Return to current game");
            Console.Write("\nEnter your choice >> ");
            try
            {
                helpMenuOption = Convert.ToInt32(Console.ReadLine());
                switch (helpMenuOption)
                {
                    case 1:
                        Console.WriteLine("\n============================================ Game rules  ============================================");
                        Console.WriteLine("\nThe game is played on three boards with grids of size 3x3.");
                        Console.WriteLine("\nBoth players play 'X'");
                        Console.WriteLine("Two players alternate playing 'X' in any free cell on any live board.");
                        Console.WriteLine("Once a board has three-in-a row, it is dead and remove from the game.")
                        Console.WriteLine("The player that is forced to complete three-in-a-row on the last live board is the loser.");
                        Console.WriteLine("\nThe game can be played in two modes: HvH (Human vs Human) or HvC (Human vs Computer).");
                        Console.WriteLine("HvH: Human vs Human, HvC: Human vs Computer");
                        DisplayHelpMenu();
                        break;
                    case 2:
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
                        DisplayHelpMenu();
                        break;
                    case 3:
                        Console.WriteLine("\nReturning to the current game...");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        DisplayHelpMenu();
                        return;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("The Input should be a number! Try again!");
                DisplayHelpMenu();
                return;
            }
            Console.WriteLine("================================= END OF HELP MENU ============================================");
        }
    }
}
}
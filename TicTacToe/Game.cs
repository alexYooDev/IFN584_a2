namespace TicTacToe
{
    using System.Collections.Generic;
    using System.Text.Json;

    /* 
Game class attributes:
- What is the number of grid of this game (3 X 3 ? 5 X 5?)
- Who is player 1 in this game?
- Who is player 2 in this game?
- turns of which player
- the mode of the game (HvH ? HvC?)

Game class methods (Actions)
- Start the new game
- Load the previously saved game
- Save the game
- 
*/
    public class Game
    {
        private Board Board;
        private Player Player1;
        private Player Player2;

        // true = Player1, false = Player2
        private bool Turn = true;

        // unique set of numbers for player 1 and player 2
        private HashSet<int> NumberSet1;
        private HashSet<int> NumberSet2;

        // Target winning sum of number
        private int TargetSum;

        // Determine if the game is ended
        private bool IsGameOver;

        // Game mode (HvH or HvC)
        private string Mode;

        public Game()
        {
            // Initialize the number sets for both players
            NumberSet1 = new HashSet<int>();
            NumberSet2 = new HashSet<int>();
        }

        public void ConfigureGame()
        {
            int boardSize = SelectBoardSize();
            string gameMode = SelectMode();
            Board = new Board(boardSize);
            Mode = gameMode;

            for (int i = 1; i <= boardSize * boardSize; ++i)
            {
                if (i % 2 == 1)
                {
                    NumberSet1.Add(i);
                }
            }

            // Generate the set of numbers for player 2
            for (int i = 1; i <= boardSize * boardSize; ++i)
            {
                if (i % 2 == 0)
                {
                    NumberSet2.Add(i);
                }
            }

            // Set the target sum for the game
            TargetSum = (boardSize * (boardSize * boardSize + 1)) / 2;
        }

        public string SelectMode()
        {
            Console.WriteLine("\n|| +++ Select the mode of the game +++ ||");
            Console.WriteLine("1. HvH (Human vs Human)");
            Console.WriteLine("2. HvC (Human vs Computer)");
            Console.Write("\nEnter your choice >> ");

            int input = Convert.ToInt32(Console.ReadLine());
            string mode = string.Empty;

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

        public int SelectBoardSize()
        {
            int boardSize = 0;

            bool validInput = false;

            Console.WriteLine("\n|| +++ Size of the board +++ ||");
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
                    Console.WriteLine("\nInvalid board size. Please enter a number greater or equals to 3.");
                }
            }

            return boardSize;
        }

        public void ConfigurePlayers()
        {
            switch (Mode)
            {
                case "HvH":
                    Console.Write("\nEnter player 1 name >> ");
                    string player1Name = Console.ReadLine();
                    Player1 = new HumanPlayer(player1Name, "Human", NumberSet1);

                    Console.Write("\nEnter player 2 name >> ");
                    string player2Name = Console.ReadLine();
                    Player2 = new HumanPlayer(player2Name, "Human", NumberSet2);
                    break;
                case "HvC":
                    Player1 = new HumanPlayer("player 1", "Human", NumberSet1);
                    Player2 = new ComputerPlayer("Computer", NumberSet2);
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
                Console.WriteLine($"\nTarget Sum: {TargetSum}");

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
        }

        public void Play()
        {
            ConfigureGame();
            ConfigurePlayers();
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
            int number = player.SelectNumber();
            int[] position = Board.SelectPosition();

            // Validate and place the move on the board
            if (Board.IsValidMove(position[0], position[1]))
            {
                Board.SetPosition(position[0], position[1], number);
            }
            else
            {
                // Retry on invalid input
                MakePlayerMove(player);
            }
        }

        private bool CalculateSumInLine(string line, int index)
        {
            int sum = 0;
            bool hasEmptySlot = false;
            // row 
            switch (line)
            {
                case "row":
                    // Check if the line has empty slot, if yes then check next line.
                    for (int i = 0; i < Board.GetBoardSize(); ++i)
                    {
                        int slotValue = Board.GetValue(index, i);
                        if (slotValue == 0)
                        {
                            hasEmptySlot = true;
                            break;
                        }
                        sum += slotValue;
                    }
                    // Check if the line is full and if the sum equals the target sum
                    return !hasEmptySlot && sum == TargetSum;
                case "col":
                    sum = 0;
                    hasEmptySlot = false;
                    for (int i = 0; i < Board.GetBoardSize(); ++i)
                    {
                        int slotValue = Board.GetValue(i, index);
                        if (slotValue == 0)
                        {
                            hasEmptySlot = true;
                            break;
                        }
                        sum += slotValue;
                    }
                    // Check if the line is full and if the sum equals the target sum
                    return !hasEmptySlot && sum == TargetSum;
                case "diagonal":
                    sum = 0;
                    hasEmptySlot = false;
                    for (int i = 0; i < Board.GetBoardSize(); ++i)
                    {
                        int slotValue = Board.GetValue(i, i);
                        if (slotValue == 0)
                        {
                            hasEmptySlot = true;
                            break;
                        }
                        sum += slotValue;
                    }
                    // Check if the line is full and if the sum equals the target sum
                    return !hasEmptySlot && sum == TargetSum;
                case "anti-diagonal":
                    sum = 0;
                    hasEmptySlot = false;
                    for (int i = 0; i < Board.GetBoardSize(); ++i)
                    {
                        int slotValue = Board.GetValue(i, Board.GetBoardSize() - 1 - i);
                        if (slotValue == 0)
                        {
                            hasEmptySlot = true;
                            break;
                        }
                        sum += slotValue;
                    }
                    // Check if the line is full and if the sum equals the target sum
                    return !hasEmptySlot && sum == TargetSum;
                default:
                    Console.WriteLine("\nInvalid line type. (e.g - 'row', 'col', 'diagonal', or 'anti-diagonal')");
                    return false;
            }
        }

        private bool CheckWinningLine()
        {
            for (int row = 0; row < Board.GetBoardSize(); ++row)
            {
                if (CalculateSumInLine("row", row))
                {
                    return true;
                }
            }

            for (int col = 0; col < Board.GetBoardSize(); ++col)
            {
                if (CalculateSumInLine("col", col))
                {
                    return true;
                }
            }
            // Check diagonal
            if (CalculateSumInLine("diagonal", 0))
            {
                return true;
            }
            // Check anti-diagonal
            if (CalculateSumInLine("anti-diagonal", 0))
            {
                return true;
            }
            return false;
        }

        // Check if the Gameover condition is met
        private void CheckGameOver()
        {
            for (int row = 0; row < Board.GetBoardSize(); ++row)
            {
                if (CalculateSumInLine("row", row))
                {
                    IsGameOver = true;
                    return;
                }
            }

            for (int col = 0; col < Board.GetBoardSize(); ++col)
            {
                if (CalculateSumInLine("col", col))
                {
                    IsGameOver = true;
                    return;
                }
            }
            // Check diagonal
            if (CalculateSumInLine("diagonal", 0))
            {
                IsGameOver = true;
                return;
            }
            // Check anti-diagonal
            if (CalculateSumInLine("anti-diagonal", 0))
            {
                IsGameOver = true;
                return;
            }

            if (Board.IsBoardFull())
            {
                IsGameOver = true;
                return;
            }

            else { IsGameOver = false; }
        }


        // Display the result of the game
        private void DisplayGameResult()
        {

            // Draw condition
            if (Board.IsBoardFull() && !CheckWinningLine())
            {
                Board.GetBoardStatus();
                Console.WriteLine("\nGame over! It's a draw!");
            }
            // Condition with a winner
            else
            {
                Board.GetBoardStatus();
                Console.WriteLine($"\nGame over! {(!Turn ? Player1.GetName() : Player2.GetName())} wins!");
            }
        }


        // Check if the move is winning move for computer player
        private bool IsWinningMove(int row, int col)
        {
            // check row's winning move
            int rowSum = 0;
            bool rowFull = true;
            int boardSize = Board.GetBoardSize();

            for (int i = 0; i < boardSize; ++i)
            {

                int rowValue = Board.GetValue(row, i);
                if (rowValue == 0)
                {
                    rowFull = false;
                    break;
                }
                rowSum += rowValue;
            }
            if (rowFull && rowSum == TargetSum)
            {
                return true;
            }

            //check column's winning move
            int colSum = 0;
            bool colFull = true;
            for (int i = 0; i < boardSize; ++i)
            {
                int colValue = Board.GetValue(i, col);
                if (colValue == 0)
                {
                    colFull = false;
                    break;
                }
                colSum += colValue;
            }
            if (colFull && colSum == TargetSum)
            {
                return true;
            }

            if (row == col)
            {
                int diagonalSum = 0;
                bool diagonalFull = true;
                for (int i = 0; i < boardSize; ++i)
                {
                    int diagonalValue = Board.GetValue(i, i);
                    if (diagonalValue == 0)
                    {
                        diagonalFull = false;
                        break;
                    }
                    diagonalSum += diagonalValue;
                }
                if (diagonalFull && diagonalSum == TargetSum)
                {
                    return true;
                }
            }
            // check anti-diagonal's winning move
            if (row + col == boardSize - 1)
            {
                int antiDiagonalSum = 0;
                bool antiDiagonalFull = true;
                for (int i = 0; i < boardSize; ++i)
                {
                    int antiDiagonalValue = Board.GetValue(i, boardSize - 1 - i);
                    if (antiDiagonalValue == 0)
                    {
                        antiDiagonalFull = false;
                        break;
                    }
                    antiDiagonalSum += antiDiagonalValue;
                }
                if (antiDiagonalFull && antiDiagonalSum == TargetSum)
                {
                    return true;
                }
            }
            return false;
        }


        /* 
            Simulate computer player's move (acting towards winning move)
         */
        private void MakeComputerMove()
        {
            int sideSize = Board.GetBoardSize();

            List<int> availableNumbers = Player2.GetGivenNumbers().ToList();

            foreach (int number in availableNumbers)
            {
                for (int row = 0; row < sideSize; ++row)
                {
                    for (int col = 0; col < sideSize; ++col)
                    {
                        if (Board.GetValue(row, col) == 0)
                        {
                            Board.SetPosition(row, col, number);
                            if (IsWinningMove(row, col))
                            {
                                Player2.GetGivenNumbers().Remove(number);
                                Console.WriteLine($"\nComputer placed {number} at position ({row * Board.GetBoardSize() + col + 1})");
                                return;
                            }
                            else
                            {
                                // Undo the move if it doesn't lead to a win
                                Board.SetPosition(row, col, 0);
                            }
                        }
                    }
                }
            }

            // If no winning move is found, make a random move
            Random random = new Random();
            List<(int, int)> emptyPositions = new List<(int, int)>();

            for (int row = 0; row < Board.GetBoardSize(); row++)
            {
                for (int col = 0; col < Board.GetBoardSize(); col++)
                {
                    if (Board.GetValue(row, col) == 0)
                    {
                        emptyPositions.Add((row, col));
                    }
                }
            }

            if (emptyPositions.Count > 0 && availableNumbers.Count > 0)
            {
                int randomNumberIndex = random.Next(availableNumbers.Count);
                int randomNumber = availableNumbers[randomNumberIndex];

                int randomPositionIndex = random.Next(emptyPositions.Count);
                (int row, int col) = emptyPositions[randomPositionIndex];

                Board.SetPosition(row, col, randomNumber);
                Player2.GetGivenNumbers().Remove(randomNumber);
                Console.WriteLine($"Computer placed {randomNumber} at position ({row * Board.GetBoardSize() + col + 1})");
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
                int[][] boardData = new int[Board.GetBoardSize()][];
                for (int i = 0; i < Board.GetBoardSize(); ++i)
                {
                    boardData[i] = new int[Board.GetBoardSize()];
                    for (int j = 0; j < Board.GetBoardSize(); ++j)
                    {
                        boardData[i][j] = Board.GetValue(i, j);
                    }
                }
                GameData SaveGameData = new GameData
                {
                    BoardSize = Board.GetBoardSize(),
                    GameMode = Mode,
                    BoardState = boardData,
                    CurrentPlayer1Name = Player1.GetName(),
                    CurrentPlayer2Name = Player2.GetName(),
                    CurrentTurn = Turn,
                    IsGameOver = IsGameOver,
                    RemainingOdds = Player1.GetGivenNumbers(),
                    RemainingEvens = Player2.GetGivenNumbers(),
                    TargetSum = TargetSum
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
                    Board = new Board(loadedGameData.BoardSize);

                    // Convert BoardState from JSON compatible board form int[][] to int[,] 
                    int[,] boardState = new int[loadedGameData.BoardSize, loadedGameData.BoardSize];
                    for (int i = 0; i < loadedGameData.BoardSize; ++i)
                    {
                        for (int j = 0; j < loadedGameData.BoardSize; ++j)
                        {
                            boardState[i, j] = loadedGameData.BoardState[i][j];
                        }
                    }
                    Board.SetBoardData(boardState);

                    Mode = loadedGameData.GameMode;
                    Turn = loadedGameData.CurrentTurn;
                    Player1 = new HumanPlayer(loadedGameData.CurrentPlayer1Name, "Human", loadedGameData.RemainingOdds);
                    Player2 = loadedGameData.CurrentPlayer2Name == "Computer" ? new ComputerPlayer("Computer", loadedGameData.RemainingEvens) : new HumanPlayer(loadedGameData.CurrentPlayer2Name, "Human", loadedGameData.RemainingEvens);
                    IsGameOver = loadedGameData.IsGameOver;
                    TargetSum = loadedGameData.TargetSum;

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
                        Console.WriteLine("\nThe game is played on a grid of size NxN, where N is the size of the board.");
                        Console.WriteLine("Player 1 uses odd numbers (1, 3, 5, ...)");
                        Console.WriteLine("Player 2 uses even numbers (2, 4, 6, ...)");
                        Console.WriteLine("\nThe goal of the game is to get a sum of (N * (N^2 + 1)) / 2 in any row, column, or diagonal.");
                        Console.WriteLine("The game can be played in two modes: HvH (Human vs Human) or HvC (Human vs Computer).");
                        Console.WriteLine("HvH: Human vs Human, HvC: Human vs Computer");
                        Console.WriteLine("\nThe game is played in turns, where each player selects a number from their set of numbers.");
                        Console.WriteLine("The player can place the number on the board in an empty cell.");
                        Console.WriteLine("The game ends when one player reaches the target sum or when the board is full.");
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
                        Console.WriteLine("\n<<Player can select a number on the grid position to put the number as shown below.>>");
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
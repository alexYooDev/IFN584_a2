using System.Text.Json;

namespace GameFrameWork
{
    using System.Collections.Generic;

    /* This base game class encompasses all board games */
    /* The class implements multiple interfaces for Single Responsibility Principle
        - Only manages common game flow and logic
     */

    public abstract class AbstractGame
    {

        /* Interface dependencies to apply Dependency Inversion Principle */
        protected readonly IGameRenderer renderer;
        protected readonly IInputHandler inputHandler;
        protected readonly IGameDataPersistence dataPersistence;

        /* Game state properties shared in all games */
        public AbstractBoard Board { get; set; }
        protected AbstractPlayer CurrentPlayer { get; set; }
        protected AbstractPlayer Player1 { get; set; }
        protected AbstractPlayer Player2 { get; set; }
        protected bool IsGameOver { get; set; }
        protected string GameMode { get; set; } // HvH or HvC
        protected bool IsPlayerQuit { get; set; } = false;
        protected Stack<Move> MoveHistory { get; set; }
        protected Stack<Move> RedoHistory { get; set; }

        /* Constructor - incorporated with dependency injection*/
        protected AbstractGame(IGameRenderer renderer, IInputHandler inputHandler, IGameDataPersistence dataPersistence)
        {
            /* check if the value is null */
            this.renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            this.inputHandler = inputHandler ?? throw new ArgumentNullException(nameof(inputHandler));
            this.dataPersistence = dataPersistence ?? throw new ArgumentNullException(nameof(dataPersistence));
            MoveHistory = new Stack<Move>();
            RedoHistory = new Stack<Move>();
        }

        protected virtual void SelectGameMode()
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

        protected virtual void ConfigurePlayersWithNames()
        {
            switch (GameMode)
            {
                case "HvH":
                    string player1Name = inputHandler.GetUserInput("Enter player 1 name");
                    string player2Name = inputHandler.GetUserInput("Enter player 2 name");
                    CreateHumanVsHumanPlayers(player1Name, player2Name);
                    break;

                case "HvC":
                    string playerName = inputHandler.GetUserInput("Enter your name");
                    CreateHumanVsComputerPlayers(playerName);
                    break;
            }

            CurrentPlayer = Player1;
        }

        protected string GetSaveDirectory()
        {
            string saveDirectory = Path.Combine(Directory.GetCurrentDirectory(), "saveData");
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
            return saveDirectory;
        }

        protected string GetSaveFilePath(string filename)
        {
            return Path.Combine(GetSaveDirectory(), filename + ".json");
        }

        protected void HandleSaveSuccess(string filename)
        {
            renderer.DisplayMessage($"\nGame saved successfully as {filename}");
        }

        protected void HandleSaveError(Exception e)
        {
            renderer.DisplayMessage($"\nError saving game: {e.Message}");
        }

        protected void HandleLoadSuccess(string filename)
        {
            renderer.DisplayMessage($"\nGame loaded successfully from {filename}");
        }

        protected void HandleLoadError(Exception e)
        {
            renderer.DisplayMessage($"\nError loading game: {e.Message}");
        }

        protected void HandleFileNotFound()
        {
            renderer.DisplayMessage("\nSave file not found. Please check the filename and try again.");
        }

        /* Template Method Pattern : provide save/load game structure for all game types */

        protected virtual void SaveGame(string filename)
        {
            try
            {
                // Create game-specific data object
                var gameData = CreateGameData();

                // Populate with current game state (including BoardState)
                gameData.PopulateFromGame(this);

                // Save using data persistence
                SaveGameData(gameData, filename);

                HandleSaveSuccess(filename);
            }
            catch (Exception e)
            {
                HandleSaveError(e);
            }
        }

        public virtual bool LoadGame(string filename)
        {
            try
            {
                // Load game-specific data
                var gameData = LoadGameData(filename);

                if (gameData == null)
                {
                    return false;
                }

                // Restore game state
                gameData.RestoreToGame(this);

                HandleLoadSuccess(filename);
                return true;
            }
            catch (Exception e)
            {
                HandleLoadError(e);
                return false;
            }
        }

        /* Template method for game flow */
        public virtual void Play()
        {
            IsPlayerQuit = false;
            ConfigureGame();
            StartGame();
        }

        /* To start the game loop */
        public virtual void StartGame()
        {
            renderer.DisplayMessage("\n============================================ Game Started!  ============================================");
            IsGameOver = false;
            IsPlayerQuit = false;

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
                }
                else
                {
                    ProcessComputerTurn();
                }

                IsGameOver = CheckGameOver();

                if (!IsGameOver)
                {
                    SwitchCurrentPlayer();
                }
            }

            // Only display results if player didn't quit
            if (!IsPlayerQuit)
            {
                DisplayGameStatus();
                AnnounceWinner();
            }
        }

        // Process human player turn using interface methods
        protected virtual void ProcessHumanTurn()
        {
            bool turnComplete = false;
            while (!turnComplete)
            {
                renderer.DisplayTurnOptions();
                PlayerChoice input = inputHandler.GetPlayerChoice();

                switch (input)
                {
                    case PlayerChoice.MakeMove:
                        MakeHumanMove();
                        turnComplete = true;
                        break;

                    case PlayerChoice.Undo:
                        HandleUndoRequest();
                        break;

                    case PlayerChoice.Save:
                        HandleSaveRequest();
                        break;

                    case PlayerChoice.Help:
                        HandleHelpRequest();
                        break;

                    case PlayerChoice.Quit:
                        HandleQuitRequest();
                        turnComplete = true;
                        break;

                    case PlayerChoice.Invalid:
                        renderer.DisplayMessage("\nInvalid choice. Please try again.");
                        break;
                }
            }
        }

        protected virtual void HandleHelpRequest()
        {
            renderer.DisplayHelpMenu();
            int choice = inputHandler.GetUserIntInput("Enter your choice", 1, 3);

            switch (choice)
            {
                case 1:
                    DisplayRules();
                    break;
                case 2:
                    DisplayCommands();
                    break;
                case 3:
                    // return to game
                    return;
            }
        }

        protected virtual void SwitchCurrentPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
        }

        // Handle quit request
        protected virtual void HandleQuitRequest()
        {
            bool confirmQuit = inputHandler.GetUserConfirmation("Are you sure you want to quit the game?");
            if (confirmQuit)
            {
                IsGameOver = true;
                IsPlayerQuit = true;
                renderer.DisplayMessage("Thank you for playing!");
            }
        }

        // Handle save request
        protected virtual void HandleSaveRequest()
        {
            string saveFilename = inputHandler.GetUserInput("Enter filename to save");
            SaveGame(saveFilename);
        }

        // Handle undo request
        protected virtual void HandleUndoRequest()
        {
            int maxUndo = GetUndoableMoveCountForPlayer(CurrentPlayer);
            if (maxUndo > 0)
            {
                int undoCount = inputHandler.GetUserIntInput($"How many moves to undo (up to {maxUndo})?", 1, maxUndo);
                UndoPlayerMoves(CurrentPlayer, undoCount);
            }
            else
            {
                renderer.DisplayMessage("No moves to undo.");
            }
        }

        /* Display game status - current player's name, the number of turn, and board display */
        protected virtual void DisplayGameStatus()
        {
            renderer.DisplayGameStatus(CurrentPlayer.Name, MoveHistory.Count);
            renderer.DisplayBoard(Board);
        }

        protected virtual void DisplayRules()
        {
            string rules = GetGameRules();
            renderer.DisplayRules(rules);
            renderer.PressAnyKeyToContinue();
        }

        protected virtual void DisplayCommands()
        {
            string commands = GetGameCommands();
            renderer.DisplayCommands(commands);
            renderer.PressAnyKeyToContinue();
        }

        // Offer undo after loading the saved game 
        protected virtual void OfferUndoAfterLoad()
        {
            // Default implementation - can be overridden in derived classes
            int maxUndo = GetUndoableMoveCountForPlayer(CurrentPlayer);

            if (maxUndo > 0)
            {
                Console.WriteLine($"\nYou have {maxUndo} move(s) that can be undone.");
                bool confirmUndo = inputHandler.GetUserConfirmation("Would you like to undo any moves?");

                if (confirmUndo)
                {
                    int undoCount = inputHandler.GetUserIntInput($"How many moves to undo (up to {maxUndo})?", 1, maxUndo);
                    UndoPlayerMoves(CurrentPlayer, undoCount);
                }
            }
        }

        /* Returns the max number of moves players made  */
        protected int GetUndoableMoveCountForPlayer(AbstractPlayer player)
        {
            int count = 0;
            foreach (var move in MoveHistory)
            {
                if (move.Player.Name == player.Name)
                    count++;
            }
            return count;
        }

        protected void UndoPlayerMoves(AbstractPlayer player, int movesToUndo)
        {
            if (movesToUndo <= 0)
            {
                Console.WriteLine("Invalid number of moves to undo.");
                return;
            }

            int undone = 0;
            Stack<Move> tempStack = new Stack<Move>();

            // Keep track of consecutive opponent moves that were skipped
            // to maintain correct game state
            Stack<Move> skippedMoves = new Stack<Move>();

            while (undone < movesToUndo && MoveHistory.Count > 0)
            {
                var move = MoveHistory.Pop();

                if (move.Player.Name == player.Name)
                {
                    // Process the current player's move
                    // First, restore any skipped opponent moves to the history
                    while (skippedMoves.Count > 0)
                    {
                        MoveHistory.Push(skippedMoves.Pop());
                    }

                    // Now handle this player's move
                    RedoHistory.Push(move);
                    ApplyUndoState(move);
                    undone++;
                }
                else
                {
                    // If the player undoes the move, it undoes any opponent's previous move
                    // moves that happened after the last move
                    skippedMoves.Push(move);

                    // We must also undo the opponent's move from the board state
                    // Without counting it toward the player's undo count
                    ApplyUndoState(move);
                }
            }

            // Restore skipped moves that didn't get processed
            while (skippedMoves.Count > 0)
            {
                MoveHistory.Push(skippedMoves.Pop());
            }

            // Restore other moves that were popped but not the player's
            while (tempStack.Count > 0)
                MoveHistory.Push(tempStack.Pop());

            if (undone > 0)
            {
                renderer.DisplayMessage($"Undid {undone} move(s) for {player.Name}.");
            }
            else
            {
                renderer.DisplayMessage("No moves to undo currently for this player.");
            }
        }

        protected void RedoPlayerMoves(AbstractPlayer player, int movesToRedo)
        {
            if (movesToRedo <= 0)
            {
                Console.WriteLine("Invalid number of moves to redo.");
                return;
            }

            int redone = 0;
            Stack<Move> tempStack = new Stack<Move>();
            while (redone < movesToRedo && RedoHistory.Count > 0)
            {
                var move = RedoHistory.Pop();
                if (move.Player.Name == player.Name)
                {
                    MoveHistory.Push(move);
                    ApplyRedoState(move);
                    redone++;
                }
                else
                {
                    tempStack.Push(move);
                }
            }
            // Restore other moves
            while (tempStack.Count > 0)
                RedoHistory.Push(tempStack.Pop());

            if (redone > 0)
            {
                Console.WriteLine($"Redid {redone} move(s) for {player.Name}.");
            }
            else
            {
                Console.WriteLine("No moves to redo currently for this player.");
            }
        }

        // Clear redo history when a new move is made after undo
        protected virtual void ClearRedoStackOnNewMove()
        {
            if (RedoHistory.Count > 0)
            {
                RedoHistory.Clear();
                renderer.DisplayMessage("Redo history cleared due to new move.");
            }
        }

        // Check if player quit game
        protected bool DidPlayerQuit()
        {
            return IsPlayerQuit;
        }


        /* Abstract methods that need implementation in derived classes */

        protected abstract void ConfigureGame();
        protected abstract void ConfigurePlayer();
        protected abstract void CreateHumanVsHumanPlayers(string player1Name, string player2Name);
        protected abstract void CreateHumanVsComputerPlayers(string playerName);

        /* Configure player settings => for setting their names */
        protected abstract void MakeHumanMove();
        protected abstract void ProcessComputerTurn();
        protected abstract bool CheckGameOver();
        protected abstract void AnnounceWinner();
        protected abstract void ApplyUndoState(Move move);
        protected abstract void ApplyRedoState(Move move);
        protected abstract string GetGameRules();
        protected abstract string GetGameCommands();

        /* Save and load related abstract functions -> specific games can access abstract functions to 
          reflect their specific game data
         */
        protected abstract GameData CreateGameData();
        protected abstract void SaveGameData(GameData gameData, string filename);
        protected abstract GameData LoadGameData(string filename);

    }
}
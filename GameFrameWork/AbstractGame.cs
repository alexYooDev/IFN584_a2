namespace GameFrameWork
{
    using System.Collections.Generic;

    public abstract class AbstractGame
    {
        protected AbstractBoard Board { get; set; }
        protected AbstractPlayer CurrentPlayer { get; set; }
        protected AbstractPlayer Player1 { get; set; }
        protected AbstractPlayer Player2 { get; set; }
        protected bool IsGameOver { get; set; }
        protected string GameMode { get; set; } // HvH or HvC
        protected bool IsPlayerQuit { get; set; } = false;
        protected Stack<Move> MoveHistory { get; set; }
        protected Stack<Move> RedoHistory { get; set; }

        /* Constructor */
        public AbstractGame()
        {
            MoveHistory = new Stack<Move>();
            RedoHistory = new Stack<Move>();
        }

        /* Abstract game methods to be implemented */

        /* Configure game settings, selecting different games would possibly happen here */
        public abstract void ConfigureGame();

        /* Configure player settings => for setting their names */
        public abstract void ConfigurePlayer();

        /* Template method for game flow */
        public virtual void Play()
        {
            IsPlayerQuit = false;
            ConfigureGame();
            ConfigurePlayer();
            StartGame();
        }

        /* To start the game loop */
        public virtual void StartGame()
        {
            Console.WriteLine("\n============================================ Game Started!  ============================================");
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
                    SwithCurrentPlayer();
                }
            }
            
            // Only display results if player didn't quit
            if (!IsPlayerQuit)
            {
                DisplayGameStatus();
                AnnounceWinner();
            }
        }

        // Process human player's turn
        protected virtual void ProcessHumanTurn()
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
                        turnComplete = true;
                        break;
                    default:
                        Console.WriteLine("\nInvalid choice. Please try again.");
                        break;
                }
            }
        }
        
        // Common game options
        protected virtual void DisplayTurnOptions()
        {
            Console.WriteLine("\n|| +++ Options +++ ||");
            Console.WriteLine("\nSelect your option for this turn:\n");
            Console.WriteLine("1. Make a move");
            Console.WriteLine("2. Undo previous moves");
            Console.WriteLine("3. Save the game");
            Console.WriteLine("4. View help menu");
            Console.WriteLine("5. Quit the game");
            Console.Write("\nEnter your choice >> ");
        }
        
        // Handle quit request
        protected virtual void HandleQuitRequest()
        {
            IsGameOver = true;
            IsPlayerQuit = true;
        }
        
        // Handle save request
        protected virtual void HandleSaveRequest()
        {
            Console.Write("\nEnter filename to save >> ");
            string saveFilename = Console.ReadLine();
            SaveGame(saveFilename);
        }
        
        // Handle undo request
        protected virtual void HandleUndoRequest()
        {
            int maxUndo = GetUndoableMoveCountForPlayer(CurrentPlayer);
            if (maxUndo > 0)
            {
                Console.Write($"How many moves to undo (up to {maxUndo})? ");
                if (int.TryParse(Console.ReadLine(), out int undoCount) && undoCount > 0 && undoCount <= maxUndo)
                {
                    UndoPlayerMoves(CurrentPlayer, undoCount);
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

        /* Abstract methods that need implementation in derived classes */
        protected abstract void MakeHumanMove();
        protected abstract void ProcessComputerTurn();
        public abstract bool CheckGameOver();
        protected abstract void AnnounceWinner();
        public abstract void DisplayGameStatus();
        public abstract void SaveGame(string filename);
        public abstract bool LoadGame(string filename);
        protected abstract void ApplyUndoState(Move move);
        protected abstract void ApplyRedoState(Move move);
        protected abstract void SwithCurrentPlayer();
        protected abstract void DisplayRules();
        protected abstract void DisplayCommands();

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

        protected int GetRedoableMoveCountForPlayer(AbstractPlayer player)
        {
            int count = 0;
            foreach (var move in RedoHistory)
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
                    // If we're undoing our move, we need to also "undo" any opponent 
                    // moves that happened after our last move
                    skippedMoves.Push(move);
                    
                    // We must also undo the opponent's move from the board state
                    // but we don't count it toward the player's undo count
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
                Console.WriteLine($"Undid {undone} move(s) for {player.Name}.");
            }
            else
            {
                Console.WriteLine("No moves to undo currently for this player.");
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
                Console.WriteLine("Redo history cleared due to new move.");
            }
        }

        /* Display help menu */
        public virtual void DisplayHelpMenu()
        {
            Console.WriteLine("\n|| +++ Help Menu +++ ||");
            Console.WriteLine("\nHow can I help you?");
            Console.WriteLine("1. Game Rules");
            Console.WriteLine("2. Available Commands");
            Console.WriteLine("3. Return to Game");
            Console.Write("\nEnter your choice >> ");

            int selectedChoice;
            bool validInputEntered = int.TryParse(Console.ReadLine(), out selectedChoice);
            if (validInputEntered)
            {
                switch (selectedChoice)
                {
                    case 1:
                        DisplayRules();
                        break;
                    case 2:
                        DisplayCommands();
                        break;
                    case 3:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again!");
                        return;
                }
            }
        }
        
        // Check if player quit game
        public bool DidPlayerQuit()
        {
            return IsPlayerQuit;
        }
        
        // Offer undo after loading a saved game
        protected virtual void OfferUndoAfterLoad()
        {
            // Default implementation - can be overridden in derived classes
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
                        UndoPlayerMoves(CurrentPlayer, undoCount);
                    }
                    else
                    {
                        Console.WriteLine($"Invalid input. No moves will be undone.");
                    }
                }
            }
        }
    }
}
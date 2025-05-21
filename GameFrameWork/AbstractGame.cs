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

        /* To start the game loop */
        public abstract void StartGame();

        /* A method that executes 
            ConfigureGame();
            ConfigurePlayers();
            StartGameLoop();
         */
        public abstract void Play();
        public abstract bool CheckGameOver();

        /* Displays current status of the game */
        public abstract void DisplayGameStatus();

        /* Save game | Load game */
        public abstract void SaveGame(string filename);
        public abstract void LoadGame(string filename);

        /* Redo | Undo */

        /* Update changed undo state  */
        protected abstract void ApplyUndoState(Move move);

        /* update changed redo state */
        protected abstract void ApplyRedoState(Move move);

        /* switching players turn */
        protected abstract void SwithCurrentPlayer();
        

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
            while (undone < movesToUndo && MoveHistory.Count > 0)
            {
                var move = MoveHistory.Pop();
                if (move.Player.Name == player.Name)
                {
                    RedoHistory.Push(move);
                    ApplyUndoState(move);
                    undone++;
                }
                else
                {
                    tempStack.Push(move);
                }
            }
            // Restore other moves
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

        // public virtual void UndoMove(int movesToUndo)
        // {
        //     if (movesToUndo <= 0)
        //     {
        //         Console.WriteLine("Invalid number of moves to undo.");
        //         return;
        //     }

        //     int undoCount = 0;
        //     while (undoCount < movesToUndo && MoveHistory.Count > 0)
        //     {
        //         Move lastMove = MoveHistory.Pop();
        //         RedoHistory.Push(lastMove);
        //         ApplyUndoState(lastMove);
        //         undoCount++;
        //     }

        //     if (undoCount > 0)
        //     {
        //         Console.WriteLine($"Undid {undoCount} move(s).");
        //         // The current player should be set in ApplyUndoState implementation
        //     }
        //     else
        //     {
        //         Console.WriteLine("No moves to undo currently.");
        //     }
        // }

        /* if there was a last move in the redo history, pop the move from the redo history stack  */

        /* 
            Stack DataStructure  []
            LIFO : Last in First out => ensures that it can reddo the last move made 
         */

        // public virtual void RedoMove(int movesToRedo)
        // {
        //     if (movesToRedo <= 0)
        //     {
        //         Console.WriteLine("Invalid number of moves to redo.");
        //         return;
        //     }

        //     int redoCount = 0;
        //     while (redoCount < movesToRedo && RedoHistory.Count > 0)
        //     {
        //         Move redoMove = RedoHistory.Pop();
        //         MoveHistory.Push(redoMove);
        //         ApplyRedoState(redoMove);
        //         redoCount++;
        //     }

        //     if (redoCount > 0)
        //     {
        //         Console.WriteLine($"Redid {redoCount} move(s).");
        //         // The current player should be set in ApplyRedoState implementation
        //     }
        //     else
        //     {
        //         Console.WriteLine("No moves to redo currently.");
        //     }
        // }

        // Clear redo history when a new move is made after undo
        protected virtual void ClearRedoStackOnNewMove()
        {
            if (RedoHistory.Count > 0)
            {
                RedoHistory.Clear();
                Console.WriteLine("Redo history cleared due to new move.");
            }
        }

        /* Help menu specific abstract functions */


        /* Help instruction would possibly be imported from the seperate file */

        /* display chosen game's rule */
        protected abstract void DisplayRules();
        /* display specific commands to use in the chosen games */
        protected abstract void DisplayCommands();

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
    }
}
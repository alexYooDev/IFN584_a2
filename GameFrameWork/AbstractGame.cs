namespace GameFrameWork
{
    using System.Collections.Generic;

    public abstract class AbstractGame
    {
        protected AbstractBoard Board { get; set; }
        protected Player Player1 { get; set; }
        protected Player Player2 { get; set; }
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
        public abstract void ConfigureGame();
        public abstract void StartGame();
        public abstract void Play();
        public abstract bool CheckGameOver();
        // boardIndex is addded for Notakto
        public abstract void MakeMove(int boardIndex, int row, int col, object moveData = null);
        public abstract void DisplayGameStatus();

        /* Save game | Load game */
        public abstract void SaveGame(string filename);
        public abstract void LoadGame(string filename);

        /* Redo | Undo */

        protected abstract void ApplyUndoState(Move move);
        protected abstract void ApplyRedoState(Move move);
        protected abstract void SwithCurrentPlayer();

        public virtual void UndoMove(int movesToUndo)
        {
            if (movesToUndo <= 0)
            {
                Console.WriteLine("Invalid number of moves to undo.");
                return;
            }
            
            int undoCount = 0;
            while (undoCount < movesToUndo && MoveHistory.Count > 0)
            {
                Move lastMove = MoveHistory.Pop();
                RedoHistory.Push(lastMove);
                ApplyUndoState(lastMove);
                undoCount++;
            }
            
            if (undoCount > 0)
            {
                Console.WriteLine($"Undid {undoCount} move(s).");
                // The current player should be set in ApplyUndoState implementation
            }
            else
            {
                Console.WriteLine("No moves to undo currently.");
            }
        }

        /* if there was a last move in the redo history, pop the move from the redo history stack  */

            /* 
                Stack DataStructure  []
                LIFO : Last in First out => ensures that it can reddo the last move made 
             */

        public virtual void RedoMove(int movesToRedo)
        {
            if (movesToRedo <= 0)
            {
                Console.WriteLine("Invalid number of moves to redo.");
                return;
            }
            
            int redoCount = 0;
            while (redoCount < movesToRedo && RedoHistory.Count > 0)
            {
                Move redoMove = RedoHistory.Pop();
                MoveHistory.Push(redoMove);
                ApplyRedoState(redoMove);
                redoCount++;
            }
            
            if (redoCount > 0)
            {
                Console.WriteLine($"Redid {redoCount} move(s).");
                // The current player should be set in ApplyRedoState implementation
            }
            else
            {
                Console.WriteLine("No moves to redo currently.");
            }
        }

        protected virtual void ClearRedoStackOnNewMove()
        {
            if (RedoHistory.Count > 0)
            {
                RedoHistory.Clear();
                Console.WriteLine("Redo history cleared due to new move.");
            }
        }

        /* Help menu specific abstract functions */
        protected abstract void DisplayRules();
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
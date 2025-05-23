namespace GameFrameWork
{
    /* Interface for handling user input operations */
    public interface IInputHandler
    {
        /* Input handling methods */
        string GetUserInput(string prompt);
        int GetUserIntInput(string prompt, int min, int max);
        bool GetUserConfirmation(string message);

        /* handle player's option choice */
        PlayerChoice GetPlayerChoice();
    }

    /* Instead of hard coding numbers, provide template enum for player's choice */

    public enum PlayerChoice
    {
        MakeMove = 1,
        Undo = 2,
        Save = 3,
        Help = 4,
        Quit = 5,
        Invalid = -1,
    }
}
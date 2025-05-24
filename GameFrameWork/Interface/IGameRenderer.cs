namespace GameFrameWork
{
    /* Game UI rendering interface resposible for framing all rendering functionalities */
    public interface IGameRenderer
    {

        /* Game UI display related frames */
        void DisplayBoard(AbstractBoard board);
        void DisplayMessage(string message);
        void DisplayGameStatus(string currentPlayer, int moveCount);
        void DisplayTurnOptions();

        /* Game information display frame */
        void DisplayHelpMenu();
        void DisplayRules(string rules);
        void DisplayCommands(string commands);

        /* Command terminal UI method frames */
        void ClearScreen();
        void PressAnyKeyToContinue();
    }
}
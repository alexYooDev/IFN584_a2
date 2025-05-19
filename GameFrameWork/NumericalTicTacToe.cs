namespace GameFrameWork
{
    public class NumericalTicTacToe : AbstractGame
    {
        private int[,] Slots;
        private int TargetSum;

        public NumericalTicTacToe(int size)
        {
            Slots = new int[size, size];
            TargetSum = (size * (size * size + 1)) / 2;
        }

        public int GetTargetSum()
        {
            return TargetSum;
        }
    }
}
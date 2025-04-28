using TicTacToe;

namespace TicTacToe
{

    /* Parent Class Player */

    /* 
        Player class attributes:
        - What type of player is this?
        - What numbers do you get from start?
        - Is this player first on to start?

        Player class methods (Actions)
        - Place number on the board
        - Select number to put on the board
    */
    public abstract class Player 
    {
        // player type : Human or Computer
        public string Type { get; }
        // unique set of numbers for player {1,3,5,7,9} for first or {2,4,6,8} for second player
        private HashSet<int> GivenNumbers { get; set; }
        // first player or second player

        public Player(string type, HashSet<int> givenNumbers)
        {
            Type = type;
            GivenNumbers = givenNumbers;
        }

        // Prompts the user to select a number from their own set to put on the board 
        public int SelectNumber()
        {
            try 
            {
                Console.Write($"\nSelect the number to put on the board! ({string.Join(", ", GivenNumbers)}) >> ");
                int selectedNumber = Convert.ToInt32(Console.ReadLine());

                // Check if the selected number is in the player's set of numbers
                if(GivenNumbers.Contains(selectedNumber))
                {
                    GivenNumbers.Remove(selectedNumber);
                    return selectedNumber;
                }
                
                // If the number is not in the set, prompt the user again
                else if(!GivenNumbers.Contains(selectedNumber))
                {
                    Console.WriteLine("You do not have this number! Please try again!");
                    return SelectNumber();
                }


                // Prompts the error message to user to try again
                else
                {
                    Console.WriteLine("You entered an invalid number input! Please try again!");
                    return SelectNumber();
                }
            } 
             catch (FormatException)
            {
                Console.WriteLine("The Input should be a number! Try again!");
                return SelectNumber();
            }
        }

        // returns the set of player's given numbers
        public HashSet<int> GetGivenNumbers()
        {
            return GivenNumbers;
        }

        // sets the set of player's number and returns it
        public  HashSet<int> SetGivenNumbers(HashSet<int> givenNumbers)
        {
            GivenNumbers = new HashSet<int>(givenNumbers);
            return GivenNumbers;
        }

        public abstract string GetName();
    }
}

class HumanPlayer : Player
{
    // users name
    public string Name { get; }
    public HumanPlayer(string name, string type, HashSet<int> givenNumbers) : base(type, givenNumbers)
    {
        Name = name;
    }

    public override string GetName() 
    {
        return Name;
    }
}

class ComputerPlayer : Player
{
    public ComputerPlayer(string type, HashSet<int> givenNumbers) : base(type, givenNumbers){}

    public override string GetName() 
    {
        return "Computer";
    }
}

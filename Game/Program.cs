using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;


namespace Game
{
    public class Program
    {
        // Entry point of the program
        static void Main()
        {
            // Creates game object and disposes of it after leaving the scope
            using (Game game = new Game(500, 500,"GAME"))
            {
                // running the game
                game.Run();
            }
        }
    }
}
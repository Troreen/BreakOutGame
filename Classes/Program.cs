using System;
using BreakOutGame; // This is necessary if your Game1 class is inside the BreakOutGame namespace

namespace BreakOutGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
}
using System;

namespace bytepusheremu
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new BytePusherEmu())
                game.Run();
        }
    }
}

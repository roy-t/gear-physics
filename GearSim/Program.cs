using System;

namespace GearSim
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var loop = new GameLoop())
            {
                loop.Run();
            }
        }
    }
}

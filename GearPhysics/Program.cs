using System;

namespace GearPhysics
{
    sealed class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            using (var simLoop = new SimLoop())
            {
                simLoop.Run();
            }
        }
    }
}

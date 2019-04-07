using System;

namespace Bürgerampel_Simulation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Bürgerampel_Simulation_Allgemein game = new Bürgerampel_Simulation_Allgemein())
            {
                game.Run();
            }
        }
    }
}


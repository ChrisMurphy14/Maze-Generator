//////////////////////////////////////////////////
// File:				Program.cs
// Author:				Chris Murphy
// Date Created:		06/11/2014
// Date Last Edited:	09/11/2014
//////////////////////////////////////////////////
#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

// The namespace used to contain the code for the Maze Generator project.
namespace Maze_Generator
{
#if WINDOWS || LINUX
    // The class used to contain and run the Game1 class.
    public static class Program
    {
        // Defines the project as single-threaded.
        [STAThread]

        // The entry point for the program.
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
#endif
}

using System;
using Dagger.Models;
using Dagger.Services;

namespace Dagger.Routines
{
    /// <summary>
    /// Provide a method that allows documentation to be displayed.
    /// </summary>
    public sealed class HelpRoutine : Routine
    {
        /// <summary>
        /// Write documentation to stdout.
        /// </summary>
        public override void Execute()
        {
            StringService.Colorize(ConsoleColor.Cyan, "|> ", false);
            StringService.Colorize(ConsoleColor.Gray, "build ([path])");
            StringService.Colorize(ConsoleColor.DarkGray,
                "Build Dagger project at the provided path, or build the " 
                + "Dagger project in the current directory if no path is provided.");
            Console.WriteLine();
            StringService.Colorize(ConsoleColor.Cyan, "|> ", false);
            StringService.Colorize(ConsoleColor.Gray, "new ([path])");
            StringService.Colorize(ConsoleColor.DarkGray,
                "Create a new Dagger project " 
                + "at the given path, or in the current directory if no path is provided.");
            Console.WriteLine();
            StringService.Colorize(ConsoleColor.Cyan, "|> ", false);
            StringService.Colorize(ConsoleColor.Gray, "serve ([path]) ([port])");
            StringService.Colorize(ConsoleColor.DarkGray, 
                "Create a local web server with the given [port] and host " 
                + "the Dagger project located at the given [path]. Defaults to " 
                + "port 8000 and checks the current directory for a Dagger " 
                + "project when no arguments are given.");
            Console.WriteLine();
        }
    }
}

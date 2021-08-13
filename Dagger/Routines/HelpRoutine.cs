using System;
using Dagger.Models;
using static Dagger.Services.StringService;
using static System.Console;

namespace Dagger.Routines
{
    /// <summary>
    /// Provide a method that allows documentation to be displayed.
    /// </summary>
    public sealed class HelpRoutine : Routine
    {
        private HelpTopic Topic { get; }
        
        public HelpRoutine(HelpTopic topic = HelpTopic.General)
        {
            Topic = topic;
        }
        
        /// <summary>
        /// Write documentation to stdout.
        /// </summary>
        public override void Execute()
        {
            ConsoleColor gray = ConsoleColor.Gray;
            ConsoleColor darkGray = ConsoleColor.DarkGray;
            ConsoleColor cyan = ConsoleColor.Cyan;
            
            if (Topic is HelpTopic.Build)
            {
                Colorize(cyan, "Building a project");
                WriteLine();
                Colorize(gray, "Synopsis: ");
                Colorize(cyan, "build ", false);
                Colorize(gray, "[path]");
                WriteLine();
                Colorize(gray, "Example: ");
                Colorize(cyan, "dagger build ", false);
                Colorize(gray, "./projects/mysite");
                WriteLine();
                Colorize(gray, "Options:");
                WriteLine();
                Colorize(cyan, "[path] ", false);
                Colorize(darkGray, "(default: current directory)");
                Colorize(gray, "An absolute or relative path to the project that is being built.");
            }
            else if (Topic is HelpTopic.New)
            {
                Colorize(cyan, "Creating a new project");
                WriteLine();
                Colorize(gray, "Synopsis: ");
                Colorize(cyan, "new ", false);
                Colorize(gray, "[path] ");
                WriteLine();
                Colorize(gray, "Example: ");
                Colorize(cyan, "dagger new ", false);
                Colorize(gray, "./projects/newsite");
                WriteLine();
                Colorize(gray, "Options:");
                WriteLine();
                Colorize(cyan, "[path] ", false);
                Colorize(darkGray, "(default: current directory)");
                Colorize(gray, "An absolute or relative path to the desired location of the new project. " +
                               "If the directory does not exist, it will be created.");
            }
            
            else if (Topic is HelpTopic.Serve)
            {
                Colorize(cyan, "Serving a project");
                WriteLine();
                Colorize(gray, "Synopsis: ");
                Colorize(cyan, "serve ", false);
                Colorize(gray, "[path] ", false);
                Colorize(gray, "[port]");
                WriteLine();
                Colorize(gray, "Example: ");
                Colorize(cyan, "dagger serve ", false);
                Colorize(gray, "./projects/mysite 8001");
                WriteLine();
                Colorize(gray, "Options:");
                WriteLine();
                Colorize(cyan, "[path] ", false);
                Colorize(darkGray, "(default: current directory)");
                Colorize(gray, "An absolute or relative path to the project that is being served.");
                WriteLine();
                Colorize(cyan, "[port] ", false);
                Colorize(darkGray, "(default: 8000)");
                Colorize(gray, "Represents the desired port on which to make this project viewable.");
            }
            else if (Topic is HelpTopic.General)
            {
                Colorize(cyan, "build ", false);
                Colorize(gray, "[path]");
                Colorize(cyan, "new ", false);
                Colorize(gray, "[path]");
                Colorize(cyan, "serve ", false);
                Colorize(gray, 
                    "[path] [port]");
                WriteLine();
                Colorize(gray, "View more information with ", false);
                Colorize(cyan, "help ", false);
                Colorize(gray, "<command>");
            }
        }

        public enum HelpTopic
        {
            General,
            Build,
            New,
            Serve
        }
    }
}

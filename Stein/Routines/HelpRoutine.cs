using static System.Console;
using static System.ConsoleColor;
using Stein.Models;
using static Stein.Services.StringService;

namespace Stein.Routines
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
            if (Topic is HelpTopic.Build)
            {
                Colorize("Building a project", White, true);
                WriteLine();
                Colorize("Synopsis: ", Gray, true);
                Colorize("build ", White, false);
                Colorize("[path]", Gray, true);
                WriteLine();
                Colorize("Example: ", Gray, true);
                Colorize("stein build ", White, false);
                Colorize("./projects/mysite", Gray, true);
                WriteLine();
                Colorize("Options:", Gray, true);
                WriteLine();
                Colorize("[path] ", White, false);
                Colorize("(default: current directory)", DarkGray, true);
                Colorize("An absolute or relative path to the project that is being built.", Gray, true);
            }
            else if (Topic is HelpTopic.New)
            {
                Colorize("Creating a new project", White, true);
                WriteLine();
                Colorize("Synopsis: ", Gray, true);
                Colorize("new ", White, false);
                Colorize("[path] ", Gray, true);
                WriteLine();
                Colorize("Example: ", Gray, true);
                Colorize("stein new ", White, false);
                Colorize("./projects/newsite", Gray, true);
                WriteLine();
                Colorize("Options:", Gray, true);
                WriteLine();
                Colorize("[path] ", White,  false);
                Colorize("(default: current directory)", DarkGray, true);
                Colorize("An absolute or relative path to the desired location of the new project. " +
                               "If the directory does not exist, it will be created.", Gray, true);
            }
            
            else if (Topic is HelpTopic.Serve)
            {
                Colorize("Serving a project", White, true);
                WriteLine();
                Colorize("Synopsis: ", Gray, true);
                Colorize("serve ", White, false);
                Colorize("[path] ", Gray, false);
                Colorize("[port]", Gray, true);
                WriteLine();
                Colorize("Example: ", Gray, true);
                Colorize("stein serve ", White, false);
                Colorize("./projects/mysite 8001", Gray, true);
                WriteLine();
                Colorize("Options:", Gray, true);
                WriteLine();
                Colorize("[path] ", White, false);
                Colorize("(default: current directory)", DarkGray, true);
                Colorize("An absolute or relative path to the project that is being served.", Gray, true);
                WriteLine();
                Colorize("[port] ", White, false);
                Colorize("(default: 8000)", DarkGray, true);
                Colorize("Represents the desired port on which to make this project viewable.", Gray, true);
            }
            else if (Topic is HelpTopic.General)
            {
                Colorize("build ", White, false);
                Colorize("[path]", Gray, true);
                Colorize("new ", White, false);
                Colorize("[path]", Gray, true);
                Colorize("serve ", White, false);
                Colorize("[path] [port]", Gray, true);
                WriteLine();
                Colorize("View more information with ", Gray, false);
                Colorize("help ", White, false);
                Colorize("<command>", Gray, true);
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

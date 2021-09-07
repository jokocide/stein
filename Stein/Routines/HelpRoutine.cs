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
                Colorize("Description:", Gray, true);
                Colorize("Attempt to process all resources in a given project and output the "
                    + "results to the site directory.", White, true);
                Colorize("Synopsis: ", Gray, true);
                Colorize("build ", White, false);
                Colorize("[path]", Gray, true);
                Colorize("Example: ", Gray, true);
                Colorize("stein build ", White, false);
                Colorize("./projects/mysite", Gray, true);
                Colorize("Options:", Gray, true);
                Colorize("[path] ", White, false);
                Colorize("(default: current directory)", DarkGray, true);
                Colorize("An absolute or relative path to the project that is being built.", Gray, true);
            }
            else if (Topic is HelpTopic.New)
            {
                Colorize("Description:", Gray, true);
                Colorize("Scaffold out resources and site directories, as well as a " +
                    "stein.json file in the root of the new project directory.", White, true);
                Colorize("Synopsis: ", Gray, true);
                Colorize("new ", White, false);
                Colorize("[path] ", Gray, true);
                Colorize("Example: ", Gray, true);
                Colorize("stein new ", White, false);
                Colorize("./projects/newsite", Gray, true);
                Colorize("Options:", Gray, true);
                Colorize("[path] ", White,  false);
                Colorize("(default: current directory)", DarkGray, true);
                Colorize("An absolute or relative path to the desired location of the new project. " +
                               "If the directory does not exist, it will be created.", Gray, true);
            }
            
            else if (Topic is HelpTopic.Serve)
            {
                Colorize("Description:", Gray, true);
                Colorize("Make the contents of a projects site directory available at a given port. " + 
                    "It also watches the projects resources directory for changes. When a file is changed," +
                    " renamed, created, moved or deleted, a build is triggered and the new contents of site " +
                    "will automatically be available for viewing at the previously specified port.", White, true);
                Colorize("Synopsis: ", Gray, true);
                Colorize("serve ", White, false);
                Colorize("[path] ", Gray, false);
                Colorize("[port]", Gray, true);
                Colorize("Example: ", Gray, true);
                Colorize("stein serve ", White, false);
                Colorize("./projects/mysite 8001", Gray, true);
                Colorize("Options:", Gray, true);
                Colorize("[path] ", White, false);
                Colorize("(default: current directory)", DarkGray, true);
                Colorize("An absolute or relative path to the project that is being served.", Gray, true);
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

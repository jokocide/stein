using static System.Console;
using static Stein.Services.StringService;
using Stein.Models;
using System;

namespace Stein.Routines
{
    /// <summary>
    /// Represents a Routine that can be used to display helpful information about 
    /// the command line interface.
    /// </summary>
    public sealed class HelpRoutine : Routine
    {
        /// <summary>
        /// Initializes a new instance of the HelpRoutine class with the specified HelpTopic,
        /// or HelpTopic.General when no HelpTopic is provided.
        /// </summary>
        /// <param name="topic">
        /// Represents the desired information.
        /// </param>
        public HelpRoutine(HelpTopic topic = HelpTopic.General) => Topic = topic;

        /// <summary>
        /// Display command line usage information.
        /// </summary>
        public override void Execute()
        {
            if (Topic is HelpTopic.Build)
            {
                BoldLine("DESCRIPTION");
                WriteLine("Attempt to process all resources in a given project and output the " + 
                "results to the site directory.");
                BoldLine("USAGE");
                WriteLine("stein build [PATH]");
                BoldLine("EXAMPLE");
                WriteLine("stein build ./projects/mysite");
                BoldLine("OPTIONS");
                Bold("- ");
                WriteLine("PATH");
                WriteLine("An absolute or relative path to the project that is being built. " + 
                "Defaults to the current directory.");
            }
            else if (Topic is HelpTopic.New)
            {
                BoldLine("DESCRIPTION");
                WriteLine("Scaffold the required directories and files for a new project.");
                BoldLine("USAGE");
                WriteLine("stein new [PATH]");
                BoldLine("EXAMPLE");
                WriteLine("stein new ./projects/mysite");
                BoldLine("OPTIONS");
                Bold("- ");
                WriteLine("PATH");
                WriteLine("An absolute or relative path to the desired location of the new project. " +
                               "If the directory does not exist it will be created. Defaults to the current directory.");
            }
            else if (Topic is HelpTopic.Serve)
            {
                BoldLine("DESCRIPTION");
                WriteLine("Serve a project at the given port on localhost, and watch the project for changes.");
                BoldLine("USAGE");
                WriteLine("stein serve [PATH] [PORT]");
                BoldLine("EXAMPLE");
                WriteLine("stein build ./projects/mysite");
                BoldLine("OPTIONS");
                Bold("- ");
                WriteLine("PATH");
                WriteLine("An absolute or relative path to the project that is being built. " + 
                "Defaults to the current directory.");
            }
            else if (Topic is HelpTopic.General)
            {
                BoldLine("COMMANDS");
                Bold("- ");
                WriteLine("build [PATH]");
                Bold("- ");
                WriteLine("new [PATH]");
                Bold("- ");
                WriteLine("serve [PATH] [PORT]");
                Bold("- ");
                WriteLine("help <command>");
            }
        }

        /// <summary>
        /// The supported and documented topics.
        /// </summary>
        public enum HelpTopic
        {
            General,
            Build,
            New,
            Serve
        }

        private HelpTopic Topic { get; }

        private string indentOne = "  ";
    }
}

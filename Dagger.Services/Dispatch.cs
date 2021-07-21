using System.Collections.Generic;
using System.IO;
using Dagger.Data.Models;
using Dagger.Services.Routines;

namespace Dagger.Services
{
    /// <summary>
    /// Examine the received arguments and respond with some Routine-typed object.
    /// </summary>
    /// <returns>
    /// A class instance that derives from the Routine abstract class.
    /// </returns>
    public static class Dispatch
    {
        private static int MaxTotalArgs { get; } = 3;
        private static int MaxBuildArgs { get; } = 2;

        public static Routine Evaluate(string[] arguments)
        {
            List<string> args = new List<string>(arguments);
            
            // We can return right away if too many arguments are passed in.
            if (args.Count > MaxTotalArgs) return new Help(new Message
            {
                message = "Too many arguments were received.", 
                type = Message.Type.Error
            });

            // foreach is not allowed when assigning to iterator.
            for (int count = 0; count < args.Count; count++)
            {
                args[count] = args[count].ToLower();
            }

            switch (args[0])
            {
                case "help":
                    return PipelineHelp(args);
                case "build":
                    return PipelineBuild(args);
                case "new":
                    return PipelineNew(args);
                default:
                    return new NotRecognized();
            }
        }

        private static Routine PipelineHelp(List<string> args)
        {
            // We aren't expecting any arguments after help.
            return args.Count > 1
                ? new Help(new Message()
                {
                    message = $"Dagger doesn't support help specific to the command: {args[1]}",
                    type = Message.Type.Warning
                })
                : new Help();
        }

        private static Routine PipelineBuild(List<string> args)
        {
            // First argument was build. Did they pass in a path?
            if (args.Count > MaxBuildArgs)
            {
                Help.TooManyArguments("Build");
            }
            
            return args.Count == 1 ? new Build() : PipelineBuildPath(args);
        }

        private static Routine PipelineBuildPath(List<string> args)
        {
            // First argument is 'build' so the second should be a valid path to a Dagger project.
            try
            {
                Directory.SetCurrentDirectory(args[1]);
            }
            catch (IOException)
            {
                return new Help(new Message { message = $"'{args[1]}' is not a valid path.", type = Message.Type.Error });
            }

            return Helper.CheckIsProject() ? new Build() : new Help(new Message
            {
                message = "Provide a path to a Dagger project or move to project before calling build.", 
                type = Message.Type.Error
            });
        }
        
        private static Routine PipelineNew(List<string> args)
        {
            // We only support creating a project in the current directory right now, so we should only have one argument.
            return args.Count > 1 ? Help.TooManyArguments("New") : new New();
        }
    }
}
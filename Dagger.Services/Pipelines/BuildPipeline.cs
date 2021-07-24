using System.IO;
using System.Collections.Generic;

using Dagger.Data.Models;
using Dagger.Services.Routines;

namespace Dagger.Services.Pipelines
{
    public class BuildPipeline : Pipeline
    {
        private int MaxBuildArgs { get; } = 2;
        
        public BuildPipeline(List<string> args) : base(args) { }
        
        public override Routine Execute()
        {
            if (Args.Count > MaxBuildArgs)
                return Help.TooManyArguments("Build");

            if (Args.Count == 1)
            {
                if (Helper.CheckIsProject()) 
                    return new Build();
                
                return Help.NotInDaggerProject(true);
            }
            
            return PipelineBuildPath(Args);
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
                return new Help(new Message { message = $"'{args[1]}' is not a valid path to a Dagger project.", type = Message.Type.Error });
            }

            return Helper.CheckIsProject() ? new Build() : new Help(new Message
            {
                message = "Provide a path to a Dagger project or move to project before calling build.", 
                type = Message.Type.Error
            });
        }
    }
}
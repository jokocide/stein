using System;
using System.IO;
using Dagger.Services.Routines;

namespace Dagger.Services.Pipelines
{
    /// <summary>
    /// A Pipeline to handle the Serve command.
    /// </summary>
    public class ServePipeline : Pipeline
    {
        public ServePipeline(string[] args) : base(args) { }

        public override Routine Execute()
        {
            if (Args.Length > 1) return ServePathPipeline(Args);

            return Helper.CheckIsProject(Directory.GetCurrentDirectory())
                ? new ServeRoutine()
                : HelpRoutine.NotInDaggerProject(true);
        }

        private Routine ServePathPipeline(string[] args)
        {
            if (args.Length > 2) return ServePathPortPipeline(args);
            
            if (!Helper.CheckIsProject(args[1]))
            {
                int number;
                
                if (Int32.TryParse(args[1], out number))
                {
                    return new ServeRoutine(args[1]);
                }

                return HelpRoutine.ProvidedPathIsNotProject();
            }
            
            Directory.SetCurrentDirectory(args[1]);
            return new ServeRoutine();
        }

        private Routine ServePathPortPipeline(string[] args)
        {
            if (!Helper.CheckIsProject(args[1])) return HelpRoutine.ProvidedPathIsNotProject();
            
            Directory.SetCurrentDirectory(args[1]);
            return new ServeRoutine(args[2]);
        }
    }
}
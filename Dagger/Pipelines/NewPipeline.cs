using System;
using System.IO;
using Dagger.Routines;

namespace Dagger.Pipelines
{
    /// <summary>
    /// A Pipeline to handle the New command.
    /// </summary>
    public class NewPipeline : Pipeline
    {
        public NewPipeline(string[] args) : base(args) { }
        
        public override Routine Execute()
        {
            return Args.Length > 1 ? PipelineBuildPath(Args) : new NewRoutine();
        }

        public Routine PipelineBuildPath(string[] args)
        {
            if (!Directory.Exists(args[1]))
            {
                try
                {
                    Directory.CreateDirectory(args[1]);
                }
                catch (IOException)
                {
                    return HelpRoutine.ProvidedPathIsInvalid();
                }
                catch (ArgumentException)
                {
                    return HelpRoutine.ProvidedPathIsInvalid();
                }
                catch (NotSupportedException)
                {
                    return HelpRoutine.ProvidedPathIsInvalid();
                }
            }
            
            Directory.SetCurrentDirectory(args[1]);
            return new NewRoutine();
        }
    }
}
using Stein.Routines;
using Stein.Interfaces;
using Stein.Models;
using System.IO;

namespace Stein.Services
{
    public static class SetupService
    {
        public static IExecutable Evaluate(string[] args)
        {
            if (args.Length == 0) return HelpRoutine.GetDefault;

            string firstArg = args[0].ToLower();
            return firstArg switch
            {
                "help" => Help(args),
                "build" => Build(args),
                "new" => New(args),
                "serve" => Serve(args),
                _ => new NotRecognizedRoutine(),
            };
        }

        private static int MaxHelpArgs { get; } = 2;

        private static int MaxBuildArgs { get; } = 2;

        private static int MaxNewArgs { get; } = 2;

        private static int MaxServeArgs { get; } = 3;

        private static IExecutable Help(string[] args)
        {
            if (args.Length > MaxHelpArgs)
            {
                MessageService.Log(Message.TooManyArgs());
                MessageService.Print(true);
            }

            return args.Length > 1 ? HelpTopic(args) : HelpRoutine.GetDefault;
        }

        private static IExecutable HelpTopic(string[] args)
        {
            string topic = args[1].ToLower();

            if (topic != "build" && topic != "new" && topic != "serve")
            {
                MessageService.Log(Message.CommandNotRecognized());
                MessageService.Print(true);
            }

            return topic switch
            {
                "build" => HelpRoutine.GetBuildTopic,
                "new" => HelpRoutine.GetNewTopic,
                _ => HelpRoutine.GetServeTopic
            };
        }

        private static IExecutable Build(string[] args)
        {
            if (args.Length > MaxBuildArgs)
            {
                MessageService.Log(Message.TooManyArgs());
                MessageService.Print(true);
            }

            if (args.Length > 1) return BuildPath(args);

            if (!PathService.IsProject())
            {
                MessageService.Log(Message.NotInProject(true));
                MessageService.Print(true);
            }

            return BuildRoutine.GetDefault();
        }

        private static IExecutable BuildPath(string[] args)
        {
            try
            {
                Directory.SetCurrentDirectory(args[1]);
            }
            catch (IOException)
            {
                MessageService.Log(Message.ProvidedPathIsNotProject());
                MessageService.Print(true);
            }

            if (!PathService.IsProject())
            {
                MessageService.Log(Message.NotInProject(true));
                MessageService.Print(true);
            }

            return BuildRoutine.GetDefault();
        }

        private static IExecutable New(string[] args)
        {
            if (args.Length > MaxNewArgs)
            {
                MessageService.Log(Message.TooManyArgs());
                MessageService.Print(true);
            }

            return args.Length > 1 ? NewPath(args) : NewRoutine.GetDefault;
        }

        private static IExecutable NewPath(string[] args)
        {
            if (!Directory.Exists(args[1]))
            {
                try
                {
                    Directory.CreateDirectory(args[1]);
                }
                catch (IOException)
                {
                    MessageService.Log(Message.ProvidedPathIsInvalid());
                    MessageService.Print(true);
                }
            }

            Directory.SetCurrentDirectory(args[1]);
            return NewRoutine.GetDefault;
        }

        private static IExecutable Serve(string[] args)
        {
            if (args.Length > MaxServeArgs)
            {
                MessageService.Log(Message.TooManyArgs());
                MessageService.Print(true);
            }

            if (args.Length > 1) return ServePath(args);

            if (!PathService.IsProject())
            {
                MessageService.Log(Message.NotInProject(true));
                MessageService.Print(true);
            }

            return ServeRoutine.GetDefault;
        }

        private static IExecutable ServePath(string[] args)
        {
            if (args.Length > 2) return ServePathPort(args);

            if (!PathService.IsProject(args[1]))
            {
                if (int.TryParse(args[1], out _) && (args[1].Length == 4 || args[1].Length == 5))
                {
                    return new ServeRoutine(new Configuration(), args[1]);
                }

                MessageService.Log(Message.ProvidedPathIsNotProject());
                MessageService.Print(true);
            }

            Directory.SetCurrentDirectory(args[1]);
            return ServeRoutine.GetDefault;
        }

        private static IExecutable ServePathPort(string[] args)
        {
            if (!PathService.IsProject(args[1]))
            {
                MessageService.Log(Message.ProvidedPathIsNotProject());
                MessageService.Print(true);
            }

            Directory.SetCurrentDirectory(args[1]);
            return new ServeRoutine(new Configuration(), args[2]);
        }
    }
}


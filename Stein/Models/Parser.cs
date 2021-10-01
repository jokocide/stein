using Stein.Routines;
using System.IO;

namespace Stein.Models
{
    public class Parser
    {
        public Routine Evaluate(string[] args)
        {
            if (args.Length == 0)
                return HelpRoutine.GetDefault;

            string argOne = args[0].ToLower();

            if (argOne == "help")
                return Help(args);
            if (argOne == "build")
                return Build(args);
            if (argOne == "new")
                return New(args);
            if (argOne == "serve")
                return Serve(args);

            Message.Log(Message.CommandNotRecognized());
            return null;
        }

        private int MaxHelpArgs { get; } = 2;

        private int MaxBuildArgs { get; } = 2;

        private int MaxNewArgs { get; } = 2;

        private int MaxServeArgs { get; } = 3;

        private Routine Help(string[] args)
        {
            if (args.Length > MaxHelpArgs)
            {
                Message.Log(Message.TooManyArgs());
                return null;
            }

            return args.Length > 1 ? HelpTopic(args) : HelpRoutine.GetDefault;
        }

        private Routine HelpTopic(string[] args)
        {
            string topic = args[1].ToLower();

            if (topic != "build" && topic != "new" && topic != "serve")
            {
                Message.Log(Message.CommandNotRecognized());
                return null;
            }

            return topic switch
            {
                "build" => HelpRoutine.GetBuildTopic,
                "new" => HelpRoutine.GetNewTopic,
                _ => HelpRoutine.GetServeTopic
            };
        }

        private Routine Build(string[] args)
        {
            if (args.Length > MaxBuildArgs)
            {
                Message.Log(Message.TooManyArgs());
                return null;
            }

            if (args.Length > 1) return BuildPath(args);

            if (!IsProject())
            {
                Message.Log(Message.NotInProject(true));
                return null;
            }

            Configuration config = new Configuration().GetConfig();

            if (config == null)
            {
                Message.Log(Message.InvalidJson(new("stein.json")));
                return null;
            }

            return new BuildRoutine(config);
        }

        private Routine BuildPath(string[] args)
        {
            try
            {
                Directory.SetCurrentDirectory(args[1]);
            }
            catch (IOException)
            {
                Message.Log(Message.ProvidedPathIsNotProject());
                return null;
            }

            if (!IsProject())
            {
                Message.Log(Message.NotInProject(true));
                return null;
            }

            Configuration config = new Configuration().GetConfig();

            if (config == null)
            {
                Message.Log(Message.InvalidJson(new("stein.json")));
                return null;
            }

            return new BuildRoutine(config);
        }

        private Routine New(string[] args)
        {
            if (args.Length > MaxNewArgs)
            {
                Message.Log(Message.TooManyArgs());
                return null;
            }

            return args.Length > 1 ? NewPath(args) : new NewRoutine();
        }

        private Routine NewPath(string[] args)
        {
            if (!Directory.Exists(args[1]))
            {
                try
                {
                    Directory.CreateDirectory(args[1]);
                    Directory.SetCurrentDirectory(args[1]);
                }
                catch (IOException)
                {
                    Message.Log(Message.ProvidedPathIsInvalid());
                    return null;
                }
            }

            return new NewRoutine();
        }

        private Routine Serve(string[] args)
        {
            if (args.Length > MaxServeArgs)
            {
                Message.Log(Message.TooManyArgs());
                return null;
            }

            if (args.Length > 1) return ServePath(args);

            if (!IsProject())
            {
                Message.Log(Message.NotInProject(true));
                return null;
            }

            Configuration config = new Configuration().GetConfig();

            if (config == null)
            {
                Message.Log(Message.InvalidJson(new("stein.json")));
                return null;
            }

            return new ServeRoutine(config);
        }

        private Routine ServePath(string[] args)
        {
            if (args.Length > 2) return ServePathPort(args);

            if (int.TryParse(args[1], out _) && (args[1].Length == 4 || args[1].Length == 5))
            {
                Configuration config = new Configuration().GetConfig();

                if (config == null)
                {
                    Message.Log(Message.InvalidJson(new("stein.json")));
                    return null;
                }

                return new ServeRoutine(config, args[1]);
            }

            Directory.SetCurrentDirectory(args[1]);

            if (IsProject())
            {
                Configuration config = new Configuration().GetConfig();

                if (config == null)
                {
                    Message.Log(Message.InvalidJson(new("stein.json")));
                    return null;
                }

                return new ServeRoutine(config);
            }

            if (!IsProject(args[1]))
            {
                Message.Log(Message.ProvidedPathIsNotProject());
                return null;
            }

            Message.Log(Message.CommandNotRecognized());
            return null;
        }

        private Routine ServePathPort(string[] args)
        {
            if (!IsProject(args[1]))
            {
                Message.Log(Message.ProvidedPathIsNotProject());
                return null;
            }

            Directory.SetCurrentDirectory(args[1]);

            Configuration config = new Configuration().GetConfig();

            if (config == null)
            {
                Message.Log(Message.InvalidJson(new("stein.json")));
                return null;
            }

            return new ServeRoutine(config, args[2]);
        }

        private bool IsProject(string path = null)
        {
            path ??= Directory.GetCurrentDirectory();
            return File.Exists(Path.Join(path, "stein.json"));
        }
    }
}


using Stein.Routines;
using static Stein.Routines.HelpRoutine;
using System.IO;
using Stein.Services;
using Stein.Interfaces;
using Stein.Engines;

namespace Stein.Models
{
    /// <summary>
    /// Facilitates command line argument parsing.
    /// </summary>
    public class Parser
    {
        private string[] Args { get; }

        private int MaxHelpArgs { get; } = 2;

        private int MaxBuildArgs { get; } = 2;

        private int MaxNewArgs { get; } = 2;

        private int MaxServeArgs { get; } = 3;

        /// <summary>
        /// Initializes a new instance of the Parser class that contains 
        /// the specified arguments.
        /// </summary>
        /// <param name="args">
        /// The arguments to be parsed.
        /// </param>
        public Parser(string[] args) => Args = args;

        /// <summary>
        /// Set up program execution and return a new instance of a class that
        /// derives from Routine.
        /// </summary>
        /// <remarks>Returns null for invalid arguments.</remarks>
        public Routine Evaluate()
        {
            if (Args.Length == 0)
                return new HelpRoutine();

            string argOne = Args[0].ToLower();

            if (argOne == "help")
                return CheckHelp();
            if (argOne == "build")
                return CheckBuild();
            if (argOne == "new")
                return CheckNew();
            if (argOne == "serve")
                return CheckServe();

            Message.Log(Message.CommandNotRecognized());
            return null;
        }

        private Routine CheckHelp()
        {
            if (Args.Length > MaxHelpArgs)
            {
                Message.Log(Message.TooManyArgs());
                return null;
            }

            return Args.Length > 1 ? CheckHelpTopic() : new HelpRoutine();
        }

        private Routine CheckHelpTopic()
        {
            string topic = Args[1].ToLower();

            if (topic != "build" && topic != "new" && topic != "serve")
            {
                Message.Log(Message.CommandNotRecognized());
                return null;
            }

            return topic switch
            {
                "build" => new HelpRoutine(HelpTopic.Build),
                "new" => new HelpRoutine(HelpTopic.New),
                _ => new HelpRoutine(HelpTopic.Serve)
            };
        }

        private Routine CheckBuild()
        {
            if (Args.Length > MaxBuildArgs)
            {
                Message.Log(Message.TooManyArgs());
                return null;
            }

            if (Args.Length > 1) return CheckBuildPath();

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

            IEngine engine = null;

            if (config.Engine == "hbs")
                engine = new HandlebarsEngine();

            if (engine == null)
            {
                Message.Log(Message.NoEngine());
                return null;
            }

            return new BuildRoutine(config, engine);
        }

        private Routine CheckBuildPath()
        {
            try
            {
                Directory.SetCurrentDirectory(Args[1]);
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

            IEngine engine = null;

            if (config.Engine == "hbs")
                engine = new HandlebarsEngine();

            if (engine == null)
            {
                Message.Log(Message.NoEngine());
                return null;
            }

            return new BuildRoutine(config, engine);
        }

        private Routine CheckNew()
        {
            if (Args.Length > MaxNewArgs)
            {
                Message.Log(Message.TooManyArgs());
                return null;
            }

            if (Args.Length > 1) return CheckNewPath();

            if (IsProject())
            {
                Message.Log(Message.ProjectAlreadyExists());
                return null;
            }

            string serializedConfiguration = new JsonService().Serialize(new Configuration());
            return Args.Length > 1 ? CheckNewPath() : new NewRoutine(serializedConfiguration);
        }

        private Routine CheckNewPath()
        {
            try
            {
                Directory.CreateDirectory(Args[1]);
                Directory.SetCurrentDirectory(Args[1]);
            }
            catch (IOException)
            {
                Message.Log(Message.ProvidedPathIsInvalid());
                return null;
            }

            if (IsProject())
            {
                Message.Log(Message.ProjectAlreadyExists());
                return null;
            }

            string serializedConfiguration = new JsonService().Serialize(new Configuration());
            return new NewRoutine(serializedConfiguration);
        }

        private Routine CheckServe()
        {
            if (Args.Length > MaxServeArgs)
            {
                Message.Log(Message.TooManyArgs());
                return null;
            }

            if (Args.Length > 1) return CheckServePath();

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

            IEngine engine = null;

            if (config.Engine == "hbs")
                engine = new HandlebarsEngine();

            if (engine == null)
            {
                Message.Log(Message.NoEngine());
                return null;
            }

            return new ServeRoutine(config, engine);
        }

        private Routine CheckServePath()
        {
            if (Args.Length > 2) return CheckServePathPort();

            if (int.TryParse(Args[1], out _) && (Args[1].Length == 4 || Args[1].Length == 5))
            {
                Configuration config = new Configuration().GetConfig();

                if (config == null)
                {
                    Message.Log(Message.InvalidJson(new("stein.json")));
                    return null;
                }

                IEngine engine = null;

                if (config.Engine == "hbs")
                    engine = new HandlebarsEngine();

                if (engine == null)
                {
                    Message.Log(Message.NoEngine());
                    return null;
                }

                return new ServeRoutine(config, engine, Args[1]);
            }

            Directory.SetCurrentDirectory(Args[1]);

            if (IsProject())
            {
                Configuration config = new Configuration().GetConfig();

                if (config == null)
                {
                    Message.Log(Message.InvalidJson(new("stein.json")));
                    return null;
                }

                IEngine engine = null;

                if (config.Engine == "hbs")
                    engine = new HandlebarsEngine();

                if (engine == null)
                {
                    Message.Log(Message.NoEngine());
                    return null;
                }

                return new ServeRoutine(config, engine);
            }

            if (!IsProject(Args[1]))
            {
                Message.Log(Message.ProvidedPathIsNotProject());
                return null;
            }

            Message.Log(Message.CommandNotRecognized());
            return null;
        }

        private Routine CheckServePathPort()
        {
            if (!IsProject(Args[1]))
            {
                Message.Log(Message.ProvidedPathIsNotProject());
                return null;
            }

            Directory.SetCurrentDirectory(Args[1]);

            Configuration config = new Configuration().GetConfig();

            if (config == null)
            {
                Message.Log(Message.InvalidJson(new("stein.json")));
                return null;
            }

            IEngine engine = null;

            if (config.Engine == "hbs")
                engine = new HandlebarsEngine();

            if (engine == null)
            {
                Message.Log(Message.NoEngine());
                return null;
            }

            return new ServeRoutine(config, engine, Args[2]);
        }

        private bool IsProject(string path = null)
        {
            path ??= Directory.GetCurrentDirectory();
            return File.Exists(Path.Join(path, "stein.json"));
        }
    }
}


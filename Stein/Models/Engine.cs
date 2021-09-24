using Stein.Engines;
using Stein.Interfaces;

namespace Stein.Models
{
    public abstract class Engine
    {
        public static IEngine GetEngine(Configuration config)
        {
            IEngine engine;

            switch (config.Engine)
            {
                case "handlebars":
                    engine = new HandlebarsEngine();
                    break;
                default:
                    return null;
            }

            return engine;
        }
    }
}

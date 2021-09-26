using Stein.Engines;
using Stein.Interfaces;

namespace Stein.Models
{
    public abstract class Engine
    {
        public static IEngine GetEngine(Configuration config)
        {
            return config.Engine switch
            {
                "handlebars" => new HandlebarsEngine(),
                _ => null
            };
        }
    }
}

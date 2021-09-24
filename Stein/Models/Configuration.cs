namespace Stein.Models
{
    public class Configuration
    {
        public bool SilenceWarnings { get; set; } = false;

        public string Engine { get; } = "handlebars";
    }
}

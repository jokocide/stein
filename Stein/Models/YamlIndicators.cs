namespace Stein.Models
{
    public class YamlIndicators
    {
        public Indices(string text)
        {
            OpenBlock = text.IndexOf("---");
            CloseBlock = rawFile.IndexOf("---", 2);

            int firstStart = text.IndexOf("---", StringComparison.Ordinal);
            int firstEnd = firstStart + 3;
            int secondStart = text.IndexOf("---", firstEnd, StringComparison.Ordinal);
            if (secondStart == -1) return (0, 0, 0, 0);
            int secondEnd = secondStart + 3;
            return (firstStart, firstEnd, secondStart, secondEnd);
        }

        public int FirstStart { get; }

        public int FirstEnd { get; }

        public int SecondStart { get; }

        public int SecondEnd { get; }

        public bool NoYaml { get; } => OpenBlock == -1 || CloseBlock == -1 || CloseBlock - OpenBlock <= 5;

        private int OpenBlock { get; }

        private int CloseBlock { get; }
    }
}
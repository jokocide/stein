using System;

namespace Stein.Models
{
    public class YamlIndicators
    {
        public YamlIndicators(string text)
        {
            FirstStart = text.IndexOf("---", StringComparison.Ordinal);
            FirstEnd = FirstStart + 3;
            SecondStart = text.IndexOf("---", FirstEnd, StringComparison.Ordinal);
            SecondEnd = SecondStart + 3;
        }

        public (int, int, int, int) Indices => (FirstStart, FirstEnd, SecondStart, SecondEnd);

        public bool NoYaml => FirstStart == -1 || SecondStart == -1 || SecondStart - FirstStart <= 5;

        public int FirstStart { get; }

        public int FirstEnd { get; }

        public int SecondStart { get; }

        public int SecondEnd { get; }
    }
}
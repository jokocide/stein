using System;

namespace Stein.Models
{
    /// <summary>
    /// Represents the location of YAML content within a file.
    /// </summary>
    public class YamlIndicators
    {
        /// <summary>
        /// Initializes a new instance of YamlIndicators on text.
        /// </summary>
        public YamlIndicators(string text)
        {
            FirstStart = text.IndexOf("---", StringComparison.Ordinal);
            FirstEnd = FirstStart + 3;
            SecondStart = text.IndexOf("---", FirstEnd, StringComparison.Ordinal);
            SecondEnd = SecondStart + 3;
        }

        /// <summary>
        /// The starting and ending location of both frontmatter indicators. (---)
        /// </summary>
        public (int, int, int, int) Indices => (FirstStart, FirstEnd, SecondStart, SecondEnd);

        /// <summary>
        /// Represents the presence of YAML within the text passed in during object initialization. If YAML
        /// is not found, this will be true.
        /// </summary>
        public bool NoYaml => FirstStart == -1 || SecondStart == -1 || SecondStart - FirstStart <= 5;

        /// <summary>
        /// The index of the first hyphen of the first indicator.
        /// </summary>
        public int FirstStart { get; }

        /// <summary>
        /// The index of the last hyphen of the first indicator.
        /// </summary>
        /// <value></value>
        public int FirstEnd { get; }

        /// <summary>
        /// The index of the first hyphen of the second indicator.
        /// </summary>
        public int SecondStart { get; }

        /// <summary>
        /// The index of the last hyphen of the second indicator.
        /// </summary>
        /// <value></value>
        public int SecondEnd { get; }
    }
}
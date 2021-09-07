using Xunit;
using System.Collections.Generic;
using System;

namespace Stein.Services.Tests
{
    public class YamlServiceTests
    {
        /// <summary>
        /// Test data to be injected into GetIndicesTest via MemberData attribute.
        /// </summary>
        public static IEnumerable<object[]> Indices =>
            new List<object[]>
            {
                new object[] { "# Hello World", (0, 0, 0, 0)},
                new object[] { "---one: value one\ntwo: value two---", (0, 3, 32, 35)},
                new object[] { "---Key One: Value One\nKey Two: Getting Started: The Guide---", (0, 3, 57, 60)}
            };

        /// <summary>
        /// GetIndices should return a (int, int, int, int) tuple that indicates
        /// the location of YAML frontmatter within a string, defaults to (0, 0, 0, 0)
        /// to indicate a problem with the format of the frontmatter, or that frontmatter
        /// does not exist in the string.
        /// <param name="text">The source string.</param>
        /// <param name="expectedResult">The expected tuple result.</param>
        [Theory]
        [MemberData(nameof(Indices))]
        public void GetIndicesTest(string text, (int, int, int, int) expectedResult)
        {
            (int, int, int, int) result = YamlService.GetIndices(text);
            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Deserialize should return a dictionary by splitting up the string line by line at the 
        /// first occurance of a given delimiter, which is ':' by default, and trimming all existing 
        /// white space around the key/value pairs.
        /// </summary>
        [Fact()]
        public void DeserializeTest()
        {
            string text = $"keyOne: Value for key one{Environment.NewLine}keyTwo: and value for key two is here.";
            Dictionary<string, string> result = YamlService.Deserialize(text);

            Assert.Equal("Value for key one", result["keyOne"]);
            Assert.Equal("and value for key two is here.", result["keyTwo"]);
        }
    }
}
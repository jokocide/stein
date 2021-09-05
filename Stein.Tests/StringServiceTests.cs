using Xunit;
using System;
using Stein.Services;
using System.IO;

namespace Stein.Tests
{
    public class StringServiceTests
    {
        /// <summary>
        /// Assert that Slice returns the correct portion of text. The character
        /// at startIndex should not be included in the output, while the
        /// character at endIndex should be included.
        /// </summary>
        /// <param name="text">The source text.</param>
        /// <param name="startIndex">The character to start capturing input on.</param>
        /// <param name="endIndex">The desired final character.</param>
        /// <param name="expectedResult">The string that we expect to be returned.</param>
        [Theory]
        [InlineData("abcdThis is the way.22", 4, 20, "This is the way.")]
        [InlineData("   hello! ", 3, 9, "hello!")]
        [InlineData("Seven", 2, 5, "ven")]
        public void Slice(string text, int startIndex, int endIndex, string expectedResult)
        {
            string result = StringService.Slice(startIndex, endIndex, text);
            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Assert that Colorize correctly prints out text and resets the
        /// ConsoleColor back to default.
        /// </summary>
        [Fact]
        public void Colorize()
        {
            string text = "Bonk bonk bonk";
            ConsoleColor color = ConsoleColor.DarkBlue;

            StringWriter writer = new StringWriter();
            Console.SetOut(writer);

            StringService.Colorize(text, color, false);

            string result = writer.ToString();
            Assert.True(Console.ForegroundColor != ConsoleColor.DarkBlue && result == "Bonk  bonk");
        }
    }
}
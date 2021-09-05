using Xunit;
using System;
using System.IO;

namespace Stein.Services.Tests
{
    public class StringServiceTests
    {
        /// <summary>
        /// Slice should return a portion of text between the given indices, not including startIndex.
        /// </summary>
        /// <param name="text">
        /// The source text.
        /// </param>
        /// <param name="startIndex">
        /// The character to start capturing input on.
        /// </param>
        /// <param name="endIndex">
        /// The desired final character.
        /// </param>
        /// <param name="expectedResult">
        /// The string that we expect to be returned.
        /// </param>
        [Theory()]
        [InlineData("abcdThis is the way.22", 4, 20, "This is the way.")]
        [InlineData("   hello! ", 3, 9, "hello!")]
        [InlineData("Seven", 2, 5, "ven")]
        public void SliceTest(string text, int startIndex, int endIndex, string expectedResult)
        {
            string result = StringService.Slice(startIndex, endIndex, text);
            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Colorize should print out text and always reset the ConsoleColor.Foreground
        /// property back to default via Console.ResetColor().
        /// </summary>
        [Fact()]
        public void ColorizeTest()
        {
            string text = "Bonk bonk bonk";
            ConsoleColor color = ConsoleColor.DarkBlue;

            StringWriter writer = new StringWriter();
            Console.SetOut(writer);

            StringService.Colorize(text, color, false);

            string result = writer.ToString();
            Assert.True(Console.ForegroundColor != ConsoleColor.DarkBlue && result == "Bonk bonk bonk");
        }

        /// <summary>
        /// Slugify should trim whitespace around a string before converting any
        /// remaining whitespace characters in the middle of the string to hypens,
        /// and then return that string in lowercase.
        /// </summary>
        /// <param name="testString">
        /// The source string.
        /// </param>
        /// <param name="expectedResult">
        /// The expected slug result.
        /// </param>
        [Theory()]
        [InlineData("Master Chief", "master-chief")]
        [InlineData("Arbiter", "arbiter")]
        [InlineData("Cortana ", "cortana")]
        [InlineData(" uhhh hmmm ", "uhhh-hmmm")]
        public void SlugifyTest(string testString, string expectedResult)
        {
            string result = StringService.Slugify(testString);
            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Camelize should remove all whitespace in the string and capitalize each character
        /// that comes after the whitespace, the very first character in the string should
        /// never be changed.
        /// </summary>
        /// <param name="text">
        /// The source string.
        /// </param>
        /// <param name="expectedResult">
        /// The expected camel case string.
        /// </param>
        [Theory()]
        [InlineData("Alabama alaska", "AlabamaAlaska")]
        [InlineData("arizona california Arkansas ", "arizonaCaliforniaArkansas")]
        [InlineData(" Colorado Delaware florida:", "ColoradoDelawareFlorida:")]
        public void CamelizeTest(string text, string expectedResult)
        {
            string result = StringService.Camelize(text);
            Assert.Equal(expectedResult, result);
        }
    }
}
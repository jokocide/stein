using Xunit;
using System;
using System.IO;

namespace Stein.Services.Tests
{
    public class StringServiceTests
    {
        [Theory()]
        [InlineData("This is a test.", 10, 14, "test")]
        [InlineData("   hello! ", 3, 9, "hello!")]
        [InlineData("Seven", 2, 5, "ven")]
        public void SliceTest(string text, int startIndex, int endIndex, string expectedResult)
        {
            string result = StringService.Slice(startIndex, endIndex, text);
            Assert.Equal(expectedResult, result);
        }

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

        [Theory()]
        [InlineData("Round Trip", "round-trip")]
        [InlineData("orbital", "orbital")]
        [InlineData("Mimic ", "mimic")]
        [InlineData("question feed", "question-feed")]
        public void SlugifyTest(string testString, string expectedResult)
        {
            string result = StringService.Slugify(testString);
            Assert.Equal(expectedResult, result);
        }

        [Theory()]
        [InlineData("Alabama alaska", "alabamaAlaska")]
        [InlineData("arizona california Arkansas ", "arizonaCaliforniaArkansas")]
        [InlineData(" Colorado Delaware florida:", "coloradoDelawareFlorida:")]
        public void CamelizeTest(string text, string expectedResult)
        {
            string result = StringService.Camelize(text);
            Assert.Equal(expectedResult, result);
        }

        [Theory()]
        [InlineData("Texas Arizona", "TexasArizona")]
        [InlineData("missouri mississippi", "MissouriMississippi")]
        public void PascalizeTest(string text, string expectedResult)
        {
            string result = StringService.Pascalize(text);
            Assert.Equal(expectedResult, result);
        }
    }
}
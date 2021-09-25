using Xunit;
using System.Collections.Generic;
using System;

namespace Stein.Services.Tests
{
    public class YamlServiceTests
    {
        public static IEnumerable<object[]> Indices =>
            new List<object[]>
            {
                new object[] { "# Hello World", (0, 0, 0, 0)},
                new object[] { "---one: value one\ntwo: value two---", (0, 3, 32, 35)},
                new object[] { "---Key One: Value One\nKey Two: Getting Started: The Guide---", (0, 3, 57, 60)}
            };

        //[Theory]
        //[MemberData(nameof(Indices))]
        //public void GetIndicesTest(string text, (int, int, int, int) expectedResult)
        //{
        //    (int, int, int, int) result = new YamlService().Deserialize(text);
        //    Assert.Equal(expectedResult, result);
        //}

        [Fact()]
        public void DeserializeTest()
        {
            string text = $"keyOne: Value for key one{Environment.NewLine}keyTwo: and value for key two is here.";
            Dictionary<string, string> result = new YamlService().Deserialize(text);

            Assert.Equal("Value for key one", result["keyOne"]);
            Assert.Equal("and value for key two is here.", result["keyTwo"]);
        }
    }
}
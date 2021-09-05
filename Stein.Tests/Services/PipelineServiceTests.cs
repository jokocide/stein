using Xunit;
using System;
using System.Collections.Generic;
using Stein.Models;
using Stein.Routines;

namespace Stein.Services.Tests
{
    public class PipelineServiceTests
    {
        /// <summary>
        /// Test data to be injected into EvaluateTest via MemberData attribute.
        /// </summary>
        public static IEnumerable<object[]> Pipelines =>
            new List<object[]>
            {
                new object[] { new string[] { "build" }, new BuildRoutine() },
            };

        [Theory()]
        [MemberData(nameof(Pipelines))]
        public void EvaluateTest(string[] arguments, Routine expectedRoutine)
        {
            Routine routine = PipelineService.Evaluate(arguments);
            Assert.Equal(expectedRoutine, routine);
        }
    }
}
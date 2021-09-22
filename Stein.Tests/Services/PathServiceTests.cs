using System.Collections.Generic;
using Xunit;
using System.IO;

namespace Stein.Services.Tests
{
    public class PathServiceTests
    {
        public static IEnumerable<object[]> OutputFiles =>
            new List<object[]>
            {
                new object[] { new FileInfo(Path.Join("pages/index.html")), $"site{Path.DirectorySeparatorChar}index.html"},
                new object[] { new FileInfo(Path.Join("pages/about.html")), $"about{Path.DirectorySeparatorChar}index.html"},
                new object[] { new FileInfo(Path.Join("posts/hello-world.html")), $"hello-world{Path.DirectorySeparatorChar}index.html"},
            };

        public static IEnumerable<object[]> IterableFiles =>
            new List<object[]>
            {
                new object[] { new FileInfo(Path.Join("posts/hello-world.html")), "hello-world"},
                new object[] { new FileInfo(Path.Join("posts/testing.html")), "testing"},
            };

        [Fact()]
        public void SynchronizeTest()
        {
            string testDirOne = Path.Join(Directory.GetCurrentDirectory(), "testDirOne");
            string subDir = Path.Join(Directory.GetCurrentDirectory(), "testDirOne", "subDir");
            Directory.CreateDirectory(subDir);

            string testFile = Path.Join(subDir, "info.txt");
            File.WriteAllText(testFile, "Test data");

            string testDirTwo = Path.Join(Directory.GetCurrentDirectory(), "testDirTwo");
            PathService.Synchronize(testDirOne, testDirTwo, true);

            string expectedFileLocation = Path.Join(testDirTwo, "subDir", "info.txt");
            Assert.True(File.Exists(expectedFileLocation));

            Directory.Delete(testDirOne, true);
            Directory.Delete(testDirTwo, true);
        }

        [Fact()]
        public void IsProjectFalseTest()
        {
            string cd = Directory.GetCurrentDirectory();
            Assert.False(PathService.IsProject(cd));
        }

        [Fact()]
        public void IsProjectTrueTest()
        {
            string cd = Directory.GetCurrentDirectory();
            string fileLocation = Path.Join(cd, "stein.json");

            FileStream fileStream = File.Create(fileLocation);
            fileStream.Close();

            Assert.True(PathService.IsProject(Path.Join(cd)));

            File.Delete(fileLocation);
        }

        [Theory()]
        [MemberData(nameof(OutputFiles))]
        public void GetOutputPathTest(FileInfo fileInfo, string expectedResult)
        {
            string result = PathService.GetOutputPath(fileInfo);
            string fileName = Path.GetFileName(result);
            string? directory = Path.GetFileName(Path.GetDirectoryName(result));

            Assert.Equal(expectedResult, Path.Join(directory, fileName));
        }

        [Theory()]
        [MemberData(nameof(IterableFiles))]
        public void GetIterablePathTest(FileInfo fileInfo, string expectedResult)
        {
            string result = PathService.GetIterablePath(fileInfo);
            string? directory = Path.GetFileName(Path.GetDirectoryName(result));

            Assert.Equal(expectedResult, directory);
        }
    }
}
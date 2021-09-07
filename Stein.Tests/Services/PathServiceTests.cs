using System.Collections.Generic;
using Xunit;
using System.IO;

namespace Stein.Services.Tests
{
    public class PathServiceTests
    {
        /// <summary>
        /// Test data to be injected into GetOutputPath via MemberData attribute.
        /// </summary>
        public static IEnumerable<object[]> OutputFiles =>
            new List<object[]>
            {
                new object[] { new FileInfo(Path.Join("pages/index.html")), $"site{Path.DirectorySeparatorChar}index.html"},
                new object[] { new FileInfo(Path.Join("pages/about.html")), $"about{Path.DirectorySeparatorChar}index.html"},
                new object[] { new FileInfo(Path.Join("posts/hello-world.html")), $"hello-world{Path.DirectorySeparatorChar}index.html"},
            };

        /// <summary>
        /// Test data to be injected into GetIterablePath via MemberData attribute.
        /// </summary>
        public static IEnumerable<object[]> IterableFiles =>
            new List<object[]>
            {
                new object[] { new FileInfo(Path.Join("posts/hello-world.html")), "hello-world"},
                new object[] { new FileInfo(Path.Join("posts/testing.html")), "testing"},
            };

        [Fact()]
        public void SynchronizeTest()
        {
            // Setup
            string testDirOne = Path.Join(Directory.GetCurrentDirectory(), "testDirOne");
            string subDir = Path.Join(Directory.GetCurrentDirectory(), "testDirOne", "subDir");
            Directory.CreateDirectory(subDir);

            string testFile = Path.Join(subDir, "info.txt");
            File.WriteAllText(testFile, "Test data");

            // Method is tested
            string testDirTwo = Path.Join(Directory.GetCurrentDirectory(), "testDirTwo");
            PathService.Synchronize(testDirOne, testDirTwo, true);

            // The files within the subdirectory should exist to confirm that the
            // recursive behavior is working as intended.
            string expectedFileLocation = Path.Join(testDirTwo, "subDir", "info.txt");
            Assert.True(File.Exists(expectedFileLocation));

            // Cleanup
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

            // Setup
            FileStream fileStream = File.Create(fileLocation);
            fileStream.Close();

            // Method is tested
            Assert.True(PathService.IsProject(Path.Join(cd)));

            // Cleanup
            File.Delete(fileLocation);
        }

        [Theory()]
        [MemberData(nameof(OutputFiles))]
        public void GetOutputPathTest(FileInfo fileInfo, string expectedResult)
        {
            string result = PathService.GetOutputPath(fileInfo);
            string fileName = Path.GetFileName(result);
            string directory = Path.GetFileName(Path.GetDirectoryName(result));
            Assert.Equal(expectedResult, Path.Join(directory, fileName));
        }

        [Theory()]
        [MemberData(nameof(IterableFiles))]
        public void GetIterablePathTest(FileInfo fileInfo, string expectedResult)
        {
            string result = PathService.GetIterablePath(fileInfo);
            string directory = Path.GetFileName(Path.GetDirectoryName(result));
            Assert.Equal(expectedResult, directory);
        }
    }
}
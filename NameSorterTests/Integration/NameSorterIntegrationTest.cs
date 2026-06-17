using NameSorter.Application.Services;
using NameSorter.Application.Ports.Out;
using NameSorter.Infrastructure;

namespace NameSorterTests.Integration;

public class NameSorterIntegrationTest
{
    [Fact]
    [Trait("Category", "Integration")]
    public void OrderNamesInFile_WritesSortedOutputAndInvalidNamesFile()
    {
        var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDirectory);

        try
        {
            // Arrange
            var inputFile = Path.Combine(tempDirectory, "unsorted-names-list.txt");
            var outputFile = Path.Combine(tempDirectory, "sorted-names-list.txt");
            var invalidFile = Path.Combine(tempDirectory, "invalid-names-list.txt");

            File.WriteAllLines(inputFile, new[]
            {
                "Alice Johnson",
                "Bob",
                "Eve Ann Smith",
                "John Jacob Jingleheimer James Schmidt"
            });

            var repository = new PersonNameFileRepository();
            var outputWriter = new TestOutputWriter();
            var orderService = new OrderNamesService(repository, outputWriter);

            // Act
            orderService.OrderNamesInFile(inputFile, outputFile);

            // Assert
            Assert.True(File.Exists(outputFile));
            Assert.True(File.Exists(invalidFile));

            var sortedLines = File.ReadAllLines(outputFile);
            var invalidLines = File.ReadAllLines(invalidFile);

            Assert.Equal(new[] { "Alice Johnson", "Eve Ann Smith" }, sortedLines);
            Assert.Equal(2, invalidLines.Length);
            Assert.Contains("Bob", invalidLines);
            Assert.Contains("John Jacob Jingleheimer James Schmidt", invalidLines);
            Assert.DoesNotContain(outputWriter.Lines, line => line.Contains("Bob") || line.Contains("John Jacob"));
        }
        finally
        {
            Directory.Delete(tempDirectory, true);
        }
    }

    private sealed class TestOutputWriter : IOutputWriter
    {
        public TestOutputWriter()
        {
            Lines = [];
        }

        public List<string> Lines { get; }

        public void WriteLine(string line)
        {
            Lines.Add(line);
        }
    }
}

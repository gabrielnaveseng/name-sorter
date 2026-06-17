using Moq;
using NameSorter.Domain;
using NameSorter.Infrastructure;

namespace NameSorterTests.Infrastructure
{
    public class PersonNameFileRepositoryTests
    {
        [Fact]
        public void ReadPersonNamesFromFile_ReturnsCorrectNames()
        {
            // Arrange
            var repository = new Mock<PersonNameFileRepository>() { CallBase = true };

            repository.Setup(r => r.ReadAllLines(It.IsAny<string>())).Returns(new string[]
            {
                "John Doe",
                "Jane Smith"
            });

            const string FilePath = "dummyPath";
            
            // Act
            var result = repository.Object.ReadPersonNamesFromFile(FilePath).ToList();
            
            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("John", result[0].GivenNames.First());
            Assert.Equal("Doe", result[0].LastName);
            Assert.Equal("Jane", result[1].GivenNames.First());
            Assert.Equal("Smith", result[1].LastName);

            repository.Verify(r => r.ReadAllLines(FilePath), Times.Once);
        }

        [Fact]
        public void WritePersonNamesToFile_WritesCorrectLines()
        {
            // Arrange
            var repository = new Mock<PersonNameFileRepository>() { CallBase = true };

            var personNames = new List<PersonName>
            {
                new(["John"], "Doe"),
                new(["Jane"], "Smith")
            };

            const string FilePath = "dummyPath";

            repository.Setup(r => r.WriteAllLines(It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).Verifiable();

            // Act
            repository.Object.WritePersonNamesToFile(personNames, FilePath);

            // Assert
            repository.Verify(r => r.WriteAllLines(FilePath, It.Is<IEnumerable<string>>(lines =>
                lines.SequenceEqual(new[] { "John Doe", "Jane Smith" }))), Times.Once);
        }
    }
}
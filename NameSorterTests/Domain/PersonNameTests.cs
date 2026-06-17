using NameSorter.Domain;

namespace NameSorterTests.Domain
{
    public class PersonNameTests
    {
        [Fact]
        public void FullName_ReturnsCorrectFormat()
        {
            // Arrange
            var personName = new PersonName(["John"], "Doe");

            // Act
            var fullName = personName.FullName;

            // Assert
            Assert.Equal("John Doe", fullName);
        }
        
        [Fact]
        public void FullName_ReturnsCorrectFormat_WithMultipleGivenNames()
        {
            // Arrange
            var personName = new PersonName(["John", "Michael"], "Doe");

            // Act
            var fullName = personName.FullName;

            // Assert
            Assert.Equal("John Michael Doe", fullName);
        }
    }
}
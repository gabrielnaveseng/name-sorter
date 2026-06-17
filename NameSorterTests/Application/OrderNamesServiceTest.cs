using System;
using Moq;
using NameSorter.Application.Ports.Out;
using NameSorter.Application.Services;
using NameSorter.Domain;

namespace NameSorterTests.Application;
public class OrderNamesServiceTest
{
    private readonly OrderNamesService orderNamesService;
    private readonly Mock<IPersonNameRepository> mockFileAccessRepository;
    private readonly Mock<IOutputWriter> mockOutputWriter;

    public OrderNamesServiceTest()
    {
        mockFileAccessRepository = new Mock<IPersonNameRepository>();
        mockOutputWriter = new Mock<IOutputWriter>();
        orderNamesService = new OrderNamesService(mockFileAccessRepository.Object, mockOutputWriter.Object);
    }    

    [Fact]
    public void OrderNamesInFile_ShouldOrderNamesCorrectly()
    {
        // Arrange
        var inputPath = "input.txt";
        var outputPath = "output.txt";
        
        var personNames = new List<PersonName>
        {
            new ( ["John"], "Doe"),
            new ( ["Jane"], "Smith"),
            new ( ["Alice"], "Doe")
        };
        
        mockFileAccessRepository.Setup(repo => repo.ReadPersonNamesFromFile(inputPath)).Returns(personNames);
        
        var errorFilePath = "invalid-names-list.txt";
        // Act
        orderNamesService.OrderNamesInFile(inputPath, outputPath);
        
        // Assert
        mockFileAccessRepository.Verify(repo => repo.WritePersonNamesToFile(
            It.Is<IEnumerable<PersonName>>(names => names.Any() &&
                                                  names.ElementAt(0).FullName == "Alice Doe" &&
                                                  names.ElementAt(1).FullName == "John Doe" &&
                                                  names.ElementAt(2).FullName == "Jane Smith"),
            outputPath), Times.Once);
        
        mockFileAccessRepository.Verify(repo => repo.WritePersonNamesToFile(
            It.Is<IEnumerable<PersonName>>(names => !names.Any()),
            errorFilePath), Times.Once);
        
        mockOutputWriter.Verify(writer => writer.WriteLine("Alice Doe"), Times.Once);
        mockOutputWriter.Verify(writer => writer.WriteLine("John Doe"), Times.Once);
        mockOutputWriter.Verify(writer => writer.WriteLine("Jane Smith"), Times.Once);
    }

    [Fact]
    public void OrderNamesInFile_ShouldHandleEmptyFile()
    {
        // Arrange
        var inputPath = "empty.txt";
        var outputPath = "output.txt";
        
        mockFileAccessRepository.Setup(repo => repo.ReadPersonNamesFromFile(inputPath)).Returns(new List<PersonName>());

        // Act
        orderNamesService.OrderNamesInFile(inputPath, outputPath);

        // Assert
        mockFileAccessRepository.Verify(repo => repo.WritePersonNamesToFile(
            It.Is<IEnumerable<PersonName>>(names => !names.Any()),
            outputPath), Times.Once);

        mockOutputWriter.Verify(writer => writer.WriteLine(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void OrderNamesInFile_ShouldHandleSingleName()
    {
        // Arrange
        var inputPath = "single.txt";
        var outputPath = "output.txt";
        
        var personNames = new List<PersonName>
        {
            new ( ["John"], "Doe")
        };
        
        mockFileAccessRepository.Setup(repo => repo.ReadPersonNamesFromFile(inputPath)).Returns(personNames);
        
        // Act
        orderNamesService.OrderNamesInFile(inputPath, outputPath);
        
        // Assert
        mockFileAccessRepository.Verify(repo => repo.WritePersonNamesToFile(
            It.Is<IEnumerable<PersonName>>(names => names.Any() && names.ElementAt(0).FullName == "John Doe"),
            outputPath), Times.Once);
        
        mockOutputWriter.Verify(writer => writer.WriteLine("John Doe"), Times.Once);
    }

    [Fact]
    public void OrderNamesInFile_ShouldHandleDuplicateNames()
    {
        // Arrange
        var inputPath = "duplicates.txt";
        var outputPath = "output.txt";
        
        var personNames = new List<PersonName>
        {
            new ( ["John"], "Doe"),
            new ( ["John"], "Doe"),
            new ( ["Jane"], "Smith")
        };
        
        mockFileAccessRepository.Setup(repo => repo.ReadPersonNamesFromFile(inputPath)).Returns(personNames);
        
        // Act
        orderNamesService.OrderNamesInFile(inputPath, outputPath);
        
        // Assert
        mockFileAccessRepository.Verify(repo => repo.WritePersonNamesToFile(
            It.Is<IEnumerable<PersonName>>(names => names.Any() && names.ElementAt(0).FullName == "John Doe" &&
                                                  names.ElementAt(1).FullName == "John Doe" &&
                                                  names.ElementAt(2).FullName == "Jane Smith"),
            outputPath), Times.Once);
        
        mockOutputWriter.Verify(writer => writer.WriteLine("John Doe"), Times.Exactly(2));
        mockOutputWriter.Verify(writer => writer.WriteLine("Jane Smith"), Times.Once);
    }

    [Fact]
    public void OrderNamesInFile_ShouldHandleNamesWithMultipleGivenNames()
    {
        // Arrange
        var inputPath = "multiple_given_names.txt";
        var outputPath = "output.txt";
        
        var personNames = new List<PersonName>
        {
            new ( ["John", "Michael"], "Doe"),
            new ( ["Jane", "Ann"], "Smith"),
            new ( ["Alice", "Marie"], "Doe")
        };
        
        mockFileAccessRepository.Setup(repo => repo.ReadPersonNamesFromFile(inputPath)).Returns(personNames);
        
        // Act
        orderNamesService.OrderNamesInFile(inputPath, outputPath);
        
        // Assert
        mockFileAccessRepository.Verify(repo => repo.WritePersonNamesToFile(
            It.Is<IEnumerable<PersonName>>(names => names.Any() && names.ElementAt(0).FullName == "Alice Marie Doe" &&
                                                  names.ElementAt(1).FullName == "John Michael Doe" &&
                                                  names.ElementAt(2).FullName == "Jane Ann Smith"),
            outputPath), Times.Once);

        mockOutputWriter.Verify(writer => writer.WriteLine("Alice Marie Doe"), Times.Once);
        mockOutputWriter.Verify(writer => writer.WriteLine("John Michael Doe"), Times.Once);
        mockOutputWriter.Verify(writer => writer.WriteLine("Jane Ann Smith"), Times.Once);
    }

    [Fact]
    public void OrderNamesInFile_ShouldHandleInvalidNameFormats()
    {
        // Arrange
        var inputPath = "invalid_names.txt";
        var outputPath = "output.txt";
        
        var personNames = new List<PersonName>
        {
            new ( ["John"], "Doe"),
            new ( [], "Smith"), // Invalid: no given names
            new ( ["Alice"], ""), // Invalid: no last name
            new ( ["Bob", "Michael", "James", "David"], "Johnson") // Invalid: more than 3 given names
        };
        
        mockFileAccessRepository.Setup(repo => repo.ReadPersonNamesFromFile(inputPath)).Returns(personNames);
        
        var errorFilePath = "invalid-names-list.txt";

        // Act
        orderNamesService.OrderNamesInFile(inputPath, outputPath);
        
        // Assert
        mockFileAccessRepository.Verify(repo => repo.WritePersonNamesToFile(
            It.Is<IEnumerable<PersonName>>(names => names.Any() && names.ElementAt(0).FullName == "John Doe" &&
                                                  !names.Any(n => n.FullName == "Smith") &&
                                                  !names.Any(n => n.FullName == "Alice") &&
                                                  !names.Any(n => n.FullName == "Bob Michael James David Johnson")),
            outputPath), Times.Once);

        mockFileAccessRepository.Verify(repo => repo.WritePersonNamesToFile(
            It.Is<IEnumerable<PersonName>>(names => names.Any()),
            errorFilePath), Times.Once);
    }
}
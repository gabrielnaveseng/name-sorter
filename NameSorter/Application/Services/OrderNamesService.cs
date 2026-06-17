using NameSorter.Application.Ports.In;
using NameSorter.Application.Ports.Out;
using NameSorter.Domain;

namespace NameSorter.Application.Services
{
    public class OrderNamesService(IPersonNameRepository fileAccessRepository, IOutputWriter outputWriter) : IOrderNamesService
    {
        private readonly IPersonNameRepository _fileAccessRepository = fileAccessRepository;
        private readonly IOutputWriter _outputWriter = outputWriter;

        public void OrderNamesInFile(string inputPath, string outputPath)
        {
            var personNames = _fileAccessRepository.ReadPersonNamesFromFile(inputPath);
            var errorFilePath = GetErrorFilePath(outputPath);

            var orderedNames = FilterInvalidNames(personNames, errorFilePath)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.GivenNamesText)
                .ToList();

            _fileAccessRepository.WritePersonNamesToFile(orderedNames, outputPath);

            foreach (var name in orderedNames)
            {
                _outputWriter.WriteLine(name.FullName);
            }
        }

        private static string GetErrorFilePath(string outputPath)
        {
            var directory = Path.GetDirectoryName(outputPath);
            return string.IsNullOrWhiteSpace(directory)
                ? "invalid-names-list.txt"
                : Path.Combine(directory, "invalid-names-list.txt");
        }

        private List<PersonName> FilterInvalidNames(IList<PersonName> personNames, string errorFilePath)
        {
            var invalidNames = personNames
                .Where(name => string.IsNullOrWhiteSpace(name.LastName)
                            || string.IsNullOrWhiteSpace(name.GivenNamesText)
                            || name.GivenNames.Count > 3)
                .ToList();

            _fileAccessRepository.WritePersonNamesToFile(invalidNames, errorFilePath);

            return personNames
                .Where(name => !invalidNames.Contains(name))
                .ToList();
        }
    }
}
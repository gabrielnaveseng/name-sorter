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
            
            var orderedNames = FilterInvalidNames(personNames)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.GivenNamesText)
                .ToList();

            _fileAccessRepository.WritePersonNamesToFile(orderedNames, outputPath);

            foreach (var name in orderedNames)
            {
                _outputWriter.WriteLine(name.FullName);
            }
        }

        private IEnumerable<PersonName> FilterInvalidNames(IList<PersonName> personNames)
        {
            var invalidNames = personNames.Where(name => string.IsNullOrWhiteSpace(name.LastName)
                                                || string.IsNullOrWhiteSpace(name.GivenNamesText)
                                                || name.GivenNames.Count() > 3).ToList();
            
            var errorFilePath = "invalid-names-list.txt";
            
            _fileAccessRepository.WritePersonNamesToFile(invalidNames, errorFilePath);
            
            return personNames.Except(invalidNames);
        }
    }
}
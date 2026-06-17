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
            
            personNames = ValidatePersonNames(personNames);

            var orderedNames = personNames
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.GivenNamesText);

            _fileAccessRepository.WritePersonNamesToFile(orderedNames, outputPath);

            foreach (var name in orderedNames)
            {
                _outputWriter.WriteLine(name.FullName);
            }
        }

        private IEnumerable<PersonName> ValidatePersonNames(IEnumerable<PersonName> personNames)
        {
            var invalidNames = personNames.Where(name => string.IsNullOrWhiteSpace(name.LastName)
                                                || string.IsNullOrWhiteSpace(name.GivenNamesText)
                                                || name.GivenNames.Count() > 3);

            foreach (var invalidName in invalidNames)
            {
                _outputWriter.WriteLine($"Invalid name format: '{invalidName.FullName}'. Each name must have a last name and 1 to 3 given names.");
            }

            return personNames.Except(invalidNames);
        }
    }
}
using NameSorter.Domain;

namespace NameSorter.Application.Ports.Out
{
    public interface IPersonNameRepository
    {
        IEnumerable<PersonName> ReadPersonNamesFromFile(string filePath);
        void WritePersonNamesToFile(IEnumerable<PersonName> personNames, string filePath);
    }
}
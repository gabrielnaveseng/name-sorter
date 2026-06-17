using NameSorter.Application.Ports.Out;
using NameSorter.Domain;

namespace NameSorter.Infrastructure
{
    public class PersonNameFileRepository : IPersonNameRepository
    {
        public IEnumerable<PersonName> ReadPersonNamesFromFile(string filePath)
        {
            string[] lines = ReadAllLines(filePath);

            return lines.Select(line =>
            {
                var parts = line.Split(' ');
                var lastName = parts.Last();
                var givenNames = parts.Take(parts.Length - 1);
                return new PersonName(givenNames, lastName);
            });
        }

        public virtual string[] ReadAllLines(string filePath) => File.ReadAllLines(filePath);

        public void WritePersonNamesToFile(IEnumerable<PersonName> personNames, string filePath)
        {
            var lines = personNames.Select(p => p.FullName);
            WriteAllLines(filePath, lines);
        }

        public virtual void WriteAllLines(string filePath, IEnumerable<string> lines) => File.WriteAllLines(filePath, lines);
    }
}
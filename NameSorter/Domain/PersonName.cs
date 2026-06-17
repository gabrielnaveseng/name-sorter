namespace NameSorter.Domain
{
    public record PersonName(IEnumerable<string> givenNames, string lastName)
    {
        public IEnumerable<string> GivenNames { get; } = givenNames;
        public string LastName { get; } = lastName;
        public string FullName => $"{string.Join(" ", GivenNames)} {LastName}";
        public string GivenNamesText => string.Join(" ", GivenNames);
    }
}
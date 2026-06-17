namespace NameSorter.Domain
{
    public record PersonName(IReadOnlyList<string> GivenNames, string LastName)
    {
        public string FullName => string.Join(" ", GivenNames.Append(LastName)).Trim();
        public string GivenNamesText => string.Join(" ", GivenNames);
    }
}
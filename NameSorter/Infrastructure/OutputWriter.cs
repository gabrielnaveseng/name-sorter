using NameSorter.Application.Ports.Out;

namespace NameSorter.Infrastructure
{
    public class OutputWriter : IOutputWriter
    {
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }
    }
}
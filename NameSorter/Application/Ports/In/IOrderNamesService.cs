namespace NameSorter.Application.Ports.In
{
    public interface IOrderNamesService
    {
        void OrderNamesInFile(string inputPath, string outputPath);
    }
}
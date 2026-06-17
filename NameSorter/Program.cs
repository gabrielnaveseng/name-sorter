using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NameSorter.Application.Ports.In;
using NameSorter.Application.Ports.Out;
using NameSorter.Application.Services;
using NameSorter.Infrastructure;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: name-sorter <path-to-input-file>");
            return;
        }

        var inputPath = args[0];
        var outputPath = "sorted-names-list.txt";

        using var host = CreateHostBuilder().Build();

        var orderService = host.Services.GetRequiredService<IOrderNamesService>();
        orderService.OrderNamesInFile(inputPath, outputPath);
    }

    private static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IOrderNamesService, OrderNamesService>();
                services.AddSingleton<IPersonNameRepository, PersonNameFileRepository>();
                services.AddSingleton<IOutputWriter, OutputWriter>();
            });
}

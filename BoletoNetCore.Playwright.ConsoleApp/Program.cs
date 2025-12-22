using System.CommandLine;
using BoletoNetCore.Playwright.ConsoleApp.Commands;

namespace BoletoNetCore.Playwright.ConsoleApp;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("BoletoNetCore Playwright PDF Test Application")
        {
            SimpleCommand.Create(),
            MetricsCommand.Create()
        };

        var parseResult = rootCommand.Parse(args);
        return await parseResult.InvokeAsync();
    }
}

using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;
using TOTOllyGeek.Awesome.Dump;
using TOTOllyGeek.Awesome.Figlet;
using TOTOllyGeek.Awesome.Repl;

namespace TOTOllyGeek.Awesome.Menu;

public class MenuCommand :  Command<MenuCommand.Settings>
{   
    public class Settings : CommandSettings
    {
    }
    
    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        var executor = AnsiConsole.Prompt(
            new SelectionPrompt<OperationExecutor>()
                .Title("Choose from the menu of operations")
                .UseConverter(e => e.OperationName)
                .PageSize(10)
                .AddChoices(
                    new FigletExecutor(),
                    new DumpPersonExecutor(),
                    new ReplExecutor(),
                    new ExitExecutor()));

        return executor.Execute();
    }
}
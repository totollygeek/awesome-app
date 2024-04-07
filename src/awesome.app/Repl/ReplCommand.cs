using System.Diagnostics.CodeAnalysis;
using Spectre.Console.Cli;

namespace TOTOllyGeek.Awesome.Repl;

public class ReplCommand :  Command<ReplCommand.Settings>
{
    public class Settings : CommandSettings
    {
    }
    
    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {        
        return new ReplExecutor().Execute();
    }
}
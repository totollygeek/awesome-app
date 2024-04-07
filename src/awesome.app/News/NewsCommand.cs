using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console.Cli;
using TOTOllyGeek.Awesome.Lib;

namespace TOTOllyGeek.Awesome.News;

public class NewsCommand :  Command<NewsCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Tag (query) for which to search")]
        [CommandArgument(1, "[SearchTag]")]
        [DefaultValue("NASA")]
        public string SearchTag { get; init; }
    }
    
    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        return new NewsExecutor(settings.SearchTag).Execute();
    }
}
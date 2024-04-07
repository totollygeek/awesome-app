using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console.Cli;
using TOTOllyGeek.Awesome.Lib;

namespace TOTOllyGeek.Awesome.Figlet;

public class FigletCommand :  Command<FigletCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Text to transform as a figgle.")]
        [CommandArgument(0, "[Text]")]
        public string Text { get; init; }
        
        [Description("Optional parameter to define a font for the figgle.")]
        [CommandArgument(2, "[Font]")]
        [DefaultValue(FiggleFont.Standard)]
        public FiggleFont Font { get; init; }
    }
    
    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        return new FigletExecutor(settings.Text, settings.Font).Execute();
    }
}
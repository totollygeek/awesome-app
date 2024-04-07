#nullable enable
using System;
using Spectre.Console;
using TOTOllyGeek.Awesome.Lib;

namespace TOTOllyGeek.Awesome.Figlet;

internal class FigletExecutor(string? text = null, FiggleFont? font = null) : OperationExecutor
{
    public override string OperationName => "Figlet";

    public override int Execute()
    {
        var figletText = text ?? AnsiConsole.Prompt(
            new TextPrompt<string>("Please enter [lime]text[/] for figlet ([underline]min 2, max 12 chars[/]):")
                .PromptStyle("green")
                .ValidationErrorMessage("[red]Invalid text input[/]")
                .Validate(input =>
                {
                    return input.Length switch
                    {
                        <= 2 => ValidationResult.Error("[red]You must enter at least 2 characters[/]"),
                        > 12 => ValidationResult.Error("[red]You can enter maximum of 12 characters[/]"),
                        _ => ValidationResult.Success(),
                    };
                }));
        
        var figletFont = font ?? AnsiConsole.Prompt(
            new SelectionPrompt<FiggleFont>()
                .EnableSearch()
                .Title("Choose which [green]font[/] you want to use?")
                .AddChoices(Enum.GetValues<FiggleFont>()));

        AnsiConsole.MarkupLine($"Figlet with font [green]{figletFont}[/]: ");
        
        AnsiConsole.WriteLine(new FigMe(figletText, figletFont).ToString());

        return 0;
    }
}
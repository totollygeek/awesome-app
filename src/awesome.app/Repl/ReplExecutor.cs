using System.Diagnostics;
using Spectre.Console;

namespace TOTOllyGeek.Awesome.Repl;

internal class ReplExecutor() : OperationExecutor
{
    private const string CSharpReplDotnetTool = "csharprepl";
    
    public override string OperationName => "C# REPL";

    public override int Execute()
    {
        if (!IsDotnetToolInstalled(CSharpReplDotnetTool))
        {
            InstallDotnetTool(CSharpReplDotnetTool);
        }

        return RunDotnetTool(CSharpReplDotnetTool);
    }
    
    private static bool IsDotnetToolInstalled(string toolName)
    {
        var process = new Process();
        process.StartInfo.FileName = "dotnet";
        process.StartInfo.Arguments = $"tool list -g";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        
        process.Start();
        
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        var found = output.Contains(toolName);

        AnsiConsole.MarkupLine(!found
            ? ":cross_mark: [yellow]C# REPL dotnet tool is [underline]NOT[/] installed.[/]"
            : ":check_mark_button: [lime]C# REPL dotnet tool is [underline]INSTALLED[/].[/]");

        return found;
    }

    private static void InstallDotnetTool(string toolName)
    {
        AnsiConsole.MarkupLine(":floppy_disk: [lime][underline]Installing[/] C# REPL...[/]");
        
        // Synchronous
        AnsiConsole.Status()
            .Spinner(Spinner.Known.BouncingBall)
            .Start("Installing...", ctx => 
            {
                var process = new Process();
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = $"tool install -g {toolName} -v d";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.OutputDataReceived += (_, args) => 
                    AnsiConsole.MarkupLine($":check_mark: {args.Data.EscapeMarkup()}");
                
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            });
    }

    private static int RunDotnetTool(string toolName)
    {
        AnsiConsole.MarkupLine(":person_running: [lime][underline]Running[/] C# REPL...[/]");
        
        var process = new Process();
        process.StartInfo.FileName = toolName;
        process.Start();
        process.WaitForExit();
        
        AnsiConsole.MarkupLine(":end_arrow: [lime]C# REPL [underline]finished[/][/]");

        return process.ExitCode;
    }
}
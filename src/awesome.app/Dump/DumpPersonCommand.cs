using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console.Cli;
using TOTOllyGeek.Awesome.Lib;

namespace TOTOllyGeek.Awesome.Dump;

public class DumpPersonCommand :  Command<DumpPersonCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Include private fields in the dump")]
        [CommandOption("-f|--includeFields")]
        [DefaultValue(false)]
        public bool ShowPrivateFields { get; init; }
        
        [Description("Include private members in the dump")]
        [CommandOption("-p|--includePrivate")]
        [DefaultValue(false)]
        public bool ShowPrivateMembers { get; init; }
        
        [Description("Include salary dump")]
        [CommandOption("-s|--includeSalary")]
        [DefaultValue(false)]
        public bool IncludeSalary { get; init; }
    }
    
    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        return new DumpPersonExecutor(
            showPrivateFields: settings.ShowPrivateFields,
            showPrivateMembers: settings.ShowPrivateMembers,
            showSalary: settings.IncludeSalary).Execute();
    }
}
using Spectre.Console.Cli;
using TOTOllyGeek.Awesome.Dump;
using TOTOllyGeek.Awesome.Figlet;
using TOTOllyGeek.Awesome.Menu;
using TOTOllyGeek.Awesome.Repl;

var app = new CommandApp();
            
app.Configure(config =>
{
    config.AddCommand<MenuCommand>("menu");
    config.AddCommand<FigletCommand>("figlet");
    config.AddCommand<DumpPersonCommand>("dump");
    config.AddCommand<ReplCommand>("repl");
});

return app.Run(args);
using Spectre.Console;

namespace TOTOllyGeek.Awesome.Menu;

internal class ExitExecutor : OperationExecutor
{
    public override string OperationName => "Exit";
    public override int Execute()
    {
        
        var image = new CanvasImage("why.png");
        image.BilinearResampler();
        image.NoMaxWidth();
        
        AnsiConsole.Write(image);
        
        AnsiConsole.Write(
            new FigletText("But why ?")
                .LeftJustified()
                .Color(Color.Gold1));

        return 0;
    }
}
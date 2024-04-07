namespace TOTOllyGeek.Awesome;

internal abstract class OperationExecutor
{
    public abstract string OperationName { get; }
    public abstract int Execute();
}
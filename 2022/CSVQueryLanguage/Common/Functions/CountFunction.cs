namespace CSVQueryLanguage.Common.Functions;

public readonly struct CountFunction : IFunction
{
    public object Invoke(object[] arguments)
    {
        return (double)arguments[0] + 1;
    }
}

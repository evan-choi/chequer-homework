namespace CSVQueryLanguage.Common.Functions;

public readonly struct ConcatFunction : IFunction
{
    public object Invoke(object[] arguments)
    {
        var left = (string)arguments[0];
        var right = (string)arguments[1];

        return left + right;
    }
}

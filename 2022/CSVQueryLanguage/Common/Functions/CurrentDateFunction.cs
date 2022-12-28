using System;

namespace CSVQueryLanguage.Common.Functions;

public readonly struct CurrentDateFunction : IFunction
{
    public object Invoke(object[] arguments)
    {
        return DateOnly.FromDateTime(DateTime.Now);
    }
}

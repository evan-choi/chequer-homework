using System;

namespace CSVQueryLanguage.Common.Functions;

public readonly struct CurrentTimeFunction : IFunction
{
    public object Invoke(object[] arguments)
    {
        return TimeOnly.FromDateTime(DateTime.Now);
    }
}

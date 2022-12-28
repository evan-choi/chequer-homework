using System;

namespace CSVQueryLanguage.Common.Functions;

public readonly struct SubstringFunction : IFunction
{
    public object Invoke(object[] arguments)
    {
        var value = (string)arguments[0];
        var offset = Convert.ToInt32(arguments[1]);
        var length = value.Length - offset;

        if (arguments.Length >= 2)
            length = Convert.ToInt32(arguments[2]);

        return value.Substring(offset, length);
    }
}

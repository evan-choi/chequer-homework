using System;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Common.Functions;

public readonly struct CastFunction : IFunction
{
    private readonly DataType _type;

    public CastFunction(DataType type)
    {
        _type = type;
    }

    public object Invoke(object[] arguments)
    {
        var value = arguments[0];

        if (value is null)
            return null;

        switch (_type)
        {
            case DataType.Text:
                if (value is string)
                    return value;

                return value.ToString();

            case DataType.Number:
                if (value is double)
                    return value;

                return Convert.ToDouble(value);

            case DataType.Date:
                if (value is DateOnly)
                    return value;

                return DateOnly.Parse(value.ToString()!);

            case DataType.Time:
                if (value is TimeOnly)
                    return value;

                return TimeOnly.Parse(value.ToString()!);

            case DataType.Timestamp:
                if (value is DateTimeOffset)
                    return value;

                return DateTimeOffset.Parse(value.ToString()!);

            case DataType.Boolean:
                if (value is bool)
                    return value;

                return Convert.ToBoolean(value);

            default:
                return null;
        }
    }
}

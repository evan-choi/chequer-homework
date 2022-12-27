using System;
using System.Collections.Generic;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Analysis;

public sealed class ExpressionTypes
{
    public DataType? this[IExpression expression]
    {
        get
        {
            if (_types.TryGetValue(expression, out DataType? dataType))
                return dataType;

            if (TryGetDataType(expression, out dataType))
                return dataType;

            throw new KeyNotFoundException();
        }
        set => _types[expression] = value;
    }

    private readonly Dictionary<IExpression, DataType?> _types = new();

    private bool TryGetDataType(IExpression expression, out DataType? dataType)
    {
        switch (expression)
        {
            case BooleanLiteral:
                dataType = DataType.Boolean;
                return true;

            case DateLiteral:
                dataType = DataType.Date;
                return true;

            case NullLiteral:
                dataType = null;
                return true;

            case NumberLiteral:
                dataType = DataType.Number;
                return true;

            case TextLiteral:
                dataType = DataType.Text;
                return true;

            case TimeLiteral:
                dataType = DataType.Time;
                return true;

            case TimestampLiteral:
                dataType = DataType.Timestamp;
                return true;
        }

        dataType = default;
        return false;
    }
}

using CSVQueryLanguage.Parser;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage;

internal static class CqlErrors
{
    public static CqlException RelationNotFound()
    {
        return new CqlException("No relation");
    }

    public static CqlException RelationNotFound(string relationName)
    {
        return new CqlException($"Relation {relationName} not found");
    }

    public static CqlException ColumnNotFound(string columnName)
    {
        return new CqlException($"{columnName} column not found");
    }

    public static CqlException NotSupportedLogicalOperator(LogicalOperator op, DataType? left, DataType? right)
    {
        var opText = CqlDeparser.Deparse(op);
        var leftTypeName = left.HasValue ? CqlDeparser.Deparse(left.Value) : "NULL";
        var rightTypeName = right.HasValue ? CqlDeparser.Deparse(right.Value) : "NULL";
        return new CqlException($"Not supported logical operator '{opText}' on {leftTypeName} and {rightTypeName}");
    }

    public static CqlException NotSupportedBinaryOperator(ArithmeticOperator op, DataType? left, DataType? right)
    {
        var opText = CqlDeparser.Deparse(op);
        var leftTypeName = left.HasValue ? CqlDeparser.Deparse(left.Value) : "NULL";
        var rightTypeName = right.HasValue ? CqlDeparser.Deparse(right.Value) : "NULL";
        return new CqlException($"Not supported arithmetic operator '{opText}' on {leftTypeName} and {rightTypeName}");
    }

    public static CqlException NotSupportedUnaryOperator(ArithmeticSign sign, DataType? value)
    {
        var signText = CqlDeparser.Deparse(sign);
        var valueTypeName = value.HasValue ? CqlDeparser.Deparse(value.Value) : "NULL";
        return new CqlException($"Not supported unary operator '{signText}' on {valueTypeName}");
    }

    public static CqlException NotSupportedNotOperator(DataType? value)
    {
        var valueTypeName = value.HasValue ? CqlDeparser.Deparse(value.Value) : "NULL";
        return new CqlException($"Not supported not operator on {valueTypeName}");
    }

    public static CqlException InvalidArgumentBindingOfFunction(string functionName, string argumentName, DataType? argumentType)
    {
        var argTypeName = argumentType.HasValue ? CqlDeparser.Deparse(argumentType.Value) : "NULL";
        return new CqlException($"Cannot bind {argTypeName} to argument '{argumentName}' of function '{functionName}'");
    }

    public static CqlException UnknownFunction(string functionName)
    {
        return new CqlException($"Unknown function '{functionName}'");
    }
}

using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Driver.Interpretation;

public static partial class AtomicInteractive
{
    public static bool CanInterpret(LogicalOperator op, DataType? left, DataType? right)
    {
        return (left, right) is (DataType.Boolean or null, DataType.Boolean or null);
    }

    public static bool Interpret(LogicalOperator op, bool left, bool right)
    {
        return op is LogicalOperator.And ? left && right : left || right;
    }

    public static bool InterpretBoxed(LogicalOperator op, object boxedLeft, object boxedRight)
    {
        if (boxedLeft is not bool left)
            left = false;

        if (boxedRight is not bool right)
            right = false;

        return Interpret(op, left, right);
    }
}

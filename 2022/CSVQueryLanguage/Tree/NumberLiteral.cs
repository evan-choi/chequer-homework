using System.Diagnostics;

namespace CSVQueryLanguage.Tree;

// RULE: primaryExpression > #numberLiteral
[DebuggerDisplay("{Value}")]
public sealed class NumberLiteral : ILiteral
{
    public double Value { get; }

    object ILiteral.Value => Value;

    public NumberLiteral(double value)
    {
        Value = value;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitNumberLiteral(this);
    }
}

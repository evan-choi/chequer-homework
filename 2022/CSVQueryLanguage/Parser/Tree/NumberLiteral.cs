using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: primaryExpression > #numberLiteral
[DebuggerDisplay("{Value}")]
public sealed class NumberLiteral : ILiteral
{
    public long Value { get; }

    public NumberLiteral(long value)
    {
        Value = value;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitNumberLiteral(this);
    }
}

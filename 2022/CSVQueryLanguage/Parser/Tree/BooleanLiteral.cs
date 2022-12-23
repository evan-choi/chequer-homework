using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: primaryExpression > #booleanLiteral
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
public sealed class BooleanLiteral : ILiteral
{
    public bool Value { get; }

    public BooleanLiteral(bool value)
    {
        Value = value;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitBooleanLiteral(this);
    }
}

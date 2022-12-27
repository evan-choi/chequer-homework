using System.Diagnostics;

namespace CSVQueryLanguage.Tree;

// RULE: primaryExpression > #booleanLiteral
[DebuggerDisplay("{Parser.CqlDeparser.Deparse(this)}")]
public sealed class BooleanLiteral : ILiteral
{
    public static BooleanLiteral True { get; } = new(true);

    public static BooleanLiteral False { get; } = new(false);

    public bool Value { get; }

    object ILiteral.Value => Value;

    public BooleanLiteral(bool value)
    {
        Value = value;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitBooleanLiteral(this);
    }
}

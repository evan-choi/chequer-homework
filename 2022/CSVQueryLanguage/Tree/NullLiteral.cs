using System.Diagnostics;

namespace CSVQueryLanguage.Tree;

// RULE: primaryExpression > #nullLiteral
[DebuggerDisplay("NULL")]
public sealed class NullLiteral : ILiteral
{
    public static NullLiteral Default { get; } = new();

    public object Value => null;

    object ILiteral.Value => Value;

    private NullLiteral()
    {
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitNullLiteral(this);
    }
}

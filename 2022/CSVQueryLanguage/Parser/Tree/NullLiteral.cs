using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: primaryExpression > #nullLiteral
[DebuggerDisplay("NULL")]
public sealed class NullLiteral : ILiteral
{
    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitNullLiteral(this);
    }
}

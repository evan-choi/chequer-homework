using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: booleanExpression > #logicalNot
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
public sealed class NotExpression : IExpression
{
    public IExpression Value { get; }

    public NotExpression(IExpression value)
    {
        ArgumentNullException.ThrowIfNull(value);

        Value = value;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitNotExpression(this);
    }
}

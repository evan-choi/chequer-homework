using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Tree;

// RULE: booleanExpression > #logicalNot
[DebuggerDisplay("{Parser.CqlDeparser.Deparse(this)}")]
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

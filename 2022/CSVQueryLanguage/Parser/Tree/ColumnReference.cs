using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: primaryExpression > #columnReference
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
public sealed class ColumnReference : IExpression
{
    public QualifiedName Name { get; }

    public ColumnReference(QualifiedName name)
    {
        ArgumentNullException.ThrowIfNull(name);

        Name = name;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitColumnReference(this);
    }
}

using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: qualifiedName
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
public sealed class QualifiedName : INode
{
    public Identifier[] Parts { get; }

    public QualifiedName(params Identifier[] parts)
    {
        ArgumentNullException.ThrowIfNull(parts);

        Parts = parts;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitQualifiedName(this);
    }
}

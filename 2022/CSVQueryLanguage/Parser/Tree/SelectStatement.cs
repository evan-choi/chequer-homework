using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: statement > #selectStatement
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
public sealed class SelectStatement : IStatement
{
    public Query Query { get; }

    public SelectStatement(Query query)
    {
        ArgumentNullException.ThrowIfNull(query);

        Query = query;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitSelectStatement(this);
    }
}

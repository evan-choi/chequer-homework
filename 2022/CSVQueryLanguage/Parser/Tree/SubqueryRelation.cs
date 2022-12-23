using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: relationPrimary > #subquery
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
public sealed class SubqueryRelation : IRelation
{
    public Query Query { get; }

    public SubqueryRelation(Query query)
    {
        ArgumentNullException.ThrowIfNull(query);

        Query = query;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitSubqueryRelation(this);
    }
}

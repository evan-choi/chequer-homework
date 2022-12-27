using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Tree;

// RULE: relationPrimary > #subquery
[DebuggerDisplay("{Parser.CqlDeparser.Deparse(this)}")]
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

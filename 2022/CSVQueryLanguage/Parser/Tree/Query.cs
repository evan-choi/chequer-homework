using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: query
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
public sealed class Query : INode
{
    public Select Select { get; }

    public AliasedRelation From { get; }

    public IExpression Where { get; }

    public Limit Limit { get; }

    public Query(Select select, AliasedRelation from, IExpression where, Limit limit)
    {
        ArgumentNullException.ThrowIfNull(select);

        Select = select;
        From = from;
        Where = where;
        Limit = limit;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitQuery(this);
    }
}

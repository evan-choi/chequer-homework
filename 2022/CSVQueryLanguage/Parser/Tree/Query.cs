using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: query
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
public sealed class Query : INode
{
    public Select Select { get; }

    public AliasedRelation AliasedRelation { get; }

    public IExpression Where { get; }

    public Limit Limit { get; }

    public Query(Select select, AliasedRelation aliasedRelation, IExpression where, Limit limit)
    {
        ArgumentNullException.ThrowIfNull(select);

        Select = select;
        AliasedRelation = aliasedRelation;
        Where = where;
        Limit = limit;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitQuery(this);
    }
}

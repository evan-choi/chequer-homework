using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: aliasedRelation
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
public sealed class AliasedRelation : IRelation
{
    public IRelation Relation { get; }

    public Identifier Alias { get; }

    public AliasedRelation(IRelation relation, Identifier alias)
    {
        ArgumentNullException.ThrowIfNull(relation);

        Relation = relation;
        Alias = alias;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitAliasedRelation(this);
    }
}

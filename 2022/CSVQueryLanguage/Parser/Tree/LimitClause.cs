using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: limitClause
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
public sealed class Limit : INode
{
    public long? Offset { get; }

    public long Count { get; }

    public Limit(long? offset, long count)
    {
        Offset = offset;
        Count = count;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitLimitClause(this);
    }
}

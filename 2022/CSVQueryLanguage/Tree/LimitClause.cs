using System.Diagnostics;

namespace CSVQueryLanguage.Tree;

// RULE: limitClause
[DebuggerDisplay("{Parser.CqlDeparser.Deparse(this)}")]
public sealed class Limit : INode
{
    public long? Offset { get; }

    public long? Count { get; }

    public Limit(long? offset, long? count)
    {
        Offset = offset;
        Count = count;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitLimitClause(this);
    }
}

using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: booleanExpression > #predicateLike
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
public sealed class LikePredicate : IExpression
{
    public IExpression Value { get; }

    public IExpression Pattern { get; }

    public bool IsCaseInsensitive { get; }

    public LikePredicate(IExpression value, IExpression pattern, bool isCaseInsensitive)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(pattern);

        Value = value;
        Pattern = pattern;
        IsCaseInsensitive = isCaseInsensitive;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitLikePredicate(this);
    }
}

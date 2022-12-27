using System;

namespace CSVQueryLanguage.Tree;

public sealed class TimestampLiteral : ILiteral
{
    public DateTimeOffset Value { get; }

    object ILiteral.Value => Value;

    public TimestampLiteral(DateTimeOffset value)
    {
        Value = value;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitTimestampLiteral(this);
    }
}

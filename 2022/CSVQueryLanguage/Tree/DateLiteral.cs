using System;

namespace CSVQueryLanguage.Tree;

public sealed class DateLiteral : ILiteral
{
    public DateOnly Value { get; }

    object ILiteral.Value => Value;

    public DateLiteral(DateOnly value)
    {
        Value = value;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitDateLiteral(this);
    }
}

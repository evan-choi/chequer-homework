using System;

namespace CSVQueryLanguage.Tree;

public sealed class TimeLiteral : ILiteral
{
    public TimeOnly Value { get; }

    object ILiteral.Value => Value;

    public TimeLiteral(TimeOnly value)
    {
        Value = value;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitTimeLiteral(this);
    }
}

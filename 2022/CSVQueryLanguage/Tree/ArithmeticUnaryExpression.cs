using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Tree;

// RULE: valueExpression > #arithmeticUnary
[DebuggerDisplay("{Parser.CqlDeparser.Deparse(this)}")]
public sealed class ArithmeticUnaryExpression : IExpression
{
    public ArithmeticSign Sign { get; }

    public IExpression Value { get; }

    public ArithmeticUnaryExpression(ArithmeticSign sign, IExpression value)
    {
        ArgumentNullException.ThrowIfNull(value);

        Sign = sign;
        Value = value;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitArithmeticUnaryExpression(this);
    }
}

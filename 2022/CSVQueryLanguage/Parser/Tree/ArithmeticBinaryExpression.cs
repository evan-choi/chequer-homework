using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: valueExpression > #arithmeticBinary
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
public sealed class ArithmeticBinaryExpression : IExpression
{
    public ArithmeticOperator Operator { get; }

    public IExpression Left { get; }

    public IExpression Right { get; }

    public ArithmeticBinaryExpression(ArithmeticOperator @operator, IExpression left, IExpression right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        Operator = @operator;
        Left = left;
        Right = right;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitArithmeticBinaryExpression(this);
    }
}

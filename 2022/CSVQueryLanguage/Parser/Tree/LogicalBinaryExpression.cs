using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: booleanExpression > #logicalBinary
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
public sealed class LogicalBinaryExpression : IExpression
{
    public LogicalOperator Operator { get; }

    public IExpression Left { get; }

    public IExpression Right { get; }

    public LogicalBinaryExpression(LogicalOperator @operator, IExpression left, IExpression right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        Operator = @operator;
        Left = left;
        Right = right;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitLogicalBinaryExpression(this);
    }
}

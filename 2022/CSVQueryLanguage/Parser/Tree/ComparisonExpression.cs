using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: booleanExpression > #predicateComparison
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
public sealed class ComparisonExpression : IExpression
{
    public ComparisonOperator Operator { get; }

    public IExpression Left { get; }

    public IExpression Right { get; }

    public ComparisonExpression(ComparisonOperator @operator, IExpression left, IExpression right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        Operator = @operator;
        Left = left;
        Right = right;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitComparisonExpression(this);
    }
}

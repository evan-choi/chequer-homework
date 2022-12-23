using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: selectItem > #selectSingle
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
public sealed class SingleColumn : ISelectItem
{
    public IExpression Expression { get; }

    public Identifier Alias { get; }

    public SingleColumn(IExpression expression, Identifier alias)
    {
        ArgumentNullException.ThrowIfNull(expression);

        Expression = expression;
        Alias = alias;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitSingleColumn(this);
    }
}

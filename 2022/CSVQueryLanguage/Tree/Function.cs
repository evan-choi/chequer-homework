using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Tree;

// RULE: function
[DebuggerDisplay("{Parser.CqlDeparser.Deparse(this)}")]
public sealed class Function : IExpression
{
    public string Name { get; }

    public IExpression[] Arguments { get; }

    public Function(string name, params IExpression[] arguments)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(arguments);

        Name = name;
        Arguments = arguments;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitFunction(this);
    }
}

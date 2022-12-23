using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: function
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
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

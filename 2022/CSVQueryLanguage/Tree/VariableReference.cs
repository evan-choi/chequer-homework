using System.Diagnostics;

namespace CSVQueryLanguage.Tree;

[DebuggerDisplay("{Name}")]
public sealed class VariableReference : IExpression
{
    public string Name { get; }

    public VariableReference(string name)
    {
        Name = name;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitVariableReference(this);
    }
}

using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

[DebuggerDisplay("[{Index}]")]
public sealed class FieldReference : IExpression
{
    public int Index { get; }

    public FieldReference(int index)
    {
        Index = index;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitFieldReference(this);
    }
}

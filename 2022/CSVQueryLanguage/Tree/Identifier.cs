using System.Diagnostics;

namespace CSVQueryLanguage.Tree;

// RULE: identifier
[DebuggerDisplay("{OriginalValue,nq}")]
public sealed class Identifier : INode
{
    public string Value { get; }

    public string OriginalValue { get; }

    public Identifier(string value, string originalValue)
    {
        Value = value;
        OriginalValue = originalValue;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitIdentifier(this);
    }
}

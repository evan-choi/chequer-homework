using System.Diagnostics;

namespace CSVQueryLanguage.Tree;

// RULE: primaryExpression > #stringLiteral
[DebuggerDisplay("{Value}")]
public sealed class TextLiteral : ILiteral
{
    public string Value { get; }

    object ILiteral.Value => Value;

    public TextLiteral(string value)
    {
        Value = value;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitStringLiteral(this);
    }
}

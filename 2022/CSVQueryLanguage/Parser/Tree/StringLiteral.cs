using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: primaryExpression > #stringLiteral
[DebuggerDisplay("{Value}")]
public sealed class StringLiteral : ILiteral
{
    public string Value { get; }

    public StringLiteral(string value)
    {
        Value = value;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitStringLiteral(this);
    }
}

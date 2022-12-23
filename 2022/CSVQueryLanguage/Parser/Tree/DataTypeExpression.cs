using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: type
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
public sealed class DataTypeExpression : IExpression
{
    public DataType Type { get; }

    public DataTypeExpression(DataType type)
    {
        Type = type;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitDataTypeExpression(this);
    }
}

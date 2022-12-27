using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Plan.Nodes;

public sealed class ProjectNode : PlanNode
{
    public IExpression[] Expressions { get; }

    public DataType?[] DataTypes { get; }

    public string[] Names { get; }

    public ProjectNode(PlanNode source, IExpression[] expressions, DataType?[] dataTypes, string[] names) : base(source)
    {
        Expressions = expressions;
        DataTypes = dataTypes;
        Names = names;
    }
}

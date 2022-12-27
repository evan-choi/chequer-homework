using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Plan.Nodes;

public sealed class FilterNode : PlanNode
{
    public IExpression Predicate { get; }

    public FilterNode(PlanNode source, IExpression predicate) : base(source)
    {
        Predicate = predicate;
    }
}

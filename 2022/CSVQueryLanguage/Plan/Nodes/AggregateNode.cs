namespace CSVQueryLanguage.Plan.Nodes;

public sealed class AggregateNode : PlanNode
{
    public AggregateNode(PlanNode source) : base(source)
    {
    }
}

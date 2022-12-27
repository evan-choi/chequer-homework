namespace CSVQueryLanguage.Plan.Nodes;

public sealed class LimitNode : PlanNode
{
    public long Count { get; }

    public LimitNode(PlanNode source, long count) : base(source)
    {
        Count = count;
    }
}

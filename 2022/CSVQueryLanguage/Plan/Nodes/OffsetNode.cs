namespace CSVQueryLanguage.Plan.Nodes;

public sealed class OffsetNode : PlanNode
{
    public long Count { get; }

    public OffsetNode(PlanNode source, long count) : base(source)
    {
        Count = count;
    }
}

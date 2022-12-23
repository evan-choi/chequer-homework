using System.Collections.Generic;

namespace CSVQueryLanguage.Plan.Nodes;

public sealed class OffsetNode : PlanNode
{
    public PlanNode Source { get; }

    public long Count { get; }

    public OffsetNode(PlanNode source, long count)
    {
        Source = source;
        Count = count;
    }

    protected override IEnumerable<PlanNode> GetChildren()
    {
        yield return Source;
    }
}

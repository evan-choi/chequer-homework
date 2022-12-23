using System.Collections.Generic;
using CSVQueryLanguage.Parser.Tree;

namespace CSVQueryLanguage.Plan.Nodes;

public sealed class FilterNode : PlanNode
{
    public PlanNode Source { get; }

    public IExpression Predicate { get; }

    public FilterNode(PlanNode source, IExpression predicate)
    {
        Source = source;
        Predicate = predicate;
    }

    protected override IEnumerable<PlanNode> GetChildren()
    {
        yield return Source;
    }
}

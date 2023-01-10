using CSVQueryLanguage.Analysis;

namespace CSVQueryLanguage.Plan.Nodes;

public sealed class AggregateNode : PlanNode
{
    public VariableInfo CountVariable { get; }

    public AggregateNode(PlanNode source, VariableInfo countVariable) : base(source)
    {
        CountVariable = countVariable;
    }
}

namespace CSVQueryLanguage.Plan.Nodes;

public abstract class PlanNode
{
    public PlanNode Source { get; }

    protected PlanNode()
    {
    }

    protected PlanNode(PlanNode source)
    {
        Source = source;
    }
}

using System.Collections.Generic;

namespace CSVQueryLanguage.Plan.Nodes;

internal sealed class OutputNode : PlanNode
{
    public PlanNode Source { get; }

    public string[] ColumnNames { get; }

    public OutputNode(PlanNode source, string[] columnNames)
    {
        Source = source;
        ColumnNames = columnNames;
    }

    protected override IEnumerable<PlanNode> GetChildren()
    {
        yield return Source;
    }
}

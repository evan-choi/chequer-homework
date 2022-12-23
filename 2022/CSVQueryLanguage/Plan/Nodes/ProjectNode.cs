using System.Collections.Generic;
using CSVQueryLanguage.Parser.Tree;

namespace CSVQueryLanguage.Plan.Nodes;

public sealed class ProjectNode : PlanNode
{
    public PlanNode Source { get; }

    public IReadOnlyDictionary<string, IExpression> Columns { get; }

    public ProjectNode(PlanNode source, IDictionary<string, IExpression> columns)
    {
        Source = source;
        Columns = columns.AsReadOnly();
    }

    protected override IEnumerable<PlanNode> GetChildren()
    {
        yield return Source;
    }
}

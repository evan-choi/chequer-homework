using System.Collections.Generic;
using System.Linq;

namespace CSVQueryLanguage.Plan.Nodes;

public abstract class PlanNode
{
    public IEnumerable<PlanNode> Children => GetChildren();

    protected virtual IEnumerable<PlanNode> GetChildren()
    {
        return Enumerable.Empty<PlanNode>();
    }
}

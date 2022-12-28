using System.Collections.Generic;
using CSVQueryLanguage.Analysis;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Plan.Nodes;

public sealed class AggregateNode : PlanNode
{
    public IReadOnlyDictionary<VariableInfo, IExpression> Variables { get; }

    public AggregateNode(PlanNode source, IReadOnlyDictionary<VariableInfo, IExpression> variables) : base(source)
    {
        Variables = variables;
    }
}

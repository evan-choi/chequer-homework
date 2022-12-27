using System.Collections.Generic;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Analysis;

public sealed class AnalyzerContext
{
    public IStatement Statement { get; }

    public Dictionary<INode, QueryScope> Scopes { get; } = new();

    public ExpressionTypes ExpressionTypes { get; } = new();

    public AnalyzerContext(IStatement statement)
    {
        Statement = statement;
    }
}

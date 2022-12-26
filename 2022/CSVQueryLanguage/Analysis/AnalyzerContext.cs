using System.Collections.Generic;
using CSVQueryLanguage.Parser.Tree;

namespace CSVQueryLanguage.Analysis;

public sealed class AnalyzerContext
{
    public IStatement Statement { get; }

    public Dictionary<IRelation, QueryScope> Scopes { get; } = new();

    private Dictionary<IExpression, DataType> ExpressionTypes { get; } = new();

    public AnalyzerContext(IStatement statement)
    {
        Statement = statement;
    }
}

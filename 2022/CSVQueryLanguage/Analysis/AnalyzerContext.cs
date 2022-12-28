using System.Collections.Generic;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Analysis;

public sealed class AnalyzerContext
{
    public IStatement Statement { get; }

    public Dictionary<INode, QueryScope> Scopes { get; } = new();

    public ExpressionTypes ExpressionTypes { get; } = new();

    public Dictionary<string, VariableInfo> Variables { get; } = new();

    public AnalyzerContext(IStatement statement)
    {
        Statement = statement;
    }

    public VariableInfo DelcareAnonymousVariable(DataType type, INode source)
    {
        for (int i = 0;; i++)
        {
            var name = $"${i}";

            if (Variables.ContainsKey(name))
                continue;

            var variable = new VariableInfo(source, name, type);
            Variables[name] = variable;

            return variable;
        }
    }
}

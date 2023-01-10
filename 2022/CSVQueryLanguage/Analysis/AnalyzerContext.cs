using System;
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

    public VariableInfo DelcareAnonymousVariable(Func<string, VariableInfo> factory)
    {
        for (int i = 0;; i++)
        {
            var name = $"__val${i}";

            if (Variables.ContainsKey(name))
                continue;

            var variable = factory(name);
            Variables[name] = variable;

            return variable;
        }
    }
}

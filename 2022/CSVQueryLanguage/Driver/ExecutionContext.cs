using System;
using System.Collections.Generic;
using System.Linq;
using CSVQueryLanguage.Analysis;
using CSVQueryLanguage.Driver.Interpretation;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Driver;

public sealed class ExecutionContext
{
    public AnalyzerContext AnalyzerContext { get; }

    public Interactive Interactive { get; }

    public Dictionary<string, IRuntimeVariable> Variables { get; }

    public ExecutionContext(AnalyzerContext analyzerContext)
    {
        AnalyzerContext = analyzerContext;
        Interactive = new Interactive(this);

        Variables = analyzerContext.Variables.ToDictionary(
            x => x.Key,
            x => CreateRuntimeVariable(x.Value)
        );
    }

    private IRuntimeVariable CreateRuntimeVariable(VariableInfo info)
    {
        return info.Type switch
        {
            DataType.Text => new RuntimeVariable<string>(info),
            DataType.Number => new RuntimeVariable<double>(info),
            DataType.Date => new RuntimeVariable<DateOnly>(info),
            DataType.Time => new RuntimeVariable<TimeOnly>(info),
            DataType.Timestamp => new RuntimeVariable<DateTimeOffset>(info),
            DataType.Boolean => new RuntimeVariable<bool>(info),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

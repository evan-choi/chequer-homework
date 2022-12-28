using System.Collections.Generic;
using System.Linq;
using CSVQueryLanguage.Analysis;
using CSVQueryLanguage.Driver.Interpretation;

namespace CSVQueryLanguage.Driver;

public sealed class ExecutionContext
{
    public AnalyzerContext AnalyzerContext { get; }

    public Interactive Interactive { get; }

    public Dictionary<string, RuntimeVariable> Variables { get; }

    public ExecutionContext(AnalyzerContext analyzerContext)
    {
        AnalyzerContext = analyzerContext;
        Interactive = new Interactive(this);

        Variables = analyzerContext.Variables.ToDictionary(
            x => x.Key,
            x => new RuntimeVariable(x.Value)
        );
    }
}

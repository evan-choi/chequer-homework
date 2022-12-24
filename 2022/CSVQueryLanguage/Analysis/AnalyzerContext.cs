using CSVQueryLanguage.Parser.Tree;

namespace CSVQueryLanguage.Analysis;

public sealed class AnalyzerContext
{
    public IStatement Statement { get; }

    public AnalyzerContext(IStatement statement)
    {
        Statement = statement;
    }
}

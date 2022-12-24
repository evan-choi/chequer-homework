using CSVQueryLanguage.Parser.Tree;

namespace CSVQueryLanguage.Analysis;

public sealed class Analyzer
{
    public AnalyzerScope AnalyzeStatement(IStatement statement)
    {
        var context = new AnalyzerContext(statement);
        var scope = new AnalyzerScope(context);

        switch (statement)
        {
            case SelectStatement selectStatement:
                var queryAnalyzer = new QueryAnalyzer();
                queryAnalyzer.Analyze();
                break;
        }

        return scope;
    }
}

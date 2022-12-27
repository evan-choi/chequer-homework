using System;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Analysis;

public sealed class Analyzer
{
    public QueryScope AnalyzeStatement(IStatement statement)
    {
        var context = new AnalyzerContext(statement);

        switch (statement)
        {
            case SelectStatement selectStatement:
                var queryAnalyzer = new QueryAnalyzer(context);
                return queryAnalyzer.AnalyzeQuery(selectStatement.Query);
        }

        throw new NotImplementedException();
    }
}

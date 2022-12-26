using System;
using CSVQueryLanguage.Parser.Tree;

namespace CSVQueryLanguage.Analysis;

public sealed class Analyzer
{
    public QueryScope AnalyzeStatement(IStatement statement)
    {
        var context = new AnalyzerContext(statement);

        switch (statement)
        {
            case SelectStatement selectStatement:
                var queryAnalyzer = new QueryAnalyzer();
                return queryAnalyzer.AnalyzeQuery(selectStatement.Query, context);
        }

        throw new NotImplementedException();
    }
}

using CSVQueryLanguage.Parser.Tree;

namespace CSVQueryLanguage.Analysis;

public sealed class Analyzer
{
    public AnalyzerScope AnalyzeStatement(IStatement statement)
    {
        var context = new AnalyzerContext();
        var visitor = new StatementVisitor(context);

        return statement.Accept(visitor);
    }

    private sealed class StatementVisitor : DefaultNodeVisitor<AnalyzerScope>
    {
        private readonly AnalyzerContext _context;

        public StatementVisitor(AnalyzerContext context)
        {
            _context = context;
        }

        public override AnalyzerScope VisitSelectStatement(SelectStatement node)
        {
            return base.VisitSelectStatement(node);
        }
    }
}

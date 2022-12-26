using System;
using CSVQueryLanguage.Analysis;
using CSVQueryLanguage.Parser.Tree;
using CSVQueryLanguage.Plan.Nodes;

namespace CSVQueryLanguage.Plan;

public sealed class Planner
{
    private readonly AnalyzerContext _context;

    public Planner(AnalyzerContext context)
    {
        _context = context;
    }

    public PlanNode PlanStatement(IStatement statement)
    {
        switch (statement)
        {
            case SelectStatement selectStatement:
                var queryPlanner = new QueryPlanner(_context);
                return queryPlanner.PlanQuery(selectStatement.Query);
        }

        throw new NotImplementedException();
    }
}

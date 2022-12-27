using System;
using CSVQueryLanguage.Analysis;
using CSVQueryLanguage.Plan.Nodes;
using CSVQueryLanguage.Tree;

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
                return PlanSelectStatement(selectStatement);
        }

        throw new NotImplementedException();
    }

    private PlanNode PlanSelectStatement(SelectStatement selectStatement)
    {
        var queryPlanner = new QueryPlanner(_context);
        return queryPlanner.PlanQuery(selectStatement.Query);
    }
}

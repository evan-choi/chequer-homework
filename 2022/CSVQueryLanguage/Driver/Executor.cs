using System;
using CSVQueryLanguage.Analysis;
using CSVQueryLanguage.Driver.Cursors;
using CSVQueryLanguage.Plan.Nodes;

namespace CSVQueryLanguage.Driver;

public sealed class Executor
{
    private readonly AnalyzerContext _analyzerContext;

    public Executor(AnalyzerContext analyzerContext)
    {
        _analyzerContext = analyzerContext;
    }

    public ICursor Execute(PlanNode plan)
    {
        return BuildPlan(plan, new ExecutionContext(_analyzerContext));
    }

    private ICursor BuildPlan(PlanNode plan, ExecutionContext context)
    {
        if (plan is CsvScanNode csvScanNode)
            return BuildCsvScanNode(csvScanNode);

        ICursor sourceCursor = null;

        if (plan.Source is not null)
            sourceCursor = BuildPlan(plan.Source, context);

        switch (plan)
        {
            case AggregateNode aggregateNode:
                return BuildAggregateNode(aggregateNode, sourceCursor, context);

            case ProjectNode projectNode:
                return BuildProjectNode(projectNode, sourceCursor, context);

            case FilterNode filterNode:
                return BuildFilterNode(filterNode, sourceCursor, context);

            case LimitNode limitNode:
                return BuildLimitNode(limitNode, sourceCursor, context);

            case OffsetNode offsetNode:
                return BuildOffsetNode(offsetNode, sourceCursor, context);

            default:
                throw new ArgumentOutOfRangeException(nameof(plan));
        }
    }

    private ICursor BuildCsvScanNode(CsvScanNode plan)
    {
        return CsvCursor.OpenReadLocked(plan.FileName);
    }

    private ICursor BuildAggregateNode(AggregateNode plan, ICursor sourceCursor, ExecutionContext context)
    {
        return new AggregateCursor(sourceCursor, context, plan);
    }

    private ICursor BuildFilterNode(FilterNode plan, ICursor sourceCursor, ExecutionContext context)
    {
        return new FilterCursor(sourceCursor, context, plan);
    }

    private ICursor BuildLimitNode(LimitNode plan, ICursor sourceCursor, ExecutionContext context)
    {
        return new LimitCursor(sourceCursor, context, plan);
    }

    private ICursor BuildOffsetNode(OffsetNode plan, ICursor sourceCursor, ExecutionContext context)
    {
        return new OffsetCursor(sourceCursor, context, plan);
    }

    private ICursor BuildProjectNode(ProjectNode plan, ICursor sourceCursor, ExecutionContext context)
    {
        return new ProjectCursor(sourceCursor, context, plan);
    }
}

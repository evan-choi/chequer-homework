using System;
using System.Collections.Generic;
using System.Linq;
using CSVQueryLanguage.Analysis;
using CSVQueryLanguage.Plan.Nodes;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Plan;

public sealed class QueryPlanner
{
    private readonly AnalyzerContext _context;

    public QueryPlanner(AnalyzerContext context)
    {
        _context = context;
    }

    public PlanNode PlanQuery(Query query)
    {
        PlanNode plan = null;

        if (query.From is not null)
        {
            plan = PlanRelation(query.From);

            var fromScope = _context.Scopes[query.From.Relation];
            plan = PlanAggregate(fromScope, plan);
        }

        var scope = _context.Scopes[query];

        plan = PlanWhere(scope.Filter, plan);
        plan = PlanLimit(query.Limit, plan);
        plan = PlanSelect(query.Select, scope, plan);

        return plan;
    }

    private PlanNode PlanAggregate(QueryScope scope, PlanNode plan)
    {
        if (scope.AggregateVariables.Count == 0)
            return plan;

        return new AggregateNode(plan, scope.AggregateVariables);
    }

    private PlanNode PlanWhere(IExpression where, PlanNode source)
    {
        if (where is null)
            return source;

        return new FilterNode(source, where);
    }

    private PlanNode PlanLimit(Limit limit, PlanNode source)
    {
        if (limit is null)
            return source;

        if (limit.Offset.HasValue)
            source = new OffsetNode(source, limit.Offset.Value);

        if (limit.Count.HasValue)
            source = new LimitNode(source, limit.Count.Value);

        return source;
    }

    private PlanNode PlanSelect(Select select, QueryScope scope, PlanNode source)
    {
        if (select.Items is [AllColumns])
            return source;

        IExpression[] expressions = scope.RelationInfo.Fields
            .Select(x => x.Source)
            .ToArray();

        DataType?[] dataTypes = scope.RelationInfo.Fields
            .Select(x => x.Type)
            .ToArray();

        var names = scope.RelationInfo.Fields
            .Select(x => x.Name)
            .ToArray();

        return new ProjectNode(source, expressions, dataTypes, names);
    }

    private PlanNode PlanRelation(IRelation relation)
    {
        return relation switch
        {
            AliasedRelation aliasedRelation => PlanAliasedRelation(aliasedRelation),
            CsvRelation csvRelation => PlanCsvRelation(csvRelation),
            SubqueryRelation subqueryRelation => PlanSubqueryRelation(subqueryRelation),
            _ => throw new ArgumentOutOfRangeException(nameof(relation))
        };
    }

    private PlanNode PlanAliasedRelation(AliasedRelation aliasedRelation)
    {
        return PlanRelation(aliasedRelation.Relation);
    }

    private PlanNode PlanCsvRelation(CsvRelation csvRelation)
    {
        var scope = _context.Scopes[csvRelation];

        var names = scope.RelationInfo.Fields
            .Select(x => x.Name)
            .ToArray();

        return new CsvScanNode(csvRelation.FileName.Value, names);
    }

    private PlanNode PlanSubqueryRelation(SubqueryRelation subqueryRelation)
    {
        return PlanQuery(subqueryRelation.Query);
    }
}

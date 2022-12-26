using System;
using System.Collections.Generic;
using System.Linq;
using CSVQueryLanguage.Analysis;
using CSVQueryLanguage.Parser.Tree;
using CSVQueryLanguage.Plan.Nodes;

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
        var scope = _context.Scopes[query];
        var names = ResolveOutputColumnNames(scope.RelationInfo);

        PlanNode plan = null;

        if (query.From is not null)
            plan = PlanRelation(query.From);

        plan = PlanWhere(query.Where, plan);
        plan = PlanLimit(query.Limit, plan);
        plan = PlanSelect(query.Select, plan);

        return new OutputNode(plan, names);
    }

    private PlanNode PlanWhere(IExpression where, PlanNode source)
    {
        if (where is null)
            return source;

        var predicate = ExpressionRewriter.Rewrite(where);
    }

    private PlanNode PlanLimit(Limit limit, PlanNode source)
    {
        throw new NotImplementedException();
    }

    private PlanNode PlanSelect(Select select, PlanNode source)
    {
    }

    private PlanNode PlanRelation(IRelation relation)
    {
        var plan = relation switch
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

    private string[] ResolveOutputColumnNames(RelationInfo info)
    {
        var names = info.Fields
            .Select(x => x.Name)
            .ToArray();

        int colIndex = 0;

        for (int i = 0; i < names.Length; i++)
        {
            if (!string.IsNullOrEmpty(names[i]))
                continue;

            while (names.Contains($"_col{colIndex}"))
                colIndex++;

            names[i] = $"_col{colIndex++}";
        }

        return names;
    }
}

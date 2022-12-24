using System;
using CSVQueryLanguage.Parser.Tree;

namespace CSVQueryLanguage.Analysis;

internal sealed class QueryAnalyzer
{
    public AnalyzerScope AnalyzeQuery(Query query, AnalyzerContext context)
    {
        AnalyzerScope scope = null;

        if (query.From is not null)
            scope = AnalyzeAliasedRelation(query.From, context);

        // TODO: node.Select
        // TODO: node.Where
        // TODO: node.Limit

        return scope;
    }

    private AnalyzerScope AnalyzeRelation(IRelation relation, AnalyzerContext context)
    {
        switch (relation)
        {
            case AliasedRelation aliasedRelation:
                return AnalyzeAliasedRelation(aliasedRelation, context);

            case CsvRelation csvRelation:
                return AnalyzeCsvRelation(csvRelation, context);

            case SubqueryRelation subqueryRelation:
                return AnalyzeSubqueryRelation(subqueryRelation, context);

            default:
                throw new ArgumentOutOfRangeException(nameof(relation));
        }
    }

    private AnalyzerScope AnalyzeAliasedRelation(AliasedRelation aliasedRelation, AnalyzerContext context)
    {
        var scope = AnalyzeRelation(aliasedRelation.Relation, context);

        if (aliasedRelation.Alias is null)
            return scope;

        var aliasedRelationInfo = new RelationInfo(
            aliasedRelation,
            aliasedRelation.Alias.Value,
            scope.RelationInfo.Fields
        );

        return new AnalyzerScope(context, scope, aliasedRelationInfo);
    }

    private AnalyzerScope AnalyzeCsvRelation(CsvRelation csvRelation, AnalyzerContext context)
    {
        csvRelation.FileName
    }

    private AnalyzerScope AnalyzeSubqueryRelation(SubqueryRelation subqueryRelation, AnalyzerContext context)
    {
        return AnalyzeQuery(subqueryRelation.Query, context);
    }
}

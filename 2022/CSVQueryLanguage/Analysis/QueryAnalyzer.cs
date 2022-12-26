using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using CSVQueryLanguage.Parser.Tree;
using Sylvan.Data.Csv;

namespace CSVQueryLanguage.Analysis;

internal sealed class QueryAnalyzer
{
    public QueryScope AnalyzeQuery(Query query, AnalyzerContext context)
    {
        QueryScope scope = null;

        if (query.From is not null)
            scope = AnalyzeAliasedRelation(query.From, context);

        RelationFieldInfo[] outputFields = AnalyzeSelect(query.Select, context, scope);
        var outputRelation = new RelationInfo(query, null, outputFields);

        // TODO: query.Where

        scope = new QueryScope(context, scope, outputRelation);
        context.Scopes[query] = scope;

        return scope;
    }

    private QueryScope AnalyzeRelation(IRelation relation, AnalyzerContext context)
    {
        var scope = relation switch
        {
            AliasedRelation aliasedRelation => AnalyzeAliasedRelation(aliasedRelation, context),
            CsvRelation csvRelation => AnalyzeCsvRelation(csvRelation, context),
            SubqueryRelation subqueryRelation => AnalyzeSubqueryRelation(subqueryRelation, context),
            _ => throw new ArgumentOutOfRangeException(nameof(relation))
        };

        context.Scopes[relation] = scope;

        return scope;
    }

    private QueryScope AnalyzeAliasedRelation(AliasedRelation aliasedRelation, AnalyzerContext context)
    {
        var scope = AnalyzeRelation(aliasedRelation.Relation, context);

        if (aliasedRelation.Alias is null)
            return scope;

        var aliasedRelationInfo = new RelationInfo(
            aliasedRelation,
            aliasedRelation.Alias.Value,
            scope.RelationInfo.Fields
        );

        return new QueryScope(context, scope.Parent, aliasedRelationInfo);
    }

    private QueryScope AnalyzeCsvRelation(CsvRelation csvRelation, AnalyzerContext context)
    {
        var csvFileName = csvRelation.FileName.Value;
        StreamReader csvStreamReader;

        try
        {
            csvStreamReader = File.OpenText(csvFileName);
        }
        catch (FileNotFoundException)
        {
            throw new CqlException($"{csvFileName} not found");
        }

        using var csvReader = CsvDataReader.Create(csvStreamReader);

        var relationFields = new RelationFieldInfo[csvReader.FieldCount];
        var relationInfo = new RelationInfo(csvRelation, csvFileName, relationFields);

        for (int i = 0; i < relationFields.Length; i++)
        {
            var fieldName = csvReader.GetName(i);
            relationFields[i] = new RelationFieldInfo(new FieldReference(i), fieldName);
        }

        return new QueryScope(context, null, relationInfo);
    }

    private QueryScope AnalyzeSubqueryRelation(SubqueryRelation subqueryRelation, AnalyzerContext context)
    {
        return AnalyzeQuery(subqueryRelation.Query, context);
    }

    private RelationFieldInfo[] AnalyzeSelect(Select select, AnalyzerContext context, [AllowNull] QueryScope scope)
    {
        var relationFields = new List<RelationFieldInfo>(select.Items.Count);

        foreach (var selectItem in select.Items)
            relationFields.AddRange(AnalyzeSelectItem(selectItem, context, scope));

        return relationFields.ToArray();
    }

    private IEnumerable<RelationFieldInfo> AnalyzeSelectItem(ISelectItem selectItem, AnalyzerContext context, [AllowNull] QueryScope scope)
    {
        return selectItem switch
        {
            AllColumns allColumns => AnalyzeAllColumns(allColumns, context, scope),
            SingleColumn singleColumn => new[] { AnalyzeSingleColumn(singleColumn, context, scope) },
            _ => throw new ArgumentOutOfRangeException(nameof(selectItem))
        };
    }

    private IEnumerable<RelationFieldInfo> AnalyzeAllColumns(AllColumns allColumns, AnalyzerContext context, [AllowNull] QueryScope scope)
    {
        if (scope?.RelationInfo is null)
            throw new CqlException("No table");

        var relationInfo = scope.RelationInfo;

        if (allColumns.Prefix is { } prefix && relationInfo.Name != prefix.Value)
            throw new CqlException($"{prefix.OriginalValue}.* not found");

        for (int i = 0; i < relationInfo.Fields.Length; i++)
        {
            var field = relationInfo.Fields[i];
            yield return new RelationFieldInfo(new FieldReference(i), field.Name);
        }
    }

    private RelationFieldInfo AnalyzeSingleColumn(SingleColumn singleColumn, AnalyzerContext context, [AllowNull] QueryScope scope)
    {
        var expression = singleColumn.Expression;
        string name = singleColumn.Alias?.Value;

        if (expression is ColumnReference colRef)
        {
            if (scope is null)
                throw new CqlException("No table");

            var field = scope.ResolveField(colRef.Name);
            var fieldIndex = Array.IndexOf(scope.RelationInfo.Fields, field);

            return new RelationFieldInfo(new FieldReference(fieldIndex), name ?? field.Name);
        }

        return new RelationFieldInfo(expression, name);
    }
}

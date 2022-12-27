using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CSVQueryLanguage.Driver.Cursors;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Analysis;

internal sealed class QueryAnalyzer
{
    private readonly AnalyzerContext _context;
    private readonly ExpressionAnalyzer _expressionAnalyzer;

    public QueryAnalyzer(AnalyzerContext context)
    {
        _context = context;
        _expressionAnalyzer = new ExpressionAnalyzer(context);
    }

    public QueryScope AnalyzeQuery(Query query)
    {
        QueryScope scope = null;

        if (query.From is not null)
            scope = AnalyzeAliasedRelation(query.From);

        RelationFieldInfo[] fields = AnalyzeSelect(query.Select, scope);
        var info = new RelationInfo(query, null, fields);
        IExpression filter = null;

        if (query.Where is not null)
            filter = _expressionAnalyzer.Analyze(query.Where, scope);

        scope = new QueryScope(_context, scope, info, filter);
        _context.Scopes[query] = scope;

        return scope;
    }

    private QueryScope AnalyzeRelation(IRelation relation)
    {
        var scope = relation switch
        {
            AliasedRelation aliasedRelation => AnalyzeAliasedRelation(aliasedRelation),
            CsvRelation csvRelation => AnalyzeCsvRelation(csvRelation),
            SubqueryRelation subqueryRelation => AnalyzeSubqueryRelation(subqueryRelation),
            _ => throw new ArgumentOutOfRangeException(nameof(relation))
        };

        _context.Scopes[relation] = scope;

        return scope;
    }

    private QueryScope AnalyzeAliasedRelation(AliasedRelation aliasedRelation)
    {
        var scope = AnalyzeRelation(aliasedRelation.Relation);

        if (aliasedRelation.Alias is null)
            return scope;

        var aliasedRelationInfo = new RelationInfo(
            aliasedRelation,
            aliasedRelation.Alias.Value,
            scope.RelationInfo.Fields
        );

        return new QueryScope(scope.Context, scope.Parent, aliasedRelationInfo, scope.Filter);
    }

    private QueryScope AnalyzeCsvRelation(CsvRelation csvRelation)
    {
        var csvFileName = csvRelation.FileName.Value;

        using var csv = CsvCursor.OpenRead(csvFileName);

        var relationFields = new RelationFieldInfo[csv.FieldCount];
        var relationInfo = new RelationInfo(csvRelation, csvFileName, relationFields);

        for (int i = 0; i < relationFields.Length; i++)
        {
            var fieldName = csv.GetName(i);
            relationFields[i] = new RelationFieldInfo(new FieldReference(i), DataType.Text, fieldName);
        }

        return new QueryScope(_context, null, relationInfo, null);
    }

    private QueryScope AnalyzeSubqueryRelation(SubqueryRelation subqueryRelation)
    {
        return AnalyzeQuery(subqueryRelation.Query);
    }

    private RelationFieldInfo[] AnalyzeSelect(Select select, [AllowNull] QueryScope scope)
    {
        var relationFields = new List<RelationFieldInfo>(select.Items.Count);
        var fieldNameResolver = new FieldNameResolver();

        if (scope is not null)
            fieldNameResolver.Add(scope);

        foreach (var selectItem in select.Items)
        {
            switch (selectItem)
            {
                case AllColumns allColumns:
                    foreach (var field in AnalyzeAllColumns(allColumns, scope))
                    {
                        fieldNameResolver.Add(field.Name);
                        relationFields.Add(field);
                    }

                    break;

                case SingleColumn singleColumn:
                    (var expression, DataType? type, var name) = AnalyzeSingleColumn(singleColumn, scope);
                    name ??= fieldNameResolver.GenerateColumnName();

                    relationFields.Add(new RelationFieldInfo(expression, type, name));
                    break;

                default:
                    throw new NotSupportedException(selectItem.GetType().Name);
            }
        }

        return relationFields.ToArray();
    }

    private IEnumerable<RelationFieldInfo> AnalyzeAllColumns(AllColumns allColumns, [AllowNull] QueryScope scope)
    {
        if (scope is null)
            throw CqlErrors.RelationNotFound();

        var relationInfo = scope.RelationInfo;

        if (allColumns.Prefix is { } prefix && relationInfo.Name != prefix.Value)
            throw new CqlException($"{prefix.OriginalValue}.* not found");

        for (int i = 0; i < relationInfo.Fields.Length; i++)
        {
            var field = relationInfo.Fields[i];
            yield return new RelationFieldInfo(new FieldReference(i), field.Type, field.Name);
        }
    }

    private (IExpression Expression, DataType? Type, string Name) AnalyzeSingleColumn(SingleColumn singleColumn, [AllowNull] QueryScope scope)
    {
        var expression = singleColumn.Expression;
        var name = singleColumn.Alias?.Value;

        if (expression is ColumnReference colRef)
        {
            if (scope is null)
                throw new CqlException("No table");

            var field = scope.ResolveField(colRef.Name);
            var fieldIndex = scope.ResolveFieldIndex(field);

            return (new FieldReference(fieldIndex), field.Type, name ?? field.Name);
        }

        // Analyze & Rewrite
        expression = _expressionAnalyzer.Analyze(expression, scope);
        DataType? type = _context.ExpressionTypes[expression];

        return (expression, type, name);
    }

    private sealed class FieldNameResolver
    {
        private readonly HashSet<string> _names = new();
        private int _colIndex;

        public void Add(QueryScope scope)
        {
            foreach (var field in scope.RelationInfo.Fields)
                _names.Add(field.Name);
        }

        public void Add(string name)
        {
            _names.Add(name);
        }

        public string GenerateColumnName()
        {
            while (_names.Contains($"_col{_colIndex}"))
                _colIndex++;

            var name = $"_col{_colIndex++}";

            _names.Add(name);

            return name;
        }
    }
}

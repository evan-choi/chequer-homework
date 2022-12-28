using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Analysis;

public sealed class QueryScope
{
    public AnalyzerContext Context { get; }

    public QueryScope Parent { get; }

    public RelationInfo RelationInfo { get; }

    public IExpression Filter { get; }

    public Dictionary<VariableInfo, IExpression> AggregateVariables { get; } = new();

    public QueryScope(
        AnalyzerContext context,
        [AllowNull] QueryScope parent,
        RelationInfo relationInfo,
        [AllowNull] IExpression filter)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(relationInfo);

        Context = context;
        Parent = parent;
        RelationInfo = relationInfo;
        Filter = filter;
    }

    public RelationFieldInfo ResolveField(QualifiedName name)
    {
        switch (name.Parts.Length)
        {
            case > 2:
                throw new CqlException($"Invalid column {name}");

            case 2:
                var target = name.Parts[0];

                if (RelationInfo.Name != target.Value)
                    throw CqlErrors.RelationNotFound(target.OriginalValue);

                break;
        }

        return ResolveField(name.Parts[^1].Value);
    }

    public RelationFieldInfo ResolveField(string columnName)
    {
        return RelationInfo.Fields.FirstOrDefault(x => x.Name == columnName)
               ?? throw CqlErrors.ColumnNotFound(columnName);
    }

    public int ResolveFieldIndex(RelationFieldInfo field)
    {
        var index = Array.IndexOf(RelationInfo.Fields, field);

        if (index < 0)
            throw CqlErrors.ColumnNotFound(field.Name);

        return index;
    }
}

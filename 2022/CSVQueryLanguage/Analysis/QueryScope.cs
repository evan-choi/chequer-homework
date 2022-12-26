using System.Linq;
using CSVQueryLanguage.Parser.Tree;

namespace CSVQueryLanguage.Analysis;

public sealed class QueryScope
{
    public AnalyzerContext Context { get; }

    public QueryScope Parent { get; }

    public RelationInfo RelationInfo { get; }

    public QueryScope(AnalyzerContext context)
    {
        Context = context;
    }

    public QueryScope(AnalyzerContext context, QueryScope parent, RelationInfo relationInfo)
    {
        Context = context;
        Parent = parent;
        RelationInfo = relationInfo;
    }

    public RelationFieldInfo ResolveField(QualifiedName name)
    {
        if (RelationInfo is null)
            throw new CqlException("No table");

        switch (name.Parts.Length)
        {
            case > 2:
                throw new CqlException($"Invalid column {name}");

            case 2:
                var target = name.Parts[0];

                if (RelationInfo.Name != target.Value)
                    throw new CqlException($"Table {target.OriginalValue} not found");

                break;
        }

        var columnName = name.Parts[^1].Value;

        return RelationInfo.Fields.FirstOrDefault(x => x.Name == columnName)
               ?? throw new CqlException($"{columnName} column not found");
    }
}

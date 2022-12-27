using System;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Analysis;

public sealed class RelationFieldInfo
{
    public IExpression Source { get; }

    public DataType? Type { get; }

    public string Name { get; }

    public RelationFieldInfo(IExpression source, DataType? type, string name)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrEmpty(name);

        Source = source;
        Type = type;
        Name = name;
    }
}

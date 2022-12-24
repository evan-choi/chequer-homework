using System;
using CSVQueryLanguage.Parser.Tree;

namespace CSVQueryLanguage.Analysis;

public sealed class RelationFieldInfo
{
    public INode Source { get; }

    public string Name { get; }

    public RelationFieldInfo(INode source, string name)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrEmpty(name);

        Source = source;
        Name = name;
    }
}

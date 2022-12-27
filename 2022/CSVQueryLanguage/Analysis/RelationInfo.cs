using System;
using System.Diagnostics;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Analysis;

[DebuggerDisplay("Source={Source}, Name={Name}")]
public sealed class RelationInfo
{
    public INode Source { get; }

    public string Name { get; }

    public RelationFieldInfo[] Fields { get; }

    public RelationInfo(INode source, string name, RelationFieldInfo[] fields)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(fields);

        Source = source;
        Name = name;
        Fields = fields;
    }
}

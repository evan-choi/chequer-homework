using System;
using System.Diagnostics.CodeAnalysis;
using CSVQueryLanguage.Parser.Tree;

namespace CSVQueryLanguage.Analysis;

public sealed class RelationFieldInfo
{
    public IExpression Source { get; }

    public string Name { get; }

    public RelationFieldInfo(IExpression source, [AllowNull] string name)
    {
        ArgumentNullException.ThrowIfNull(source);

        Source = source;
        Name = name;
    }
}

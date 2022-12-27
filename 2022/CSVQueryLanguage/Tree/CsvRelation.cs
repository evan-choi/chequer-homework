using System;
using System.Diagnostics;

namespace CSVQueryLanguage.Tree;

// RULE: relationPrimary > #csv
[DebuggerDisplay("{Parser.CqlDeparser.Deparse(this)}")]
public sealed class CsvRelation : IRelation
{
    public Identifier FileName { get; }

    public CsvRelation(Identifier fileName)
    {
        ArgumentNullException.ThrowIfNull(fileName);

        FileName = fileName;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitCsvRelation(this);
    }
}

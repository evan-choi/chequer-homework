using System.Diagnostics;

namespace CSVQueryLanguage.Tree;

// RULE: selectItem > #selectAll
[DebuggerDisplay("{Parser.CqlDeparser.Deparse(this)}")]
public sealed class AllColumns : ISelectItem
{
    public Identifier Prefix { get; }

    public AllColumns()
    {
    }

    public AllColumns(Identifier prefix)
    {
        Prefix = prefix;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitAllColumns(this);
    }
}

using System.Diagnostics;

namespace CSVQueryLanguage.Parser.Tree;

// RULE: selectItem > #selectAll
[DebuggerDisplay("{CqlDeparser.Deparse(this)}")]
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

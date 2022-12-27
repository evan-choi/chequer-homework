using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CSVQueryLanguage.Tree;

// RULE: querySpecification > _items
[DebuggerDisplay("Count = {Items.Count}")]
[DebuggerTypeProxy(typeof(SelectDebugView))]
public sealed class Select : INode
{
    public List<ISelectItem> Items { get; }

    public Select(IEnumerable<ISelectItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        Items = new List<ISelectItem>(items);
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitSelect(this);
    }

    private sealed class SelectDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public ISelectItem[] Items { get; }

        public SelectDebugView(Select select)
        {
            Items = select.Items.ToArray();
        }
    }
}

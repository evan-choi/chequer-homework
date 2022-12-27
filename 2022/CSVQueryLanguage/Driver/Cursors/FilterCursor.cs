using CSVQueryLanguage.Plan.Nodes;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Driver.Cursors;

public sealed class FilterCursor : CursorBase
{
    private readonly IExpression _predicate;

    public FilterCursor(ICursor source, ExecutionContext context, FilterNode node) : base(source, context)
    {
        _predicate = node.Predicate;
    }

    public override bool Read()
    {
        while (Source.Read())
        {
            if (Context.Interactive.Interpret<bool>(_predicate, Source))
                return true;
        }

        return false;
    }
}

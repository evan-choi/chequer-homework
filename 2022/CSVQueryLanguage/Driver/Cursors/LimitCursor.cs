using CSVQueryLanguage.Plan.Nodes;

namespace CSVQueryLanguage.Driver.Cursors;

public sealed class LimitCursor : CursorBase
{
    private long _count;

    public LimitCursor(ICursor source, ExecutionContext context, LimitNode node) : base(source, context)
    {
        _count = node.Count;
    }

    public override bool Read()
    {
        if (_count > 0)
        {
            if (base.Read())
            {
                _count--;
                return true;
            }

            _count = 0;
        }

        return false;
    }
}

using CSVQueryLanguage.Plan.Nodes;

namespace CSVQueryLanguage.Driver.Cursors;

public sealed class OffsetCursor : CursorBase
{
    private readonly long _count;
    private bool _read;

    public OffsetCursor(ICursor source, ExecutionContext context, OffsetNode node) : base(source, context)
    {
        _count = node.Count;
    }

    public override bool Read()
    {
        if (!_read)
        {
            _read = true;

            for (int i = 0; i < _count; i++)
            {
                if (!Source.Read())
                    return false;
            }
        }

        return Source.Read();
    }
}

using CSVQueryLanguage.Plan.Nodes;

namespace CSVQueryLanguage.Driver.Cursors;

public class AggregateCursor : CursorBase
{
    private bool _read;
    private ulong _count;
    private readonly IRuntimeVariable<double> _countVariable;

    public AggregateCursor(ICursor source, ExecutionContext context, AggregateNode node) : base(source, context)
    {
        if (node.CountVariable is not null)
            _countVariable = (IRuntimeVariable<double>)context.Variables[node.CountVariable.Name];
    }

    public override bool Read()
    {
        if (_read)
            return false;

        _read = true;

        while (Source.Read())
            _count++;

        if (_countVariable is not null)
            _countVariable.Value = _count;

        return true;
    }
}

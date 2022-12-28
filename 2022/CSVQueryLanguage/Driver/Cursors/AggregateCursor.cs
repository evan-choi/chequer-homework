using System.Linq;
using CSVQueryLanguage.Plan.Nodes;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Driver.Cursors;

public class AggregateCursor : CursorBase
{
    private readonly RuntimeVariable[] _variables;
    private readonly IExpression[] _expressions;
    private bool _read;

    public AggregateCursor(ICursor source, ExecutionContext context, AggregateNode node) : base(source, context)
    {
        _variables = node.Variables.Select(x => context.Variables[x.Key.Name]).ToArray();
        _expressions = node.Variables.Select(x => x.Value).ToArray();
    }

    public override bool Read()
    {
        if (_read)
            return false;

        _read = true;

        while (Source.Read())
        {
            for (int i = 0; i < _variables.Length; i++)
                _variables[i].Value = Context.Interactive.Interpret<object>(_expressions[i], Source);
        }

        return true;
    }
}

using System;
using System.Runtime.CompilerServices;
using CSVQueryLanguage.Plan.Nodes;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Driver.Cursors;

public sealed class ProjectCursor : ICursor
{
    public int FieldCount => _names.Length;

    private readonly ICursor _source;
    private readonly ExecutionContext _context;
    private readonly IExpression[] _expressions;
    private readonly DataType?[] _types;
    private readonly string[] _names;
    private readonly object[] _values;
    private readonly bool[] _cache;

    private bool _read;

    public ProjectCursor(ICursor source, ExecutionContext context, ProjectNode node)
    {
        _source = source;
        _context = context;
        _expressions = node.Expressions;
        _types = node.DataTypes;
        _names = node.Names;

        _values = new object[_names.Length];
        _cache = new bool[_names.Length];
    }

    public string GetName(int ordinal)
    {
        return _names[ordinal];
    }

    public bool Read()
    {
        if (_source is null)
        {
            if (_read)
                return false;

            _read = true;
            return true;
        }

        _cache.AsSpan().Clear();

        return _source.Read();
    }

    public bool IsNull(int ordinal)
    {
        if (!_types[ordinal].HasValue)
            return false;

        return GetValue(ordinal) is null;
    }

    public object GetValue(int ordinal)
    {
        ref var cache = ref _cache[ordinal];
        ref var value = ref _values[ordinal];

        if (!cache)
        {
            cache = true;
            value = _context.Interactive.Interpret<object>(_expressions[ordinal], _source);
        }

        return value;
    }

    public string GetText(int ordinal)
    {
        VerifyDataType(_types[ordinal], DataType.Text);
        return (string)GetValue(ordinal);
    }

    public double GetNumber(int ordinal)
    {
        VerifyDataType(_types[ordinal], DataType.Number);
        return (double)GetValue(ordinal);
    }

    public DateOnly GetDate(int ordinal)
    {
        VerifyDataType(_types[ordinal], DataType.Date);
        return (DateOnly)GetValue(ordinal);
    }

    public TimeOnly GetTime(int ordinal)
    {
        VerifyDataType(_types[ordinal], DataType.Time);
        return (TimeOnly)GetValue(ordinal);
    }

    public DateTimeOffset GetTimestamp(int ordinal)
    {
        VerifyDataType(_types[ordinal], DataType.Timestamp);
        return (DateTimeOffset)GetValue(ordinal);
    }

    public bool GetBoolean(int ordinal)
    {
        VerifyDataType(_types[ordinal], DataType.Boolean);
        return (bool)GetValue(ordinal);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void VerifyDataType(DataType? actual, DataType expected)
    {
        if (actual.HasValue && actual.Value != expected)
            throw new InvalidOperationException();
    }

    public void Dispose()
    {
        _source?.Dispose();
    }
}

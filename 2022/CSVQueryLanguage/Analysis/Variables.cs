using System;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Analysis;

public sealed class VariableInfo
{
    public INode Source { get; }

    public string Name { get; }

    public DataType Type { get; }

    public object Default
    {
        get
        {
            return Type switch
            {
                DataType.Text => default(string),
                DataType.Number => default(double),
                DataType.Date => default(DateOnly),
                DataType.Time => default(TimeOnly),
                DataType.Timestamp => default(DateTimeOffset),
                DataType.Boolean => default(bool),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public VariableInfo(INode source, string name, DataType type)
    {
        Source = source;
        Name = name;
        Type = type;
    }
}

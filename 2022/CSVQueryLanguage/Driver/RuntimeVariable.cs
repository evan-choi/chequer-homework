using CSVQueryLanguage.Analysis;

namespace CSVQueryLanguage.Driver;

public sealed class RuntimeVariable<T> : IRuntimeVariable<T>
{
    public VariableInfo Info { get; }

    public T Value { get; set; }

    public RuntimeVariable(VariableInfo info)
    {
        Info = info;
        Value = default;
    }
}

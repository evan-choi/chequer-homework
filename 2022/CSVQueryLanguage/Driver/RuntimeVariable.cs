using CSVQueryLanguage.Analysis;

namespace CSVQueryLanguage.Driver;

public sealed class RuntimeVariable
{
    public VariableInfo Info { get; }

    public object Value { get; set; }

    public RuntimeVariable(VariableInfo info)
    {
        Info = info;
        Value = info.Default;
    }
}

using CSVQueryLanguage.Driver.Interpretation;

namespace CSVQueryLanguage.Driver;

public sealed class ExecutionContext
{
    // TODO: aggregate variables

    public Interactive Interactive { get; }

    public ExecutionContext()
    {
        Interactive = new Interactive(this);
    }
}

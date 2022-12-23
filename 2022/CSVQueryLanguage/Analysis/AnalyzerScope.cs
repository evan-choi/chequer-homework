namespace CSVQueryLanguage.Analysis;

public sealed class AnalyzerScope
{
    public AnalyzerScope Parent { get; }

    public AnalyzerContext Context { get; }

    public AnalyzerScope(AnalyzerContext context)
    {
        Context = context;
    }

    private AnalyzerScope(AnalyzerContext context, AnalyzerScope parent) : this(context)
    {
        Parent = parent;
    }

    public AnalyzerScope CreateChildScope()
    {
        return new AnalyzerScope(Context, this);
    }
}

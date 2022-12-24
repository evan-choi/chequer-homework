namespace CSVQueryLanguage.Analysis;

internal sealed class AnalyzerScope
{
    public AnalyzerContext Context { get; }

    public AnalyzerScope Parent { get; }

    public RelationInfo RelationInfo { get; }

    public AnalyzerScope(AnalyzerContext context)
    {
        Context = context;
    }

    public AnalyzerScope(AnalyzerContext context, AnalyzerScope parent, RelationInfo relationInfo)
    {
        Context = context;
        Parent = parent;
        RelationInfo = relationInfo;
    }
}

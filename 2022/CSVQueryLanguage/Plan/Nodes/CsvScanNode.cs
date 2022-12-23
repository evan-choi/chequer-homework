namespace CSVQueryLanguage.Plan.Nodes;

public sealed class CsvScanNode : PlanNode
{
    public string FileName { get; }

    public string[] Columns { get; }

    public CsvScanNode(string fileName, string[] columns)
    {
        FileName = fileName;
        Columns = columns;
    }
}

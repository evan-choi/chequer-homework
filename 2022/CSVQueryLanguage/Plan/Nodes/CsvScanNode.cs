namespace CSVQueryLanguage.Plan.Nodes;

public sealed class CsvScanNode : PlanNode
{
    public string FileName { get; }

    public string[] ColumnNames { get; }

    public CsvScanNode(string fileName, string[] columnNames)
    {
        FileName = fileName;
        ColumnNames = columnNames;
    }
}

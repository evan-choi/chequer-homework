using System;

namespace CSVQueryLanguage.Analysis;

public sealed class Field
{
    public string CsvFileName { get; }

    public string ColumnName { get; }

    public string Name { get; }

    public Field(string csvFileName, string columnName, string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(csvFileName);
        ArgumentException.ThrowIfNullOrEmpty(columnName);
        ArgumentException.ThrowIfNullOrEmpty(name);

        CsvFileName = csvFileName;
        ColumnName = columnName;
        Name = name;
    }
}

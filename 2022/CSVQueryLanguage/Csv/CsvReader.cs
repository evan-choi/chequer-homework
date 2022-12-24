using System;
using System.IO;

namespace CSVQueryLanguage.Csv;

internal sealed class CsvReader : IDisposable
{
    public int FieldCount => _fields?.Length ?? 0;

    private readonly StreamReader _reader;
    private CsvField[] _fields;

    public CsvReader(StreamReader reader)
    {
        _reader = reader;
    }

    public bool ScanNext()
    {
        int read;

        while ((read = _reader.Read()) != -1)
        {
            var ch = (char)read;

            
        }
    }

    public ReadOnlySpan<char> GetValueSpan(int index)
    {
    }

    public string GetValue(int index)
    {
        return GetValueSpan(index).ToString();
    }

    public void Dispose()
    {
        _reader.Dispose();
    }

    private readonly struct CsvField
    {
        public Range Range { get; }

        public bool IsEscaped { get; }

        public CsvField(Range range, bool isEscaped)
        {
            Range = range;
            IsEscaped = isEscaped;
        }
    }
}

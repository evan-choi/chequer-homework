using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32.SafeHandles;
using Sylvan.Data.Csv;

namespace CSVQueryLanguage.Driver.Cursors;

public sealed class CsvCursor : ICursor, IDisposable
{
    public string FileName { get; }

    public int FieldCount => _fieldNames.Length;

    public int RowNumber => _reader.RowNumber;

    private readonly SafeFileHandle _fileHandle;
    private readonly CsvDataReader _reader;
    private readonly string[] _fieldNames;

    private CsvCursor(string fileName, SafeFileHandle fileHandle)
    {
        FileName = fileName;
        _fileHandle = fileHandle;

        var fileStream = new FileStream(fileHandle, FileAccess.Read);
        var streamReader = new StreamReader(fileStream);
        _reader = CsvDataReader.Create(streamReader);

        var names = new string[_reader.FieldCount];
        var duplicateNames = new HashSet<string>();

        for (int i = 0; i < _reader.FieldCount; i++)
        {
            var name = _reader.GetName(i);
            names[i] = name;

            if (Array.IndexOf(names, name, 0, i) >= 0)
                duplicateNames.Add(name);
        }

        // BEFORE: id, name, name
        //  AFTER: id, name[0], name[1]
        foreach (var duplicateName in duplicateNames)
        {
            Span<string> namePtr = names.AsSpan();
            int ordinal;
            var index = 0;

            while ((ordinal = namePtr.IndexOf(duplicateName)) >= 0)
            {
                namePtr[ordinal] = $"{duplicateName}[{index++}]";
                namePtr = namePtr[(ordinal + 1)..];
            }
        }

        _fieldNames = names;
    }

    public string GetName(int ordinal)
    {
        return _fieldNames[ordinal];
    }

    public bool Read()
    {
        return _reader.Read();
    }

    public bool IsNull(int ordinal)
    {
        return _reader.IsDBNull(ordinal);
    }

    public object GetValue(int ordinal)
    {
        return GetText(ordinal);
    }

    public string GetText(int ordinal)
    {
        return _reader.GetString(ordinal);
    }

    public double GetNumber(int ordinal)
    {
        return _reader.GetDouble(ordinal);
    }

    public DateOnly GetDate(int ordinal)
    {
        return _reader.GetDate(ordinal);
    }

    public TimeOnly GetTime(int ordinal)
    {
        return _reader.GetTime(ordinal);
    }

    public DateTimeOffset GetTimestamp(int ordinal)
    {
        return _reader.GetDateTimeOffset(ordinal);
    }

    public bool GetBoolean(int ordinal)
    {
        return _reader.GetBoolean(ordinal);
    }

    public void Close()
    {
        _fileHandle.Dispose();
        _reader.Dispose();
    }

    public void Dispose()
    {
        Close();
    }

    public static CsvCursor OpenRead(string fileName)
    {
        try
        {
            var fileHandle = File.OpenHandle(fileName);
            return new CsvCursor(fileName, fileHandle);
        }
        catch (FileNotFoundException)
        {
            throw CqlErrors.RelationNotFound(fileName);
        }
    }

    public static CsvCursor OpenReadLocked(string fileName)
    {
        try
        {
            var fileHandle = File.OpenHandle(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            return new CsvCursor(fileName, fileHandle);
        }
        catch (FileNotFoundException)
        {
            throw CqlErrors.RelationNotFound(fileName);
        }
    }
}

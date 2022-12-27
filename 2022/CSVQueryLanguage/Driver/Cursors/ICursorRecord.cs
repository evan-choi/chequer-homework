using System;

namespace CSVQueryLanguage.Driver.Cursors;

public interface ICursorRecord
{
    int FieldCount { get; }

    string GetName(int ordinal);

    bool IsNull(int ordinal);

    object GetValue(int ordinal);

    string GetText(int ordinal);

    double GetNumber(int ordinal);

    DateOnly GetDate(int ordinal);

    TimeOnly GetTime(int ordinal);

    DateTimeOffset GetTimestamp(int ordinal);

    bool GetBoolean(int ordinal);
}

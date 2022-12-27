using System;

namespace CSVQueryLanguage.Driver.Cursors;

public abstract class CursorBase : ICursor
{
    protected ICursor Source { get; }

    protected ExecutionContext Context { get; }

    public virtual int FieldCount => Source.FieldCount;

    protected CursorBase(ICursor source, ExecutionContext context)
    {
        Source = source;
        Context = context;
    }

    public virtual string GetName(int ordinal)
    {
        return Source.GetName(ordinal);
    }

    public virtual bool Read()
    {
        return Source.Read();
    }

    public virtual bool IsNull(int ordinal)
    {
        return Source.IsNull(ordinal);
    }

    public virtual object GetValue(int ordinal)
    {
        return Source.GetValue(ordinal);
    }

    public virtual string GetText(int ordinal)
    {
        return Source.GetText(ordinal);
    }

    public virtual double GetNumber(int ordinal)
    {
        return Source.GetNumber(ordinal);
    }

    public virtual DateOnly GetDate(int ordinal)
    {
        return Source.GetDate(ordinal);
    }

    public virtual TimeOnly GetTime(int ordinal)
    {
        return Source.GetTime(ordinal);
    }

    public virtual DateTimeOffset GetTimestamp(int ordinal)
    {
        return Source.GetTimestamp(ordinal);
    }

    public virtual bool GetBoolean(int ordinal)
    {
        return Source.GetBoolean(ordinal);
    }

    public virtual void Dispose()
    {
        Source?.Dispose();
        GC.SuppressFinalize(this);
    }
}

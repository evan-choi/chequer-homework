using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace CSVQueryLanguage.Parser;

internal readonly struct UpperCaseCharStream : ICharStream
{
    public int Index => _charStream.Index;

    public int Size => _charStream.Size;

    public string SourceName => _charStream.SourceName;

    private readonly ICharStream _charStream;

    public UpperCaseCharStream(ICharStream charStream)
    {
        _charStream = charStream;
    }

    public void Consume()
    {
        _charStream.Consume();
    }

    public int La(int i)
    {
        var result = _charStream.La(i);

        if (result is 0 or IntStreamConstants.Eof)
            return result;

        return char.ToUpper((char)result);
    }

    public int Mark()
    {
        return _charStream.Mark();
    }

    public void Release(int marker)
    {
        _charStream.Release(marker);
    }

    public void Seek(int index)
    {
        _charStream.Seek(index);
    }

    public string GetText(Interval interval)
    {
        return _charStream.GetText(interval);
    }
}

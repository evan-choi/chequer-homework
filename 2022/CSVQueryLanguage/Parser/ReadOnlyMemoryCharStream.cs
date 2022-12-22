using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace CSVQueryLanguage.Parser;

internal sealed class ReadOnlyMemoryCharStream : ICharStream
{
    public int Index { get; private set; }

    public int Size => _input.Length;

    public string SourceName => "<unknown>";

    private readonly ReadOnlyMemory<char> _input;

    public ReadOnlyMemoryCharStream(ReadOnlyMemory<char> input)
    {
        _input = input;
    }

    public void Consume()
    {
        if (Index >= Size)
            throw new InvalidOperationException("cannot consume EOF");

        Index++;
    }

    public int La(int i)
    {
        switch (i)
        {
            case 0:
                return 0;

            case < 0:
            {
                ++i;

                if (Index + i - 1 < 0)
                    return -1;

                break;
            }
        }

        return Index + i - 1 >= Size
            ? -1
            : _input.Span[Index + i - 1];
    }

    public int Mark()
    {
        return -1;
    }

    public void Release(int marker)
    {
    }

    public void Seek(int index)
    {
        Index = index > Index
            ? Math.Min(index, Size)
            : index;
    }

    public string GetText(Interval interval)
    {
        var start = interval.a;
        var end = interval.b + 1; // inclusive to exclusive

        if (end > Size)
            end = Size;

        return start >= Size 
            ? string.Empty
            : _input.Span[start..end].ToString();
    }
}

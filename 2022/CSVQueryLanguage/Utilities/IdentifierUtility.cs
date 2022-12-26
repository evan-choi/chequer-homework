using System;

namespace CSVQueryLanguage.Utilities;

public static class IdentifierUtility
{
    public static bool IsDoubleQuoted(ReadOnlySpan<char> value)
    {
        return value is ['"', .., '"'];
    }

    public static bool IsSingleQuoted(ReadOnlySpan<char> value)
    {
        return value is ['\'', .., '\''];
    }

    #region Unescape
    public static string UnescapeDoubleQuotes(ReadOnlySpan<char> value)
    {
        if (!IsDoubleQuoted(value))
            throw InvalidDoubleQuotedStringException(value);

        if (value.Length < 256)
        {
            if (!TryUnescapeQuotesFast(value, '"', out var unescapedValue))
                throw InvalidDoubleQuotedStringException(value);

            return unescapedValue;
        }
        else
        {
            if (!TryUnescapeQuotesSlow(value, '"', out var unescapedValue))
                throw InvalidDoubleQuotedStringException(value);

            return unescapedValue;
        }
    }

    public static string UnescapeSingleQuotes(string value)
    {
        if (!IsSingleQuoted(value))
            throw InvalidSingleQuotedStringException(value);

        if (value.Length <= 256)
        {
            if (!TryUnescapeQuotesFast(value, '\'', out var unescapedValue))
                throw InvalidSingleQuotedStringException(value);

            return unescapedValue;
        }
        else
        {
            if (!TryUnescapeQuotesSlow(value, '\'', out var unescapedValue))
                throw InvalidSingleQuotedStringException(value);

            return unescapedValue;
        }
    }

    private static bool TryUnescapeQuotesFast(ReadOnlySpan<char> value, char quotes, out string unescapedValue)
    {
        Span<char> buffer = stackalloc char[value.Length - 2];
        return TryUnescapeDoubleQuotesCore(buffer, value, quotes, out unescapedValue);
    }

    private static bool TryUnescapeQuotesSlow(ReadOnlySpan<char> value, char quotes, out string unescapedValue)
    {
        Span<char> buffer = new char[value.Length - 2];
        return TryUnescapeDoubleQuotesCore(buffer, value, quotes, out unescapedValue);
    }

    private static bool TryUnescapeDoubleQuotesCore(Span<char> buffer, ReadOnlySpan<char> value, char quotes, out string unescapedValue)
    {
        var bufferIndex = 0;
        value = value.Slice(1, value.Length - 2);

        while (!value.IsEmpty)
        {
            var next = value.IndexOf(quotes);

            if (next == -1)
                break;

            if (next + 1 >= value.Length || value[next + 1] != quotes)
            {
                unescapedValue = default;
                return false;
            }

            // abc''def -> abc''def
            //    ^ 3          ^ 4
            next++;

            value[..next].CopyTo(buffer[bufferIndex..]);
            bufferIndex += next;
            value = value[(next + 1)..];
        }

        if (!value.IsEmpty)
        {
            value.CopyTo(buffer[bufferIndex..]);
            bufferIndex += value.Length;
        }

        unescapedValue = buffer[..bufferIndex].ToString();
        return true;
    }

    private static ArgumentException InvalidDoubleQuotedStringException(ReadOnlySpan<char> value)
    {
        return new ArgumentException($"Invalid double quoted string {value}", nameof(value));
    }

    private static ArgumentException InvalidSingleQuotedStringException(ReadOnlySpan<char> value)
    {
        return new ArgumentException($"Invalid single quoted string {value}", nameof(value));
    }
    #endregion

    #region Escape
    public static string EscapeDoubleQuotes(string value)
    {
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    public static string EscapeSingleQuotes(string value)
    {
        return $"'{value.Replace("'", "''")}'";
    }
    #endregion
}

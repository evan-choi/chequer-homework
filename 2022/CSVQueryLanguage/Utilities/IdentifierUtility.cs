using System;

namespace CSVQueryLanguage.Utilities;

public static class IdentifierUtility
{
    public static bool IsDoubleQuoted(string value)
    {
        return value is ['"', .., '"'];
    }

    public static bool IsSingleQuoted(string value)
    {
        return value is ['\'', .., '\''];
    }

    #region Unescape
    public static string UnescapeDoubleQuotes(string value)
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

    private static bool TryUnescapeQuotesFast(string value, char quotes, out string unescapedValue)
    {
        Span<char> buffer = stackalloc char[value.Length - 2];
        return TryUnescapeDoubleQuotesCore(buffer, value, quotes, out unescapedValue);
    }

    private static bool TryUnescapeQuotesSlow(string value, char quotes, out string unescapedValue)
    {
        Span<char> buffer = new char[value.Length - 2];
        return TryUnescapeDoubleQuotesCore(buffer, value, quotes, out unescapedValue);
    }

    private static bool TryUnescapeDoubleQuotesCore(Span<char> buffer, string value, char quotes, out string unescapedValue)
    {
        var bufferIndex = 0;
        ReadOnlySpan<char> valueSpan = value.AsSpan(1, value.Length - 2);

        while (!valueSpan.IsEmpty)
        {
            var next = valueSpan.IndexOf(quotes);

            if (next == -1)
                break;

            if (next + 1 >= valueSpan.Length || valueSpan[next + 1] != quotes)
            {
                unescapedValue = default;
                return false;
            }

            // abc''def -> abc''def
            //    ^ 3          ^ 4
            next++;

            valueSpan[..next].CopyTo(buffer[bufferIndex..]);
            bufferIndex += next;
            valueSpan = valueSpan[(next + 1)..];
        }

        if (!valueSpan.IsEmpty)
        {
            valueSpan.CopyTo(buffer[bufferIndex..]);
            bufferIndex += valueSpan.Length;
        }

        unescapedValue = buffer[..bufferIndex].ToString();
        return true;
    }

    private static ArgumentException InvalidDoubleQuotedStringException(string value)
    {
        return new ArgumentException($"Invalid double quoted string {value}", nameof(value));
    }

    private static ArgumentException InvalidSingleQuotedStringException(string value)
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

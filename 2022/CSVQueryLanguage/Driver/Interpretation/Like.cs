using System;
using System.Text.RegularExpressions;

namespace CSVQueryLanguage.Driver.Interpretation;

// %: all charaters
// _: single charater
public sealed class Like
{
    private readonly Regex _pattern;

    public Like(string pattern, bool ignoreCase)
    {
        pattern = Regex.Escape(pattern);
        pattern = pattern.Replace("%", ".*").Replace("_", ".");
        _pattern = new Regex(pattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
    }

    public bool Match(ReadOnlySpan<char> value)
    {
        return _pattern.IsMatch(value);
    }
}

using Antlr4.Runtime.Misc;
using CSVQueryLanguage.Parser;
using CSVQueryLanguage.Parser.Tree;

namespace CSVQueryLanguage.Utilities;

internal static class TreeUtility
{
    public static Identifier GetFileName(CqlBaseParser.FileNameContext context)
    {
        string originalValue;
        string value = null;

        if (context.start == context.stop)
        {
            originalValue = context.start.Text;

            if (context.start.Type == CqlBaseLexer.QUOTED_DOUBLE)
                value = IdentifierUtility.UnescapeDoubleQuotes(originalValue);
        }
        else
        {
            var interval = new Interval(context.start.StartIndex, context.stop.StopIndex);
            originalValue = context.start.InputStream.GetText(interval);
        }

        return new Identifier(value ?? originalValue, originalValue);
    }
}

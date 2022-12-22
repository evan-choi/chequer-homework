using Antlr4.Runtime.Misc;
using CSVQueryLanguage.Utilities;

namespace CSVQueryLanguage.Parser;

internal sealed class CqlPostProcessor : CqlBaseBaseListener
{
    public override void ExitFileName(CqlBaseParser.FileNameContext context)
    {
        string path;

        if (context.start == context.stop)
        {
            path = context.start.Text;

            if (context.start.Type == CqlBaseLexer.QUOTED_DOUBLE)
                path = IdentifierUtility.UnescapeDoubleQuotes(path);
        }
        else
        {
            var interval = new Interval(context.start.StartIndex, context.stop.StopIndex);
            path = context.start.InputStream.GetText(interval);
        }

        if (!PathUtility.IsValidPath(path))
            throw new CqlParsingException($"Invalid csv file path: '{path}'", null, context.start);
    }
}

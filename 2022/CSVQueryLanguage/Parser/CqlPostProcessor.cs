using CSVQueryLanguage.Utilities;

namespace CSVQueryLanguage.Parser;

internal sealed class CqlPostProcessor : CqlBaseBaseListener
{
    public override void ExitFileName(CqlBaseParser.FileNameContext context)
    {
        var path = TreeUtility.GetFileName(context);

        if (!PathUtility.IsValidPath(path.Value))
            throw new CqlParsingException($"Invalid csv file path: {path.OriginalValue}", null, context.start);
    }
}

using Antlr4.Runtime;

namespace CSVQueryLanguage.Parser;

internal readonly struct CqlParsingErrorListener : IAntlrErrorListener<IToken>, IAntlrErrorListener<int>
{
    void IAntlrErrorListener<IToken>.SyntaxError(
        IRecognizer recognizer,
        IToken offendingSymbol,
        int line,
        int charPositionInLine,
        string msg,
        RecognitionException e)
    {
        throw new CqlParsingException(msg, e, line, charPositionInLine);
    }

    void IAntlrErrorListener<int>.SyntaxError(
        IRecognizer recognizer,
        int offendingSymbol,
        int line,
        int charPositionInLine,
        string msg,
        RecognitionException e)
    {
        throw new CqlParsingException(msg, e, line, charPositionInLine);
    }
}

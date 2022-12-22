using System;
using Antlr4.Runtime;

namespace CSVQueryLanguage;

public sealed class CqlParsingException : Exception
{
    public int Line { get; }

    public int Column { get; }

    public CqlParsingException(
        string message,
        Exception innerException,
        int line,
        int column)
        : base($"line {line}:{column}: {message}", innerException)
    {
        Line = line;
        Column = column;
    }

    public CqlParsingException(
        string message,
        Exception innerException,
        IToken token)
        : this(message, innerException, token.Line, token.Column)
    {
    }
}

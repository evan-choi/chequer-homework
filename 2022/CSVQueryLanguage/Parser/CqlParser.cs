using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Parser;

public sealed class CqlParser
{
    public IStatement Parse(string sql)
    {
        var input = new UpperCaseCharStream(new ReadOnlyMemoryCharStream(sql.AsMemory()));
        var lexer = new CqlBaseLexer(input);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new CqlBaseParser(tokenStream);

        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(new CqlParsingErrorListener());

        parser.RemoveErrorListeners();
        parser.AddParseListener(new CqlPostProcessor());
        parser.AddErrorListener(new CqlParsingErrorListener());

        CqlBaseParser.RootContext root;

        try
        {
            parser.Interpreter.PredictionMode = PredictionMode.Sll;
            root = parser.root();
        }
        catch (ParseCanceledException)
        {
            tokenStream.Reset();
            parser.Reset();

            parser.Interpreter.PredictionMode = PredictionMode.Ll;
            root = parser.root();
        }

        var node = new TreeBuilder().VisitRoot(root);

        return (IStatement)node;
    }
}

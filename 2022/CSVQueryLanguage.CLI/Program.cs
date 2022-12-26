using System;
using System.Linq;
using CSVQueryLanguage.Analysis;
using CSVQueryLanguage.Parser;
using CSVQueryLanguage.Utilities;

namespace CSVQueryLanguage.CLI;

internal static class Program
{
    public static void Main(string[] args)
    {
        var parser = new CqlParser();

        while (true)
        {
            try
            {
                Console.Write("CQL> ");
                var sql = Console.ReadLine();
                var statement = parser.Parse(sql);

                Console.WriteLine("==== Tree ====");
                NodePrinter.Print(statement, Console.Out, 2);
                Console.WriteLine();

                Console.WriteLine("==== Analyzer ====");
                var analyzer = new Analyzer();
                var scope = analyzer.AnalyzeStatement(statement);
                Console.WriteLine(string.Join(", ", scope.RelationInfo.Fields.Select(x => x.Name)));
                Console.WriteLine();

                Console.WriteLine("==== Deparse ====");
                Console.WriteLine(CqlDeparser.Deparse(statement));
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

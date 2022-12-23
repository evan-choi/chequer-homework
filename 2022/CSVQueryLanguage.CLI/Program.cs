using System;
using CSVQueryLanguage.Analysis;
using CSVQueryLanguage.Parser;
using CSVQueryLanguage.Utilities;

namespace CSVQueryLanguage.CLI;

internal static class Program
{
    public static void Main(string[] args)
    {
        var parser = new CqlParser();
        // var statement = parser.Parse("SELECT count(*), 1 + 2 as c, faf.name FROM a.csv AS faf where id = 4 limit 2, 3");
        // var deparse = CqlDeparser.Deparse(statement);

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
                // TODO: 
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

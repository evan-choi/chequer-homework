using System;
using System.Linq;
using CSVQueryLanguage.Analysis;
using CSVQueryLanguage.Driver;
using CSVQueryLanguage.Parser;
using CSVQueryLanguage.Plan;
using CSVQueryLanguage.Printers;

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

                Console.WriteLine("==== Deparse ====");
                Console.WriteLine(CqlDeparser.Deparse(statement));
                Console.WriteLine();

                Console.WriteLine("==== Tree ====");
                NodePrinter.Print(statement, Console.Out, 2);
                Console.WriteLine();

                Console.WriteLine("==== Analyzer ====");
                var analyzer = new Analyzer();
                var scope = analyzer.AnalyzeStatement(statement);
                QueryScopePrinter.Print(scope, Console.Out, 2);
                Console.WriteLine();

                Console.WriteLine("==== Plan ====");
                var planner = new Planner(scope.Context);
                var plan = planner.PlanStatement(statement);
                PlanPrinter.Print(plan, Console.Out, 2);
                Console.WriteLine();

                Console.WriteLine("==== Execute ====");
                var executor = new Executor(scope.Context);
                using var cursor = executor.Execute(plan);

                var cols = string.Join(", ", Enumerable.Range(0, cursor.FieldCount).Select(cursor.GetName));
                Console.WriteLine(cols);
                Console.WriteLine(new string('-', cols.Length));

                while (cursor.Read())
                {
                    for (int i = 0; i < cursor.FieldCount; i++)
                    {
                        if (i > 0)
                            Console.Write(", ");

                        Console.Write(cursor.GetValue(i));
                    }

                    Console.WriteLine();
                }

                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

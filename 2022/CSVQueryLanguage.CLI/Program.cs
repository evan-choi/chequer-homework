using System;
using System.Diagnostics;
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

                var sw = Stopwatch.StartNew();
                var statement = parser.Parse(sql);

                // Console.WriteLine("==== Tree ====");
                // NodePrinter.Print(statement, Console.Out, 2);
                // Console.WriteLine();
                //
                // Console.WriteLine("==== Analyzer ====");
                var analyzer = new Analyzer();
                var scope = analyzer.AnalyzeStatement(statement);
                // QueryScopePrinter.Print(scope, Console.Out, 2);
                // Console.WriteLine();
                //
                // Console.WriteLine("==== Plan ====");
                var planner = new Planner(scope.Context);
                var plan = planner.PlanStatement(statement);
                // PlanPrinter.Print(plan, Console.Out, 2);
                // Console.WriteLine();

                // Console.WriteLine("==== Execute ====");
                var executor = new Executor(scope.Context);
                using var cursor = executor.Execute(plan);

                var executeTime = sw.Elapsed;
                sw.Restart();

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

                var fetchTime = sw.Elapsed;

                Console.WriteLine($"execute: {executeTime.TotalMilliseconds:0.##} ms, fetch: {fetchTime.TotalMilliseconds:0.##} ms");
            }
            catch (CqlException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

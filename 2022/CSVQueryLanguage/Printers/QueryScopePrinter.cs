using System.Collections.Generic;
using System.IO;
using CSVQueryLanguage.Analysis;
using CSVQueryLanguage.Parser;

namespace CSVQueryLanguage.Printers;

public static class QueryScopePrinter
{
    public static void Print(QueryScope scope, TextWriter writer, int indentSize = 4)
    {
        var queue = new Queue<QueryScope>();

        while (scope is not null)
        {
            queue.Enqueue(scope);
            scope = scope.Parent;
        }

        var depth = 0;

        while (queue.TryDequeue(out var node))
        {
            PrintRelation();
            depth++;

            TextWriter WriteIndent(int offset = 0)
            {
                // indent
                writer.Write(new string(' ', (offset + depth) * indentSize));
                return writer;
            }

            void PrintRelation()
            {
                WriteIndent().Write(nameof(QueryScope));

                writer.Write(" { ");

                var hasName = !string.IsNullOrWhiteSpace(node.RelationInfo.Name);

                if (hasName)
                    writer.Write($"name: {node.RelationInfo.Name}");

                if (node.Filter is not null)
                {
                    if (hasName)
                        writer.Write(", ");

                    writer.Write($"filter: {CqlDeparser.Deparse(node.Filter)}");
                }

                writer.WriteLine(" } ");

                foreach (var field in node.RelationInfo.Fields)
                {
                    var typeName = field.Type.HasValue ? CqlDeparser.Deparse(field.Type.Value) : "NULL";
                    WriteIndent(1).WriteLine($"{field.Name}({typeName}): {CqlDeparser.Deparse(field.Source)}");
                }
            }
        }
    }
}

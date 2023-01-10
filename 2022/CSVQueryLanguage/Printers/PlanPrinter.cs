using System;
using System.IO;
using System.Linq;
using CSVQueryLanguage.Parser;
using CSVQueryLanguage.Plan.Nodes;
using Properties = System.Collections.Generic.Dictionary<string, string>;

namespace CSVQueryLanguage.Printers;

public static class PlanPrinter
{
    public static void Print(PlanNode plan, TextWriter writer, int indentSize = 4)
    {
        var depth = 0;

        while (plan is not null)
        {
            // indent
            writer.Write(new string(' ', depth++ * indentSize));

            // type
            var typeName = plan.GetType().Name;

            if (typeName.EndsWith("Node"))
                typeName = typeName[..^4];

            writer.Write(typeName);

            // attribute
            Properties properties = GetProperties(plan);

            if (properties is { Count: > 0 })
            {
                writer.Write(" { ");
                writer.Write(string.Join(", ", properties.Select(x => $"{x.Key}: {x.Value}")));
                writer.Write(" } ");
            }

            writer.WriteLine();

            plan = plan.Source;
        }
    }

    private static Properties GetProperties(PlanNode plan)
    {
        switch (plan)
        {
            case CsvScanNode node:
                return GetCsvScanNodeProperties(node);

            case FilterNode node:
                return GetFilterNodeProperties(node);

            case LimitNode node:
                return GetLimitNodeProperties(node);

            case OffsetNode node:
                return GetOffsetNodeProperties(node);

            case ProjectNode node:
                return GetProjectNodeProperties(node);

            case AggregateNode node:
                return GetAggregateNodeProperties(node);

            default:
                throw new ArgumentOutOfRangeException(nameof(plan));
        }
    }

    private static Properties GetCsvScanNodeProperties(CsvScanNode plan)
    {
        return new Properties
        {
            { "columns", $"[{string.Join(", ", plan.ColumnNames)}]" },
            { "file", plan.FileName }
        };
    }

    private static Properties GetFilterNodeProperties(FilterNode plan)
    {
        return new Properties
        {
            { "expression", CqlDeparser.Deparse(plan.Predicate) }
        };
    }

    private static Properties GetLimitNodeProperties(LimitNode plan)
    {
        return new Properties
        {
            { "count", plan.Count.ToString() }
        };
    }

    private static Properties GetOffsetNodeProperties(OffsetNode plan)
    {
        return new Properties
        {
            { "offset", plan.Count.ToString() }
        };
    }

    private static Properties GetProjectNodeProperties(ProjectNode plan)
    {
        return plan.Names
            .Zip(plan.DataTypes, plan.Expressions)
            .ToDictionary(
                x => $"{x.First}({(x.Second.HasValue ? CqlDeparser.Deparse(x.Second.Value) : "NULL")})",
                x => CqlDeparser.Deparse(x.Third)
            );
    }

    private static Properties GetAggregateNodeProperties(AggregateNode node)
    {
        return new Properties
        {
            ["count"] = node.CountVariable.Name
        };
    }
}

using System;
using System.Linq;
using System.Text;
using CSVQueryLanguage.Common;
using CSVQueryLanguage.Common.Functions;
using CSVQueryLanguage.Tree;
using CSVQueryLanguage.Utilities;

namespace CSVQueryLanguage.Parser;

public static class CqlDeparser
{
    public static string Deparse(INode node)
    {
        var builder = new StringBuilder();
        var visitor = new CqlDeparserVisitor(builder);

        node.Accept(visitor);

        return builder.ToString();
    }

    public static string Deparse(LogicalOperator op)
    {
        return op switch
        {
            LogicalOperator.And => "AND",
            LogicalOperator.Or => "OR",
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, $"Unknown operator '{op}'")
        };
    }

    public static string Deparse(ArithmeticOperator op)
    {
        return op switch
        {
            ArithmeticOperator.Add => "+",
            ArithmeticOperator.Subtract => "-",
            ArithmeticOperator.Multiply => "*",
            ArithmeticOperator.Divide => "/",
            ArithmeticOperator.Modulus => "%",
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, $"Unknown operator '{op}'")
        };
    }

    public static string Deparse(ComparisonOperator op)
    {
        return op switch
        {
            ComparisonOperator.Equal => "=",
            ComparisonOperator.NotEqual => "<>",
            ComparisonOperator.LessThan => "<",
            ComparisonOperator.LessThanOrEqual => "<=",
            ComparisonOperator.GreaterThan => ">",
            ComparisonOperator.GreaterThanOrEqual => ">=",
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, $"Unknown operator '{op}'")
        };
    }

    public static string Deparse(ArithmeticSign sign)
    {
        return sign switch
        {
            ArithmeticSign.Plus => "+",
            ArithmeticSign.Minus => "-",
            _ => throw new ArgumentOutOfRangeException(nameof(sign), sign, $"Unknown sign '{sign}'")
        };
    }

    public static string Deparse(DataType type)
    {
        return type switch
        {
            DataType.Text => "TEXT",
            DataType.Number => "NUMBER",
            DataType.Date => "DATE",
            DataType.Time => "TIME",
            DataType.Timestamp => "TIMESTAMP",
            DataType.Boolean => "BOOLEAN",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, $"Unknown type '{type}'")
        };
    }
}

file readonly struct CqlDeparserVisitor : INodeVisitor<object>
{
    private readonly StringBuilder _builder;

    public CqlDeparserVisitor(StringBuilder builder)
    {
        _builder = builder;
    }

    public object Visit(INode node)
    {
        return node.Accept(this);
    }

    public object VisitAliasedRelation(AliasedRelation node)
    {
        _builder.Append("FROM ");

        node.Relation.Accept(this);

        if (node.Alias is not null)
        {
            _builder.Append(" AS ");
            node.Alias.Accept(this);
        }

        return null;
    }

    public object VisitAllColumns(AllColumns node)
    {
        if (node.Prefix is not null)
        {
            node.Prefix.Accept(this);
            _builder.Append('.');
        }

        _builder.Append('*');

        return null;
    }

    public object VisitArithmeticBinaryExpression(ArithmeticBinaryExpression node)
    {
        if (NeedParenthesis(node.Operator, node.Left))
        {
            AcceptWithParenthesis(node.Left);
        }
        else
        {
            node.Left.Accept(this);
        }

        _builder.Append(' ').Append(CqlDeparser.Deparse(node.Operator)).Append(' ');

        if (NeedParenthesis(node.Operator, node.Right))
        {
            AcceptWithParenthesis(node.Right);
        }
        else
        {
            node.Right.Accept(this);
        }

        return null;

        static bool NeedParenthesis(ArithmeticOperator parentOp, INode child)
        {
            if (child is ArithmeticBinaryExpression binary)
            {
                return parentOp >= ArithmeticOperator.Multiply
                       && binary.Operator <= ArithmeticOperator.Subtract;
            }

            return child is LogicalBinaryExpression or ComparisonExpression;
        }
    }

    public object VisitArithmeticUnaryExpression(ArithmeticUnaryExpression node)
    {
        _builder.Append(CqlDeparser.Deparse(node.Sign));
        node.Value.Accept(this);

        return null;
    }

    public object VisitBooleanLiteral(BooleanLiteral node)
    {
        _builder.Append(node.Value ? "TRUE" : "FALSE");

        return null;
    }

    public object VisitColumnReference(ColumnReference node)
    {
        node.Name.Accept(this);

        return null;
    }

    public object VisitComparisonExpression(ComparisonExpression node)
    {
        node.Left.Accept(this);
        _builder.Append(' ').Append(CqlDeparser.Deparse(node.Operator)).Append(' ');
        node.Right.Accept(this);

        return null;
    }

    public object VisitCsvRelation(CsvRelation node)
    {
        node.FileName.Accept(this);

        return null;
    }

    public object VisitDataTypeExpression(DataTypeExpression node)
    {
        _builder.Append(CqlDeparser.Deparse(node.Type));

        return null;
    }

    public object VisitFunction(Function node)
    {
        switch (node.Name)
        {
            case BuiltInFunctions.Concat:
                node.Arguments[0].Accept(this);
                _builder.Append(" || ");
                node.Arguments[1].Accept(this);
                break;

            case BuiltInFunctions.CurrentDate:
            case BuiltInFunctions.CurrentTime:
                _builder.Append(node.Name).Append("()");
                break;

            case BuiltInFunctions.Count:
                _builder.Append(node.Name).Append("(*)");
                break;

            case BuiltInFunctions.Substring:
                _builder.Append(node.Name).Append('(');

                for (int i = 0; i < node.Arguments.Length; i++)
                {
                    if (i > 0)
                        _builder.Append(", ");

                    node.Arguments[i].Accept(this);
                }

                _builder.Append(')');
                break;

            case BuiltInFunctions.Cast:
                _builder.Append(node.Name).Append('(');
                node.Arguments[0].Accept(this);
                _builder.Append(" AS ");
                node.Arguments[1].Accept(this);
                _builder.Append(')');
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(node), node, $"Unknown function '{node.Name}'");
        }

        return null;
    }

    public object VisitFunctionCall(FunctionCall node)
    {
        if (node.Target is CastFunction castFunction)
        {
            _builder.Append("CAST(");
            node.Arguments[0].Accept(this);
            _builder.Append(" AS ");
            _builder.Append(CqlDeparser.Deparse(castFunction.Type));
            _builder.Append(')');
            return null;
        }

        return VisitFunction(new Function(node.Name, node.Arguments));
    }

    public object VisitFieldReference(FieldReference node)
    {
        _builder.Append("__field$").Append(node.Index);
        return null;
    }

    public object VisitIdentifier(Identifier node)
    {
        _builder.Append(node.OriginalValue);
        return null;
    }

    public object VisitLikePredicate(LikePredicate node)
    {
        node.Pattern.Accept(this);
        _builder.Append(node.IsCaseInsensitive ? " ILIKE " : " LIKE ");
        node.Value.Accept(this);

        return null;
    }

    public object VisitLimitClause(Limit node)
    {
        _builder.Append("LIMIT ");

        if (node.Offset.HasValue)
            _builder.Append(node.Offset).Append(", ");

        _builder.Append(node.Count);

        return null;
    }

    public object VisitLogicalBinaryExpression(LogicalBinaryExpression node)
    {
        // TODO: Optimize left, right parenthesize

        if (node.Left is ArithmeticBinaryExpression)
            AcceptWithParenthesis(node.Left);
        else
            node.Left.Accept(this);

        _builder.Append(' ').Append(CqlDeparser.Deparse(node.Operator)).Append(' ');

        if (node.Right is ArithmeticBinaryExpression)
            AcceptWithParenthesis(node.Right);
        else
            node.Right.Accept(this);

        return null;
    }

    public object VisitNotExpression(NotExpression node)
    {
        _builder.Append("NOT ");
        node.Value.Accept(this);

        return null;
    }

    public object VisitNullLiteral(NullLiteral node)
    {
        _builder.Append("NULL");

        return null;
    }

    public object VisitNumberLiteral(NumberLiteral node)
    {
        _builder.Append(node.Value);

        return null;
    }

    public object VisitQualifiedName(QualifiedName node)
    {
        _builder.AppendJoin(".", node.Parts.Select(x => x.OriginalValue));

        return null;
    }

    public object VisitQuery(Query node)
    {
        node.Select.Accept(this);

        if (node.From is not null)
        {
            _builder.Append(' ');
            node.From.Accept(this);
        }

        if (node.Where is not null)
        {
            _builder.Append(" WHERE ");
            node.Where.Accept(this);
        }

        if (node.Limit is not null)
        {
            _builder.Append(' ');
            node.Limit.Accept(this);
        }

        return null;
    }

    public object VisitSelect(Select node)
    {
        _builder.Append("SELECT ");

        for (int i = 0; i < node.Items.Count; i++)
        {
            if (i > 0)
                _builder.Append(", ");

            node.Items[i].Accept(this);
        }

        return null;
    }

    public object VisitSelectStatement(SelectStatement node)
    {
        node.Query.Accept(this);

        return null;
    }

    public object VisitSingleColumn(SingleColumn node)
    {
        node.Expression.Accept(this);

        if (node.Alias is not null)
        {
            _builder.Append(" AS ");
            node.Alias.Accept(this);
        }

        return null;
    }

    public object VisitStringLiteral(TextLiteral node)
    {
        _builder.Append(IdentifierUtility.EscapeSingleQuotes(node.Value));
        return null;
    }

    public object VisitSubqueryRelation(SubqueryRelation node)
    {
        AcceptWithParenthesis(node.Query);

        return null;
    }

    public object VisitVariableReference(VariableReference node)
    {
        _builder.Append("__val$").Append(node.Name);
        return null;
    }

    public object VisitDateLiteral(DateLiteral node)
    {
        throw new NotImplementedException();
    }

    public object VisitTimeLiteral(TimeLiteral node)
    {
        throw new NotImplementedException();
    }

    public object VisitTimestampLiteral(TimestampLiteral node)
    {
        throw new NotImplementedException();
    }

    private void AcceptWithParenthesis(INode node)
    {
        Parenthesize(this, x => node.Accept(x));
    }

    private static void Parenthesize(CqlDeparserVisitor visitor, Func<CqlDeparserVisitor, object> visit)
    {
        visitor._builder.Append('(');
        visit(visitor);
        visitor._builder.Append(')');
    }
}

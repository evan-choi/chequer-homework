using System;
using System.Linq;
using System.Text;
using CSVQueryLanguage.Parser.Tree;
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
}

file readonly struct CqlDeparserVisitor : INodeVisitor<object>
{
    private readonly StringBuilder _builder;

    public CqlDeparserVisitor(StringBuilder builder)
    {
        _builder = builder;
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

        _builder.Append(' ').Append(Deparse(node.Operator)).Append(' ');

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
        _builder.Append(Deparse(node.Sign));
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
        _builder.Append(' ').Append(Deparse(node.Operator)).Append(' ');
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
        _builder.Append(Deparse(node.Type));

        return null;
    }

    public object VisitFunction(Function node)
    {
        switch (node.Name)
        {
            case CqlFunctions.Concat:
                node.Arguments[0].Accept(this);
                _builder.Append(" || ");
                node.Arguments[1].Accept(this);
                break;

            case CqlFunctions.RowNumber:
            case CqlFunctions.CurrentDate:
            case CqlFunctions.CurrentTime:
                _builder.Append(node.Name).Append("()");
                break;

            case CqlFunctions.Count:
                _builder.Append(node.Name).Append("(*)");
                break;

            case CqlFunctions.Substring:
                _builder.Append(node.Name).Append('(');

                for (int i = 0; i < node.Arguments.Length; i++)
                {
                    if (i > 0)
                        _builder.Append(", ");

                    node.Arguments[i].Accept(this);
                }

                _builder.Append(')');
                break;

            case CqlFunctions.Cast:
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

    public object VisitFieldReference(FieldReference node)
    {
        _builder.Append("__ref").Append(node.Index);
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

        _builder.Append(' ').Append(Deparse(node.Operator)).Append(' ');

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

        if (node.AliasedRelation is not null)
        {
            _builder.Append(' ');
            node.AliasedRelation.Accept(this);
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

    public object VisitStringLiteral(StringLiteral node)
    {
        _builder.Append(IdentifierUtility.EscapeSingleQuotes(node.Value));
        return null;
    }

    public object VisitSubqueryRelation(SubqueryRelation node)
    {
        AcceptWithParenthesis(node.Query);

        return null;
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

    private static string Deparse(LogicalOperator op)
    {
        return op switch
        {
            LogicalOperator.And => "AND",
            LogicalOperator.Or => "OR",
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, $"Unknown operator '{op}'")
        };
    }

    private static string Deparse(ArithmeticOperator op)
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

    private static string Deparse(ComparisonOperator op)
    {
        return op switch
        {
            ComparisonOperator.Equal => "=",
            ComparisonOperator.NotEqual => "<>",
            ComparisonOperator.LessThan => "<",
            ComparisonOperator.LessThanOrEqual => "<=",
            ComparisonOperator.GreaterThan => ">=",
            ComparisonOperator.GreaterThanOrEqual => ">=",
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, $"Unknown operator '{op}'")
        };
    }

    private static string Deparse(ArithmeticSign sign)
    {
        return sign switch
        {
            ArithmeticSign.Plus => "+",
            ArithmeticSign.Minus => "-",
            _ => throw new ArgumentOutOfRangeException(nameof(sign), sign, $"Unknown sign '{sign}'")
        };
    }

    private static string Deparse(DataType type)
    {
        return type switch
        {
            DataType.Text => "TEXT",
            DataType.Number => "NUMBER",
            DataType.Date => "DATE",
            DataType.Time => "TIME",
            DataType.Boolean => "BOOLEAN",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, $"Unknown type '{type}'")
        };
    }
}

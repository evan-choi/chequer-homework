using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using CSVQueryLanguage.Parser.Tree;
using CSVQueryLanguage.Utilities;
using static CSVQueryLanguage.Parser.CqlBaseParser;

namespace CSVQueryLanguage.Parser;

internal sealed class TreeBuilder : AbstractParseTreeVisitor<INode>, ICqlBaseVisitor<INode>
{
    public INode VisitColumnReference(ColumnReferenceContext context)
    {
        var name = (QualifiedName)VisitQualifiedName(context.qualifiedName());
        return new ColumnReference(name);
    }

    public INode VisitNullLiteral(NullLiteralContext context)
    {
        return new NullLiteral();
    }

    public INode VisitParenthesizedValueExpression(ParenthesizedValueExpressionContext context)
    {
        return VisitValueExpression(context.valueExpression());
    }

    public INode VisitStringLiteral(StringLiteralContext context)
    {
        var text = context.GetText();
        text = IdentifierUtility.UnescapeSingleQuotes(text);

        return new StringLiteral(text);
    }

    public INode VisitFunctionCall(FunctionCallContext context)
    {
        return VisitFunction(context.function());
    }

    public INode VisitBooleanLiteral(BooleanLiteralContext context)
    {
        var value = context.booleanValue().start.Type == CqlBaseLexer.TRUE;
        return new BooleanLiteral(value);
    }

    public INode VisitNumberLiteral(NumberLiteralContext context)
    {
        var value = long.Parse(context.GetText());
        return new NumberLiteral(value);
    }

    public INode VisitConcatenation(ConcatenationContext context)
    {
        var left = (IExpression)VisitValueExpression(context.l);
        var right = (IExpression)VisitValueExpression(context.r);
        return new Function(CqlFunctions.Concat, left, right);
    }

    public INode VisitArithmeticBinary(ArithmeticBinaryContext context)
    {
        var op = context.op.Type switch
        {
            CqlBaseLexer.ASTERISK => ArithmeticOperator.Multiply,
            CqlBaseLexer.SLASH => ArithmeticOperator.Divide,
            CqlBaseLexer.PERCENT => ArithmeticOperator.Modulus,
            CqlBaseLexer.PLUS => ArithmeticOperator.Add,
            CqlBaseLexer.MINUS => ArithmeticOperator.Subtract,
            _ => throw new ArgumentOutOfRangeException(nameof(context), $"Unknown operator '{context.op.Text}'")
        };

        var left = (IExpression)VisitValueExpression(context.l);
        var right = (IExpression)VisitValueExpression(context.r);

        return new ArithmeticBinaryExpression(op, left, right);
    }

    public INode VisitArithmeticUnary(ArithmeticUnaryContext context)
    {
        var sign = context.op.Type switch
        {
            CqlBaseLexer.PLUS => ArithmeticSign.Plus,
            CqlBaseLexer.MINUS => ArithmeticSign.Minus,
            _ => throw new ArgumentOutOfRangeException(nameof(context), $"Unknown sign '{context.op.Text}'")
        };

        var value = (IExpression)VisitValueExpression(context.valueExpression());

        return new ArithmeticUnaryExpression(sign, value);
    }

    public INode VisitPrimary(PrimaryContext context)
    {
        return VisitPrimaryExpression(context.primaryExpression());
    }

    public INode VisitSelectAll(SelectAllContext context)
    {
        Identifier prefix = null;

        if (context.identifier() is { } identifier)
            prefix = (Identifier)VisitIdentifier(identifier);

        return new AllColumns(prefix);
    }

    public INode VisitSelectSingle(SelectSingleContext context)
    {
        var expression = (IExpression)VisitExpression(context.expression());
        Identifier alias = null;

        if (context.identifier() is { } identifier)
            alias = (Identifier)VisitIdentifier(identifier);

        return new SingleColumn(expression, alias);
    }

    public INode VisitSubquery(SubqueryContext context)
    {
        var query = (Query)VisitQuery(context.query());
        return new SubqueryRelation(query);
    }

    public INode VisitParenthesizedRelation(ParenthesizedRelationContext context)
    {
        return VisitRelationPrimary(context.relationPrimary());
    }

    public INode VisitCsv(CsvContext context)
    {
        var fileName = TreeUtility.GetFileName(context.fileName());
        return new CsvRelation(fileName);
    }

    public INode VisitLogicalNot(LogicalNotContext context)
    {
        var value = (IExpression)VisitBooleanExpression(context.booleanExpression());
        return new NotExpression(value);
    }

    public INode VisitParenthesizedBooleanExpression(ParenthesizedBooleanExpressionContext context)
    {
        return VisitBooleanExpression(context.booleanExpression());
    }

    public INode VisitPredicateLike(PredicateLikeContext context)
    {
        var isCaseInsensitive = context.op.Type == CqlBaseLexer.ILIKE;
        var value = (IExpression)VisitValueExpression(context.l);
        var pattern = (IExpression)VisitValueExpression(context.r);

        return new LikePredicate(value, pattern, isCaseInsensitive);
    }

    public INode VisitPredicateComparison(PredicateComparisonContext context)
    {
        var op = context.op.Type switch
        {
            CqlBaseLexer.EQ => ComparisonOperator.Equal,
            CqlBaseLexer.NEQ => ComparisonOperator.NotEqual,
            CqlBaseLexer.LT => ComparisonOperator.LessThan,
            CqlBaseLexer.LTE => ComparisonOperator.LessThanOrEqual,
            CqlBaseLexer.GT => ComparisonOperator.GreaterThan,
            CqlBaseLexer.GTE => ComparisonOperator.GreaterThanOrEqual,
            _ => throw new ArgumentOutOfRangeException(nameof(context), $"Unknown operator '{context.op.Text}'")
        };

        var left = (IExpression)VisitValueExpression(context.l);
        var right = (IExpression)VisitValueExpression(context.r);

        return new ComparisonExpression(op, left, right);
    }

    public INode VisitLogicalBinary(LogicalBinaryContext context)
    {
        var op = context.op.Type switch
        {
            CqlBaseLexer.AND => LogicalOperator.And,
            CqlBaseLexer.OR => LogicalOperator.Or,
            _ => throw new ArgumentOutOfRangeException(nameof(context), $"Unknown operator '{context.op.Text}'")
        };

        var left = (IExpression)VisitBooleanExpression(context.l);
        var right = (IExpression)VisitBooleanExpression(context.r);

        return new LogicalBinaryExpression(op, left, right);
    }

    public INode VisitCurrentTime(CurrentTimeContext context)
    {
        return new Function(CqlFunctions.CurrentTime);
    }

    public INode VisitCast(CastContext context)
    {
        var value = (IExpression)VisitExpression(context.expression());
        var type = (IExpression)VisitType(context.type());
        return new Function(CqlFunctions.Cast, value, type);
    }

    public INode VisitCount(CountContext context)
    {
        return new Function(CqlFunctions.Count);
    }

    public INode VisitCurrentDate(CurrentDateContext context)
    {
        return new Function(CqlFunctions.CurrentDate);
    }

    public INode VisitRowNumber(RowNumberContext context)
    {
        return new Function(CqlFunctions.RowNumber);
    }

    public INode VisitSubstring(SubstringContext context)
    {
        IExpression[] arguments = context.valueExpression()
            .Select(VisitValueExpression)
            .Cast<IExpression>()
            .ToArray();

        return new Function(CqlFunctions.Substring, arguments);
    }

    public INode VisitSelectStatement(SelectStatementContext context)
    {
        var query = (Query)VisitQuery(context.query());
        return new SelectStatement(query);
    }

    public INode VisitDefault(DefaultContext context)
    {
        return VisitQuerySpecification(context.querySpecification());
    }

    public INode VisitParenthesizedQuery(ParenthesizedQueryContext context)
    {
        return VisitQuery(context.query());
    }

    public INode VisitRoot(RootContext context)
    {
        return VisitStatement(context.statement());
    }

    public INode VisitStatement(StatementContext context)
    {
        return context switch
        {
            SelectStatementContext ctx => VisitSelectStatement(ctx),
            _ => throw new NotSupportedException(context.GetType().Name)
        };
    }

    public INode VisitQuery(QueryContext context)
    {
        var query = (Query)VisitQueryTerm(context.term);

        if (context.limit is null)
            return query;

        var limit = (Limit)VisitLimitClause(context.limit);

        return new Query(query.Select, query.AliasedRelation, query.Where, limit);
    }

    public INode VisitQueryTerm(QueryTermContext context)
    {
        return context switch
        {
            DefaultContext ctx => VisitDefault(ctx),
            ParenthesizedQueryContext ctx => VisitParenthesizedQuery(ctx),
            _ => throw new NotSupportedException(context.GetType().Name)
        };
    }

    public INode VisitQuerySpecification(QuerySpecificationContext context)
    {
        IEnumerable<ISelectItem> items = context._items
            .Select(VisitSelectItem)
            .Cast<ISelectItem>();

        var select = new Select(items);
        AliasedRelation aliasedRelation = null;
        IExpression where = null;

        if (context.from is not null)
            aliasedRelation = (AliasedRelation)VisitAliasedRelation(context.from);

        if (context.where is not null)
            where = (IExpression)VisitBooleanExpression(context.where);

        return new Query(select, aliasedRelation, where, null);
    }

    public INode VisitSelectItem(SelectItemContext context)
    {
        return context switch
        {
            SelectSingleContext ctx => VisitSelectSingle(ctx),
            SelectAllContext ctx => VisitSelectAll(ctx),
            _ => throw new NotSupportedException(context.GetType().Name)
        };
    }

    public INode VisitAliasedRelation(AliasedRelationContext context)
    {
        var relation = (IRelation)VisitRelationPrimary(context.relationPrimary());
        Identifier alias = null;

        if (context.alias is not null)
            alias = (Identifier)VisitIdentifier(context.alias);

        return new AliasedRelation(relation, alias);
    }

    public INode VisitRelationPrimary(RelationPrimaryContext context)
    {
        return context switch
        {
            CsvContext ctx => VisitCsv(ctx),
            SubqueryContext ctx => VisitSubquery(ctx),
            ParenthesizedRelationContext ctx => VisitParenthesizedRelation(ctx),
            _ => throw new NotSupportedException(context.GetType().Name)
        };
    }

    public INode VisitLimitClause(LimitClauseContext context)
    {
        long? offset = null;

        if (context.offset is not null)
            offset = long.Parse(context.offset.Text);

        var count = long.Parse(context.count.Text);

        return new Limit(offset, count);
    }

    public INode VisitFileName(FileNameContext context)
    {
        throw new NotSupportedException();
    }

    public INode VisitIdentifier(IdentifierContext context)
    {
        var originalValue = context.GetText();
        var value = originalValue;

        if (context.start.Type == CqlBaseLexer.QUOTED_DOUBLE)
            value = IdentifierUtility.UnescapeDoubleQuotes(originalValue);

        return new Identifier(value, originalValue);
    }

    public INode VisitQualifiedName(QualifiedNameContext context)
    {
        Identifier[] identifiers = context.identifier()
            .Select(VisitIdentifier)
            .Cast<Identifier>()
            .ToArray();

        return new QualifiedName(identifiers);
    }

    public INode VisitExpression(ExpressionContext context)
    {
        if (context.children[0] is ValueExpressionContext valueExpressionContext)
            return VisitValueExpression(valueExpressionContext);

        return VisitBooleanExpression((BooleanExpressionContext)context.children[0]);
    }

    public INode VisitValueExpression(ValueExpressionContext context)
    {
        return context switch
        {
            PrimaryContext ctx => VisitPrimary(ctx),
            ArithmeticUnaryContext ctx => VisitArithmeticUnary(ctx),
            ArithmeticBinaryContext ctx => VisitArithmeticBinary(ctx),
            ConcatenationContext ctx => VisitConcatenation(ctx),
            ParenthesizedValueExpressionContext ctx => VisitValueExpression(ctx.valueExpression()),
            _ => throw new NotSupportedException(context.GetType().Name)
        };
    }

    public INode VisitPrimaryExpression(PrimaryExpressionContext context)
    {
        return context switch
        {
            NullLiteralContext ctx => VisitNullLiteral(ctx),
            NumberLiteralContext ctx => VisitNumberLiteral(ctx),
            StringLiteralContext ctx => VisitStringLiteral(ctx),
            BooleanLiteralContext ctx => VisitBooleanLiteral(ctx),
            ColumnReferenceContext ctx => VisitColumnReference(ctx),
            FunctionCallContext ctx => VisitFunctionCall(ctx),
            _ => throw new NotSupportedException(context.GetType().Name)
        };
    }

    public INode VisitBooleanExpression(BooleanExpressionContext context)
    {
        return context switch
        {
            PredicateLikeContext ctx => VisitPredicateLike(ctx),
            PredicateComparisonContext ctx => VisitPredicateComparison(ctx),
            LogicalNotContext ctx => VisitLogicalNot(ctx),
            LogicalBinaryContext ctx => VisitLogicalBinary(ctx),
            ParenthesizedBooleanExpressionContext ctx => VisitParenthesizedBooleanExpression(ctx),
            _ => throw new NotSupportedException(context.GetType().Name)
        };
    }

    public INode VisitBooleanValue(BooleanValueContext context)
    {
        throw new NotSupportedException();
    }

    public INode VisitFunction(FunctionContext context)
    {
        return context switch
        {
            RowNumberContext ctx => VisitRowNumber(ctx),
            CountContext ctx => VisitCount(ctx),
            CurrentDateContext ctx => VisitCurrentDate(ctx),
            CurrentTimeContext ctx => VisitCurrentTime(ctx),
            SubstringContext ctx => VisitSubstring(ctx),
            CastContext ctx => VisitCast(ctx),
            _ => throw new NotSupportedException(context.GetType().Name)
        };
    }

    public INode VisitType(TypeContext context)
    {
        var type = context.start.Type switch
        {
            CqlBaseLexer.TEXT => DataType.Text,
            CqlBaseLexer.NUMBER => DataType.Number,
            CqlBaseLexer.DATE => DataType.Date,
            CqlBaseLexer.TIME => DataType.Time,
            CqlBaseLexer.BOOLEAN => DataType.Boolean,
            _ => throw new ArgumentOutOfRangeException(nameof(context), $"Unknown type '{context.GetText()}'")
        };

        return new DataTypeExpression(type);
    }
}

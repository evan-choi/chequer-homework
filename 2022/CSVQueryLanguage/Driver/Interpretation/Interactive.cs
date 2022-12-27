using System;
using CSVQueryLanguage.Driver.Cursors;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Driver.Interpretation;

public sealed class Interactive
{
    private readonly ExecutionContext _context;

    public Interactive(ExecutionContext context)
    {
        _context = context;
    }

    public T Interpret<T>(IExpression expression, ICursorRecord record)
    {
        return (T)expression.Accept(new Interpreter(_context, record));
    }
}

file readonly struct Interpreter : IExpressionVisitor<object>
{
    private readonly ExecutionContext _context;
    private readonly ICursorRecord _record;

    public Interpreter(ExecutionContext context, ICursorRecord record)
    {
        _context = context;
        _record = record;
    }

    public object Visit(INode node)
    {
        return node.Accept(this);
    }

    public object VisitArithmeticBinaryExpression(ArithmeticBinaryExpression node)
    {
        var left = (double)node.Left.Accept(this);
        var right = (double)node.Right.Accept(this);

        return AtomicInteractive.Interpret(node.Operator, left, right);
    }

    public object VisitArithmeticUnaryExpression(ArithmeticUnaryExpression node)
    {
        var value = (double)node.Value.Accept(this);

        return AtomicInteractive.Interpret(node.Sign, value);
    }

    public object VisitBooleanLiteral(BooleanLiteral node)
    {
        return node.Value;
    }

    public object VisitComparisonExpression(ComparisonExpression node)
    {
        var left = node.Left.Accept(this);
        var right = node.Right.Accept(this);

        return AtomicInteractive.Interpret(node.Operator, left, right);
    }

    public object VisitFunction(Function node)
    {
        throw new NotImplementedException();
    }

    public object VisitFieldReference(FieldReference node)
    {
        return _record.GetValue(node.Index);
    }

    // TODO: Optimize Like object
    public object VisitLikePredicate(LikePredicate node)
    {
        var value = (string)node.Value.Accept(this);
        var pattern = (string)node.Pattern.Accept(this);
        var like = new Like(pattern, node.IsCaseInsensitive);

        return like.Match(value);
    }

    public object VisitLogicalBinaryExpression(LogicalBinaryExpression node)
    {
        var left = node.Left.Accept(this);
        var right = node.Right.Accept(this);

        return AtomicInteractive.InterpretBoxed(node.Operator, left, right);
    }

    public object VisitNotExpression(NotExpression node)
    {
        var value = (bool)node.Value.Accept(this);
        return !value;
    }

    public object VisitNullLiteral(NullLiteral node)
    {
        return node.Value;
    }

    public object VisitNumberLiteral(NumberLiteral node)
    {
        return node.Value;
    }

    public object VisitStringLiteral(TextLiteral node)
    {
        return node.Value;
    }

    public object VisitVariableReference(VariableReference node)
    {
        throw new NotImplementedException();
    }

    public object VisitDateLiteral(DateLiteral node)
    {
        return node.Value;
    }

    public object VisitTimeLiteral(TimeLiteral node)
    {
        return node.Value;
    }

    public object VisitTimestampLiteral(TimestampLiteral node)
    {
        return node.Value;
    }

    public object VisitColumnReference(ColumnReference node)
    {
        throw new NotSupportedException();
    }

    public object VisitDataTypeExpression(DataTypeExpression node)
    {
        throw new NotSupportedException();
    }
}

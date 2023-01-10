using System;
using System.Linq;
using CSVQueryLanguage.Common;
using CSVQueryLanguage.Common.Functions;
using CSVQueryLanguage.Driver.Interpretation;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Analysis;

public sealed class ExpressionAnalyzer
{
    private readonly AnalyzerContext _context;

    public ExpressionAnalyzer(AnalyzerContext context)
    {
        _context = context;
    }

    public IExpression Analyze(IExpression expression, QueryScope scope)
    {
        return expression.Accept(new ExpressionRewriter(_context, scope));
    }
}

file sealed class ExpressionRewriter : IExpressionVisitor<IExpression>
{
    private readonly AnalyzerContext _context;
    private readonly QueryScope _scope;

    public ExpressionRewriter(AnalyzerContext context, QueryScope scope)
    {
        _context = context;
        _scope = scope;
    }

    public IExpression Visit(INode node)
    {
        return node.Accept(this);
    }

    public IExpression VisitArithmeticBinaryExpression(ArithmeticBinaryExpression node)
    {
        var left = node.Left.Accept(this);
        var right = node.Right.Accept(this);

        DataType? leftType = _context.ExpressionTypes[left];
        DataType? rightType = _context.ExpressionTypes[right];

        if (!AtomicInteractive.CanInterpret(node.Operator, leftType, rightType))
            throw CqlErrors.NotSupportedBinaryOperator(node.Operator, leftType, rightType);

        // pre-interpret
        switch (left, right)
        {
            case (NumberLiteral leftNumber, NumberLiteral rightNumber):
            {
                var result = AtomicInteractive.Interpret(node.Operator, leftNumber.Value, rightNumber.Value);
                return new NumberLiteral(result);
            }

            case (TimeLiteral leftTime, TimeLiteral rightTime):
            {
                var result = AtomicInteractive.Interpret(node.Operator, leftTime.Value, rightTime.Value);
                return new TimeLiteral(result);
            }

            case (TimestampLiteral leftTimestamp, TimeLiteral rightTime):
            {
                var result = AtomicInteractive.Interpret(node.Operator, leftTimestamp.Value, rightTime.Value);
                return new TimestampLiteral(result);
            }
        }

        // rewrite
        if (left != node.Left || right != node.Right)
            node = new ArithmeticBinaryExpression(node.Operator, left, right);

        _context.ExpressionTypes[node] = leftType;

        return node;
    }

    public IExpression VisitArithmeticUnaryExpression(ArithmeticUnaryExpression node)
    {
        var value = node.Value.Accept(this);
        DataType? valueType = _context.ExpressionTypes[value];

        if (!AtomicInteractive.CanInterpret(node.Sign, valueType))
            throw CqlErrors.NotSupportedUnaryOperator(node.Sign, valueType);

        // pre-interpret
        if (value is NumberLiteral number)
        {
            var result = AtomicInteractive.Interpret(node.Sign, number.Value);
            return new NumberLiteral(result);
        }

        // pre-interpret
        if (node.Sign is ArithmeticSign.Plus)
            return value;

        _context.ExpressionTypes[node] = valueType;

        return node;
    }

    public IExpression VisitBooleanLiteral(BooleanLiteral node)
    {
        _context.ExpressionTypes[node] = DataType.Boolean;
        return node;
    }

    public IExpression VisitColumnReference(ColumnReference node)
    {
        var field = _scope.ResolveField(node.Name);
        var fieldIndex = _scope.ResolveFieldIndex(field);

        var fieldReference = new FieldReference(fieldIndex);
        _context.ExpressionTypes[fieldReference] = field.Type;

        return fieldReference;
    }

    public IExpression VisitComparisonExpression(ComparisonExpression node)
    {
        var left = node.Left.Accept(this);
        var right = node.Right.Accept(this);

        // pre-interpret
        if (left is ILiteral leftLiteral && right is ILiteral rightLiteral)
        {
            var result = AtomicInteractive.Interpret(node.Operator, leftLiteral.Value, rightLiteral.Value);
            return result ? BooleanLiteral.True : BooleanLiteral.False;
        }

        // rewrite
        if (left != node.Left || right != node.Right)
            node = new ComparisonExpression(node.Operator, left, right);

        _context.ExpressionTypes[node] = DataType.Boolean;

        return node;
    }

    public IExpression VisitDataTypeExpression(DataTypeExpression node)
    {
        // Special node for cast function
        return node;
    }

    public IExpression VisitFunction(Function node)
    {
        var arguments = new IExpression[node.Arguments.Length];

        for (int i = 0; i < arguments.Length; i++)
            arguments[i] = node.Arguments[i].Accept(this);

        DataType? type;
        IFunction function;

        switch (node.Name)
        {
            case BuiltInFunctions.Count:
                var variableInfo = _scope.CountVariable;

                if (variableInfo is null)
                {
                    variableInfo = _context.DelcareAnonymousVariable(name => new VariableInfo(node, name, DataType.Number));
                    _scope.CountVariable = variableInfo;
                }

                var variableNode = new VariableReference(variableInfo.Name);
                _context.ExpressionTypes[variableNode] = DataType.Number;

                return variableNode;

            case BuiltInFunctions.Concat:
                type = DataType.Text;
                function = new ConcatFunction();
                break;

            case BuiltInFunctions.CurrentDate:
                type = DataType.Date;
                function = new CurrentDateFunction();
                break;

            case BuiltInFunctions.CurrentTime:
                type = DataType.Time;
                function = new CurrentTimeFunction();
                break;

            case BuiltInFunctions.Substring:
                type = DataType.Text;
                function = new SubstringFunction();
                break;

            case BuiltInFunctions.Cast:
                var typeNode = (DataTypeExpression)arguments[1];
                arguments = new[] { arguments[0] };
                type = typeNode.Type;
                function = new CastFunction(typeNode.Type);
                break;

            default:
                throw CqlErrors.UnknownFunction(node.Name);
        }

        var callNode = new FunctionCall(node.Name, function, arguments);
        _context.ExpressionTypes[callNode] = type;

        return callNode;
    }

    public IExpression VisitFunctionCall(FunctionCall node)
    {
        return node;
    }

    public IExpression VisitFieldReference(FieldReference node)
    {
        var field = _scope.RelationInfo.Fields[node.Index];
        _context.ExpressionTypes[node] = field.Type;

        return node;
    }

    public IExpression VisitLikePredicate(LikePredicate node)
    {
        var value = node.Value.Accept(this);
        var pattern = node.Pattern.Accept(this);

        DataType? valueType = _context.ExpressionTypes[value];
        DataType? patternType = _context.ExpressionTypes[pattern];

        // <string | null> LIKE <string | null>
        if (valueType is not (DataType.Text or null))
            throw CqlErrors.InvalidArgumentBindingOfFunction("LIKE", "<value>", valueType);

        if (patternType is not (DataType.Text or null))
            throw CqlErrors.InvalidArgumentBindingOfFunction("LIKE", "<pattern>", patternType);

        // pre-interpret
        if (!valueType.HasValue && !patternType.HasValue)
            return BooleanLiteral.True;

        if (!valueType.HasValue || !patternType.HasValue)
            return BooleanLiteral.False;

        if (value is TextLiteral valueLiteral && pattern is TextLiteral patternLiteral)
        {
            var like = new Like(patternLiteral.Value, node.IsCaseInsensitive);
            var result = like.Match(valueLiteral.Value);
            return result ? BooleanLiteral.True : BooleanLiteral.False;
        }

        // rewrite
        if (value != node.Value || pattern != node.Pattern)
            node = new LikePredicate(value, pattern, node.IsCaseInsensitive);

        _context.ExpressionTypes[node] = DataType.Boolean;

        return node;
    }

    public IExpression VisitLogicalBinaryExpression(LogicalBinaryExpression node)
    {
        var left = node.Left.Accept(this);
        var right = node.Right.Accept(this);

        DataType? leftType = _context.ExpressionTypes[left];
        DataType? rightType = _context.ExpressionTypes[right];

        if (!AtomicInteractive.CanInterpret(node.Operator, leftType, rightType))
            throw CqlErrors.NotSupportedLogicalOperator(node.Operator, leftType, rightType);

        // pre-interpret
        if (left is ILiteral leftLiteral && right is ILiteral rightLiteral)
        {
            var result = AtomicInteractive.InterpretBoxed(node.Operator, leftLiteral.Value, rightLiteral.Value);
            return result ? BooleanLiteral.True : BooleanLiteral.False;
        }

        // rewrite
        if (left != node.Left || right != node.Right)
            node = new LogicalBinaryExpression(node.Operator, left, right);

        _context.ExpressionTypes[node] = DataType.Boolean;

        return node;
    }

    public IExpression VisitNotExpression(NotExpression node)
    {
        var value = node.Value.Accept(this);
        DataType? type = _context.ExpressionTypes[value];

        if (type is null)
            return NullLiteral.Default;

        if (type is not DataType.Boolean)
            throw CqlErrors.NotSupportedNotOperator(type);

        if (value is BooleanLiteral boolLiteral)
            return boolLiteral.Value ? BooleanLiteral.False : BooleanLiteral.True;

        // rewrite
        if (value != node.Value)
            node = new NotExpression(value);

        _context.ExpressionTypes[node] = DataType.Boolean;

        return node;
    }

    public IExpression VisitNullLiteral(NullLiteral node)
    {
        return node;
    }

    public IExpression VisitNumberLiteral(NumberLiteral node)
    {
        return node;
    }

    public IExpression VisitStringLiteral(TextLiteral node)
    {
        return node;
    }

    public IExpression VisitVariableReference(VariableReference node)
    {
        return node;
    }

    public IExpression VisitDateLiteral(DateLiteral node)
    {
        return node;
    }

    public IExpression VisitTimeLiteral(TimeLiteral node)
    {
        return node;
    }

    public IExpression VisitTimestampLiteral(TimestampLiteral node)
    {
        return node;
    }
}

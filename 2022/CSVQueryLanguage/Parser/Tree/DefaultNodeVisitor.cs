namespace CSVQueryLanguage.Parser.Tree;

public abstract class DefaultNodeVisitor<TResult> : INodeVisitor<TResult>
{
    protected int CurrentDepth { get; private set; }

    public virtual TResult VisitAliasedRelation(AliasedRelation node)
    {
        CurrentDepth++;

        node.Relation.Accept(this);
        node.Alias?.Accept(this);

        CurrentDepth--;
        return default;
    }

    public virtual TResult VisitAllColumns(AllColumns node)
    {
        CurrentDepth++;

        node.Prefix?.Accept(this);

        CurrentDepth--;
        return default;
    }

    public virtual TResult VisitArithmeticBinaryExpression(ArithmeticBinaryExpression node)
    {
        CurrentDepth++;

        node.Left.Accept(this);
        node.Right.Accept(this);

        CurrentDepth--;
        return default;
    }

    public virtual TResult VisitArithmeticUnaryExpression(ArithmeticUnaryExpression node)
    {
        CurrentDepth++;

        node.Value.Accept(this);

        CurrentDepth--;
        return default;
    }

    public virtual TResult VisitBooleanLiteral(BooleanLiteral node)
    {
        return default;
    }

    public virtual TResult VisitColumnReference(ColumnReference node)
    {
        CurrentDepth++;

        node.Name.Accept(this);

        CurrentDepth--;
        return default;
    }

    public virtual TResult VisitComparisonExpression(ComparisonExpression node)
    {
        CurrentDepth++;

        node.Left.Accept(this);
        node.Right.Accept(this);

        CurrentDepth--;
        return default;
    }

    public virtual TResult VisitCsvRelation(CsvRelation node)
    {
        CurrentDepth++;

        node.FileName.Accept(this);

        CurrentDepth--;
        return default;
    }

    public virtual TResult VisitDataTypeExpression(DataTypeExpression node)
    {
        return default;
    }

    public virtual TResult VisitFunction(Function node)
    {
        CurrentDepth++;

        foreach (var argument in node.Arguments)
            argument.Accept(this);

        CurrentDepth--;
        return default;
    }

    public virtual TResult VisitFieldReference(FieldReference node)
    {
        return default;
    }

    public virtual TResult VisitIdentifier(Identifier node)
    {
        return default;
    }

    public virtual TResult VisitLikePredicate(LikePredicate node)
    {
        CurrentDepth++;

        node.Value.Accept(this);
        node.Pattern.Accept(this);

        CurrentDepth--;
        return default;
    }

    public virtual TResult VisitLimitClause(Limit node)
    {
        return default;
    }

    public virtual TResult VisitLogicalBinaryExpression(LogicalBinaryExpression node)
    {
        CurrentDepth++;

        node.Left.Accept(this);
        node.Right.Accept(this);

        CurrentDepth--;
        return default;
    }

    public virtual TResult VisitNotExpression(NotExpression node)
    {
        CurrentDepth++;

        node.Value.Accept(this);

        CurrentDepth--;
        return default;
    }

    public virtual TResult VisitNullLiteral(NullLiteral node)
    {
        return default;
    }

    public virtual TResult VisitNumberLiteral(NumberLiteral node)
    {
        return default;
    }

    public virtual TResult VisitQualifiedName(QualifiedName node)
    {
        CurrentDepth++;

        foreach (var part in node.Parts)
            part.Accept(this);

        CurrentDepth--;
        return default;
    }

    public virtual TResult VisitQuery(Query node)
    {
        CurrentDepth++;

        node.Select.Accept(this);
        node.AliasedRelation?.Accept(this);
        node.Where?.Accept(this);
        node.Limit?.Accept(this);

        CurrentDepth--;
        return default;
    }

    public virtual TResult VisitSelect(Select node)
    {
        CurrentDepth++;

        foreach (var selectItem in node.Items)
            selectItem.Accept(this);

        CurrentDepth--;
        return default;
    }

    public virtual TResult VisitSelectStatement(SelectStatement node)
    {
        CurrentDepth++;
        node.Query.Accept(this);
        CurrentDepth--;
        return default;
    }

    public virtual TResult VisitSingleColumn(SingleColumn node)
    {
        CurrentDepth++;
        node.Expression.Accept(this);
        node.Alias?.Accept(this);
        CurrentDepth--;
        return default;
    }

    public virtual TResult VisitStringLiteral(StringLiteral node)
    {
        return default;
    }

    public virtual TResult VisitSubqueryRelation(SubqueryRelation node)
    {
        CurrentDepth++;
        node.Query.Accept(this);
        CurrentDepth--;
        return default;
    }
}

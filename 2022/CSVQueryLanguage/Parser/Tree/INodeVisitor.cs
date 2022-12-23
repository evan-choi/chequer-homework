namespace CSVQueryLanguage.Parser.Tree;

public interface INodeVisitor<out TResult>
{
    TResult Visit(INode node) => node.Accept(this);

    TResult VisitAliasedRelation(AliasedRelation node);

    TResult VisitAllColumns(AllColumns node);

    TResult VisitArithmeticBinaryExpression(ArithmeticBinaryExpression node);

    TResult VisitArithmeticUnaryExpression(ArithmeticUnaryExpression node);

    TResult VisitBooleanLiteral(BooleanLiteral node);

    TResult VisitColumnReference(ColumnReference node);

    TResult VisitComparisonExpression(ComparisonExpression node);

    TResult VisitCsvRelation(CsvRelation node);

    TResult VisitDataTypeExpression(DataTypeExpression node);

    TResult VisitFunction(Function node);

    TResult VisitFieldReference(FieldReference node);

    TResult VisitIdentifier(Identifier node);

    TResult VisitLikePredicate(LikePredicate node);

    TResult VisitLimitClause(Limit node);

    TResult VisitLogicalBinaryExpression(LogicalBinaryExpression node);

    TResult VisitNotExpression(NotExpression node);

    TResult VisitNullLiteral(NullLiteral node);

    TResult VisitNumberLiteral(NumberLiteral node);

    TResult VisitQualifiedName(QualifiedName node);

    TResult VisitQuery(Query node);

    TResult VisitSelect(Select node);

    TResult VisitSelectStatement(SelectStatement node);

    TResult VisitSingleColumn(SingleColumn node);

    TResult VisitStringLiteral(StringLiteral node);

    TResult VisitSubqueryRelation(SubqueryRelation node);
}

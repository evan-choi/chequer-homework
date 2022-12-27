using System;

namespace CSVQueryLanguage.Tree;

public interface IExpressionVisitor<out TResult> : INodeVisitor<TResult>
{
    TResult INodeVisitor<TResult>.VisitAliasedRelation(AliasedRelation node) => throw new NotSupportedException();

    TResult INodeVisitor<TResult>.VisitAllColumns(AllColumns node) => throw new NotSupportedException();

    TResult INodeVisitor<TResult>.VisitCsvRelation(CsvRelation node) => throw new NotSupportedException();

    TResult INodeVisitor<TResult>.VisitIdentifier(Identifier node) => throw new NotSupportedException();

    TResult INodeVisitor<TResult>.VisitLimitClause(Limit node) => throw new NotSupportedException();

    TResult INodeVisitor<TResult>.VisitQualifiedName(QualifiedName node) => throw new NotSupportedException();

    TResult INodeVisitor<TResult>.VisitQuery(Query node) => throw new NotSupportedException();

    TResult INodeVisitor<TResult>.VisitSelect(Select node) => throw new NotSupportedException();

    TResult INodeVisitor<TResult>.VisitSelectStatement(SelectStatement node) => throw new NotSupportedException();

    TResult INodeVisitor<TResult>.VisitSingleColumn(SingleColumn node) => throw new NotSupportedException();

    TResult INodeVisitor<TResult>.VisitSubqueryRelation(SubqueryRelation node) => throw new NotSupportedException();
}

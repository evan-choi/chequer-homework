using System.IO;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Printers;

public static class NodePrinter
{
    public static void Print(INode node, TextWriter writer, int indentSize = 4)
    {
        var visitor = new NodePrinterVisitor(writer, indentSize);
        node.Accept(visitor);
    }
}

file sealed class NodePrinterVisitor : DefaultNodeVisitor<object>
{
    private readonly TextWriter _writer;
    private readonly int _indentSize;

    public NodePrinterVisitor(TextWriter writer, int indentSize)
    {
        _writer = writer;
        _indentSize = indentSize;
    }

    private void WriteIndent(int offset = 0)
    {
        _writer.Write(new string(' ', (CurrentDepth + offset) * _indentSize));
    }

    public override object VisitAliasedRelation(AliasedRelation node)
    {
        WriteIndent();
        _writer.WriteLine(nameof(AliasedRelation));
        return base.VisitAliasedRelation(node);
    }

    public override object VisitAllColumns(AllColumns node)
    {
        WriteIndent();
        _writer.WriteLine(nameof(AllColumns));
        return base.VisitAllColumns(node);
    }

    public override object VisitArithmeticBinaryExpression(ArithmeticBinaryExpression node)
    {
        WriteIndent();
        _writer.WriteLine($"{nameof(ArithmeticBinaryExpression)} {{ op: {node.Operator} }} ");
        return base.VisitArithmeticBinaryExpression(node);
    }

    public override object VisitArithmeticUnaryExpression(ArithmeticUnaryExpression node)
    {
        WriteIndent();
        _writer.WriteLine($"{nameof(ArithmeticUnaryExpression)} {{ sign: {node.Sign} }} ");
        return base.VisitArithmeticUnaryExpression(node);
    }

    public override object VisitBooleanLiteral(BooleanLiteral node)
    {
        WriteIndent();
        _writer.WriteLine($"{nameof(BooleanLiteral)} {{ value: {node.Value} }} ");
        return null;
    }

    public override object VisitColumnReference(ColumnReference node)
    {
        WriteIndent();
        _writer.WriteLine(nameof(ColumnReference));
        return base.VisitColumnReference(node);
    }

    public override object VisitComparisonExpression(ComparisonExpression node)
    {
        WriteIndent();
        _writer.WriteLine($"{nameof(ComparisonExpression)} {{ op: {node.Operator} }} ");
        return base.VisitComparisonExpression(node);
    }

    public override object VisitCsvRelation(CsvRelation node)
    {
        WriteIndent();
        _writer.WriteLine(nameof(CsvRelation));
        return base.VisitCsvRelation(node);
    }

    public override object VisitDataTypeExpression(DataTypeExpression node)
    {
        WriteIndent();
        _writer.WriteLine($"{nameof(DataTypeExpression)} {{ type: {node.Type.ToString().ToUpperInvariant()} }} ");
        return null;
    }

    public override object VisitFunction(Function node)
    {
        WriteIndent();
        _writer.WriteLine($"{nameof(Function)} {{ name: {node.Name} }} ");
        return base.VisitFunction(node);
    }

    public override object VisitFieldReference(FieldReference node)
    {
        WriteIndent();
        _writer.WriteLine($"{nameof(FieldReference)} {{ index: {node.Index} }} ");
        return null;
    }

    public override object VisitIdentifier(Identifier node)
    {
        WriteIndent();
        _writer.WriteLine($"{nameof(Identifier)} {{ value: {node.Value}, origin: {node.OriginalValue} }} ");
        return base.VisitIdentifier(node);
    }

    public override object VisitLikePredicate(LikePredicate node)
    {
        WriteIndent();
        _writer.WriteLine($"{nameof(LikePredicate)} {{ case-insensitive: {node.IsCaseInsensitive} }} ");
        return base.VisitLikePredicate(node);
    }

    public override object VisitLimitClause(Limit node)
    {
        WriteIndent();
        _writer.WriteLine($"{nameof(Limit)} {{ offset: {node.Offset}, limit: {node.Count} }} ");
        return null;
    }

    public override object VisitLogicalBinaryExpression(LogicalBinaryExpression node)
    {
        WriteIndent();
        _writer.WriteLine(nameof(LogicalBinaryExpression));
        return base.VisitLogicalBinaryExpression(node);
    }

    public override object VisitNotExpression(NotExpression node)
    {
        WriteIndent();
        _writer.WriteLine(nameof(NotExpression));
        return base.VisitNotExpression(node);
    }

    public override object VisitNullLiteral(NullLiteral node)
    {
        WriteIndent();
        _writer.WriteLine(nameof(NullLiteral));
        return null;
    }

    public override object VisitNumberLiteral(NumberLiteral node)
    {
        WriteIndent();
        _writer.WriteLine($"{nameof(NumberLiteral)} {{ value: {node.Value} }} ");
        return null;
    }

    public override object VisitQualifiedName(QualifiedName node)
    {
        WriteIndent();
        _writer.WriteLine(nameof(QualifiedName));
        return base.VisitQualifiedName(node);
    }

    public override object VisitQuery(Query node)
    {
        WriteIndent();
        _writer.WriteLine(nameof(Query));
        return base.VisitQuery(node);
    }

    public override object VisitSelect(Select node)
    {
        WriteIndent();
        _writer.WriteLine(nameof(Select));
        return base.VisitSelect(node);
    }

    public override object VisitSelectStatement(SelectStatement node)
    {
        WriteIndent();
        _writer.WriteLine(nameof(SelectStatement));
        return base.VisitSelectStatement(node);
    }

    public override object VisitSingleColumn(SingleColumn node)
    {
        WriteIndent();
        _writer.WriteLine(nameof(SingleColumn));
        return base.VisitSingleColumn(node);
    }

    public override object VisitStringLiteral(TextLiteral node)
    {
        WriteIndent();
        _writer.WriteLine($"{nameof(TextLiteral)} {{ value: {node.Value} }} ");
        return null;
    }

    public override object VisitSubqueryRelation(SubqueryRelation node)
    {
        WriteIndent();
        _writer.WriteLine(nameof(SubqueryRelation));
        return base.VisitSubqueryRelation(node);
    }

    public override object VisitVariableReference(VariableReference node)
    {
        WriteIndent();
        _writer.WriteLine($"{nameof(VariableReference)} {{ name: {node.Name} }} ");
        return null;
    }
}

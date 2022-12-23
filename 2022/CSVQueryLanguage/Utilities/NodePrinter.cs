using System.IO;
using CSVQueryLanguage.Parser.Tree;

namespace CSVQueryLanguage.Utilities;

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
        _writer.WriteLine(node.GetType().Name);
        return base.VisitAliasedRelation(node);
    }

    public override object VisitAllColumns(AllColumns node)
    {
        WriteIndent();
        _writer.WriteLine(node.GetType().Name);
        return base.VisitAllColumns(node);
    }

    public override object VisitArithmeticBinaryExpression(ArithmeticBinaryExpression node)
    {
        WriteIndent();
        _writer.WriteLine($"{node.GetType().Name} {{ op: {node.Operator} }} ");
        return base.VisitArithmeticBinaryExpression(node);
    }

    public override object VisitArithmeticUnaryExpression(ArithmeticUnaryExpression node)
    {
        WriteIndent();
        _writer.WriteLine($"{node.GetType().Name} {{ sign: {node.Sign} }} ");
        return base.VisitArithmeticUnaryExpression(node);
    }

    public override object VisitBooleanLiteral(BooleanLiteral node)
    {
        WriteIndent();
        _writer.WriteLine($"{node.GetType().Name} {{ value: {node.Value} }} ");
        return null;
    }

    public override object VisitColumnReference(ColumnReference node)
    {
        WriteIndent();
        _writer.WriteLine(node.GetType().Name);
        return base.VisitColumnReference(node);
    }

    public override object VisitComparisonExpression(ComparisonExpression node)
    {
        WriteIndent();
        _writer.WriteLine($"{node.GetType().Name} {{ op: {node.Operator} }} ");
        return base.VisitComparisonExpression(node);
    }

    public override object VisitCsvRelation(CsvRelation node)
    {
        WriteIndent();
        _writer.WriteLine(node.GetType().Name);
        return base.VisitCsvRelation(node);
    }

    public override object VisitDataTypeExpression(DataTypeExpression node)
    {
        WriteIndent();
        _writer.WriteLine($"{node.GetType().Name} {{ type: {node.Type.ToString().ToUpperInvariant()} }} ");
        return null;
    }

    public override object VisitFunction(Function node)
    {
        WriteIndent();
        _writer.WriteLine($"{node.GetType().Name} {{ name: {node.Name} }} ");
        return base.VisitFunction(node);
    }

    public override object VisitFieldReference(FieldReference node)
    {
        WriteIndent();
        _writer.WriteLine($"{node.GetType().Name} {{ index: {node.Index} }} ");
        return null;
    }

    public override object VisitIdentifier(Identifier node)
    {
        WriteIndent();
        _writer.WriteLine($"{node.GetType().Name} {{ value: {node.Value}, origin: {node.OriginalValue} }} ");
        return base.VisitIdentifier(node);
    }

    public override object VisitLikePredicate(LikePredicate node)
    {
        WriteIndent();
        _writer.WriteLine($"{node.GetType().Name} {{ case-insensitive: {node.IsCaseInsensitive} }} ");
        return base.VisitLikePredicate(node);
    }

    public override object VisitLimitClause(Limit node)
    {
        WriteIndent();
        _writer.WriteLine($"{node.GetType().Name} {{ offset: {node.Offset}, limit: {node.Count} }} ");
        return null;
    }

    public override object VisitLogicalBinaryExpression(LogicalBinaryExpression node)
    {
        WriteIndent();
        _writer.WriteLine(node.GetType().Name);
        return base.VisitLogicalBinaryExpression(node);
    }

    public override object VisitNotExpression(NotExpression node)
    {
        WriteIndent();
        _writer.WriteLine(node.GetType().Name);
        return base.VisitNotExpression(node);
    }

    public override object VisitNullLiteral(NullLiteral node)
    {
        WriteIndent();
        _writer.WriteLine(node.GetType().Name);
        return null;
    }

    public override object VisitNumberLiteral(NumberLiteral node)
    {
        WriteIndent();
        _writer.WriteLine($"{node.GetType().Name} {{ value: {node.Value} }} ");
        return null;
    }

    public override object VisitQualifiedName(QualifiedName node)
    {
        WriteIndent();
        _writer.WriteLine(node.GetType().Name);
        return base.VisitQualifiedName(node);
    }

    public override object VisitQuery(Query node)
    {
        WriteIndent();
        _writer.WriteLine(node.GetType().Name);
        return base.VisitQuery(node);
    }

    public override object VisitSelect(Select node)
    {
        WriteIndent();
        _writer.WriteLine(node.GetType().Name);
        return base.VisitSelect(node);
    }

    public override object VisitSelectStatement(SelectStatement node)
    {
        WriteIndent();
        _writer.WriteLine(node.GetType().Name);
        return base.VisitSelectStatement(node);
    }

    public override object VisitSingleColumn(SingleColumn node)
    {
        WriteIndent();
        _writer.WriteLine(node.GetType().Name);
        return base.VisitSingleColumn(node);
    }

    public override object VisitStringLiteral(StringLiteral node)
    {
        WriteIndent();
        _writer.WriteLine($"{node.GetType().Name} {{ value: {node.Value} }} ");
        return null;
    }

    public override object VisitSubqueryRelation(SubqueryRelation node)
    {
        WriteIndent();
        _writer.WriteLine(node.GetType().Name);
        return base.VisitSubqueryRelation(node);
    }
}

namespace CSVQueryLanguage.Parser.Tree;

public interface INode
{
    TResult Accept<TResult>(INodeVisitor<TResult> visitor);
}

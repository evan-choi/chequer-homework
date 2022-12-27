namespace CSVQueryLanguage.Tree;

public interface INode
{
    TResult Accept<TResult>(INodeVisitor<TResult> visitor);
}

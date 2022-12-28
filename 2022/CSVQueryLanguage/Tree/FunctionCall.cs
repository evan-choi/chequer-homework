using CSVQueryLanguage.Common.Functions;

namespace CSVQueryLanguage.Tree;

public sealed class FunctionCall : IExpression
{
    public string Name { get; }

    public IFunction Target { get; }

    public IExpression[] Arguments { get; }

    public FunctionCall(string name, IFunction target, IExpression[] arguments)
    {
        Name = name;
        Target = target;
        Arguments = arguments;
    }

    public TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
        return visitor.VisitFunctionCall(this);
    }
}

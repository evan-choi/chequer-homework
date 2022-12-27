namespace CSVQueryLanguage.Tree;

// RULE: primaryExpression > #..Literal
public interface ILiteral : IExpression
{
    object Value { get; }
}

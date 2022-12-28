namespace CSVQueryLanguage.Common.Functions;

public interface IFunction
{
    object Invoke(object[] arguments);
}

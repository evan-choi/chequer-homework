namespace CSVQueryLanguage.Driver;

public interface IRuntimeVariable<T> : IRuntimeVariable
{
    new T Value { get; set; }

    object IRuntimeVariable.Value
    {
        get => Value;
        set => Value = (T)value;
    }
}

public interface IRuntimeVariable
{
    object Value { get; set; }
}

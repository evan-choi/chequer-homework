using System;

namespace CSVQueryLanguage;

public class CqlException : Exception
{
    public CqlException(string message) : base(message)
    {
    }
}

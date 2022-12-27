using System;
using System.Collections;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Driver.Interpretation;

public static partial class AtomicInteractive
{
    private static readonly Func<object, object, bool>[] _comparisonOperators =
    {
        static (l, r) => Comparer.Default.Compare(l, r) == 0,
        static (l, r) => Comparer.Default.Compare(l, r) != 0,
        static (l, r) => Comparer.Default.Compare(l, r) < 0,
        static (l, r) => Comparer.Default.Compare(l, r) <= 0,
        static (l, r) => Comparer.Default.Compare(l, r) > 0,
        static (l, r) => Comparer.Default.Compare(l, r) >= 0
    };

    public static bool Interpret(ComparisonOperator op, object left, object right)
    {
        return _comparisonOperators[(int)op](left, right);
    }
}

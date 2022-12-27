using System;
using CSVQueryLanguage.Tree;

namespace CSVQueryLanguage.Driver.Interpretation;

public static partial class AtomicInteractive
{
    // <number> [+-*/%] <number>
    private static readonly Func<double, double, double>[] _numberArithmeticOperators =
    {
        static (l, r) => l + r,
        static (l, r) => l - r,
        static (l, r) => l * r,
        static (l, r) => l / r,
        static (l, r) => l % r
    };

    // <time> [+-*/%] <time>
    private static readonly Func<TimeOnly, TimeOnly, TimeOnly>[] _timeArithmeticOperators =
    {
        static (l, r) => new TimeOnly(l.Ticks + r.Ticks),
        static (l, r) => new TimeOnly(l.Ticks - r.Ticks),
        static (l, r) => new TimeOnly(l.Ticks * r.Ticks),
        static (l, r) => new TimeOnly(l.Ticks / r.Ticks),
        static (l, r) => new TimeOnly(l.Ticks % r.Ticks)
    };

    // <timestamp> [+-] <date>
    private static readonly Func<DateTimeOffset, DateOnly, DateTimeOffset>[] _timestampAndDateArithmeticOperators =
    {
        static (l, r) => l.AddYears(r.Year).AddMonths(r.Month).AddDays(r.Day),
        static (l, r) => l.AddYears(-r.Year).AddMonths(-r.Month).AddDays(-r.Day),
        static (_, _) => throw CqlErrors.NotSupportedBinaryOperator(ArithmeticOperator.Multiply, DataType.Timestamp, DataType.Date),
        static (_, _) => throw CqlErrors.NotSupportedBinaryOperator(ArithmeticOperator.Divide, DataType.Timestamp, DataType.Date),
        static (_, _) => throw CqlErrors.NotSupportedBinaryOperator(ArithmeticOperator.Modulus, DataType.Timestamp, DataType.Date),
    };

    // <timestamp> [+-] <time>
    private static readonly Func<DateTimeOffset, TimeOnly, DateTimeOffset>[] _timestampAndTimeArithmeticOperators =
    {
        static (l, r) => l.AddTicks(r.Ticks),
        static (l, r) => l.AddTicks(-r.Ticks),
        static (_, _) => throw CqlErrors.NotSupportedBinaryOperator(ArithmeticOperator.Multiply, DataType.Timestamp, DataType.Time),
        static (_, _) => throw CqlErrors.NotSupportedBinaryOperator(ArithmeticOperator.Divide, DataType.Timestamp, DataType.Time),
        static (_, _) => throw CqlErrors.NotSupportedBinaryOperator(ArithmeticOperator.Modulus, DataType.Timestamp, DataType.Time),
    };

    public static bool CanInterpret(ArithmeticOperator op, DataType? left, DataType? right)
    {
        switch ((left, right))
        {
            case (DataType.Number, DataType.Number):
            case (DataType.Time, DataType.Time):
                return true;

            case (DataType.Timestamp, DataType.Time or DataType.Date):
                return op is ArithmeticOperator.Add or ArithmeticOperator.Divide;
        }

        return false;
    }

    public static bool CanInterpret(ArithmeticSign sign, DataType? value)
    {
        return value is DataType.Number;
    }

    public static double Interpret(ArithmeticOperator op, double left, double right)
    {
        return _numberArithmeticOperators[(int)op](left, right);
    }

    public static TimeOnly Interpret(ArithmeticOperator op, TimeOnly left, TimeOnly right)
    {
        return _timeArithmeticOperators[(int)op](left, right);
    }

    public static DateTimeOffset Interpret(ArithmeticOperator op, DateTimeOffset left, DateOnly right)
    {
        return _timestampAndDateArithmeticOperators[(int)op](left, right);
    }

    public static DateTimeOffset Interpret(ArithmeticOperator op, DateTimeOffset left, TimeOnly right)
    {
        return _timestampAndTimeArithmeticOperators[(int)op](left, right);
    }

    public static double Interpret(ArithmeticSign sign, double value)
    {
        if (sign is ArithmeticSign.Minus)
            return -value;

        return value;
    }
}

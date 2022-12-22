using CSVQueryLanguage.Parser;

namespace CSVQueryLanguage.CLI;

internal static class Program
{
    public static void Main(string[] args)
    {
        var parser = new CqlParser();
        parser.Parse("SELECT * FROM .csv AS faf");
        parser.Parse("SELECT * FROM test");
        parser.Parse(@"SELECT * FROM ""C:\\Program Files\\Ad/*a*/""""obe\\test.csv""");
    }
}

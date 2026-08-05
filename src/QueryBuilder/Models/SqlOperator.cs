namespace QueryBuilder;

public class SqlOperator
{
    public static readonly SqlOperator Equal = new("=");
    public static readonly SqlOperator Greater = new(">");

    private SqlOperator(string symbol)
    {
        Symbol = symbol;
    }

    public string Symbol { get; }

    public override string ToString()
    {
        return Symbol;
    }
}
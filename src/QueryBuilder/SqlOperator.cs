namespace QueryBuilder;

public class SqlOperator
{
    public string Symbol { get; }
    private SqlOperator(string symbol)
    {
        Symbol = symbol;
    }

    public static readonly SqlOperator Equal = new("=");
    public static readonly SqlOperator Greater = new(">");

    public override string ToString()
    {
        return Symbol;
    }
}
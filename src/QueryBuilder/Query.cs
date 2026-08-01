namespace QueryBuilder;

public class Query
{
    public List<string> SelectColumns = new();
    public string FromTable = string.Empty;
    public List<(string column, object value)> WhereEntries = new();

    public Query Select(params string[] columns)
    {
        SelectColumns = columns.ToList();
        return this;
    }

    public Query From(string table)
    {
        FromTable = table;
        return this;
    }

    public Query Where(string column, object value)
    {
        WhereEntries.Add((column, value));
        return this;
    }

}

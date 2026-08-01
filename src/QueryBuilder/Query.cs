namespace QueryBuilder;

public class Query
{
    private readonly List<string> _selectColumns = new();
    private readonly List<(string column, object value)> _whereEntries = new();

    public IReadOnlyList<string> SelectColumns => _selectColumns.AsReadOnly();
    public IReadOnlyList<(string column, object value)> WhereEntries => _whereEntries.AsReadOnly();
    public string FromTable {get; private set;} = string.Empty;

    public Query Select(params string[] columns)
    {
        _selectColumns.Clear();
        _selectColumns.AddRange(columns);
        return this;
    }

    public Query From(string table)
    {
        FromTable = table;
        return this;
    }

    public Query Where(string column, object value)
    {
        _whereEntries.Add((column, value));
        return this;
    }

}

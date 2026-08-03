namespace QueryBuilder;

public class Query
{
    private readonly List<string> _selectColumns = new();
    private readonly List<WhereClause> _whereEntries = new();

    public IReadOnlyList<string> SelectColumns => _selectColumns.AsReadOnly();
    public IReadOnlyList<WhereClause> WhereEntries => _whereEntries.AsReadOnly();
    public string FromTable { get; private set; } = string.Empty;

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

    public Query Where(string column, SqlOperator op, object value)
    {
        _whereEntries.Add(new WhereClause(column, op, value));
        return this;
    }

    public Query WhereEquals(string column, object value)
    {
        Where(column, SqlOperator.Equal, value);
        return this;
    }
}
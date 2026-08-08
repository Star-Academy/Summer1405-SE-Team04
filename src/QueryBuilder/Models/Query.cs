using QueryBuilder.Utils;

namespace QueryBuilder.Models;

public class Query
{
    private readonly List<string> _selectColumns = new();
    private readonly List<WhereClause> _whereEntries = new();

    public IReadOnlyList<string> SelectColumns => _selectColumns.AsReadOnly();
    public IReadOnlyList<WhereClause> WhereEntries => _whereEntries.AsReadOnly();
    public string FromTable { get; private set; } = string.Empty;

    private readonly IValidator _validator;    

    public Query(): this(new SqlValidator()){}
    public Query(IValidator validator)
    {
        _validator=validator;
    }
    public Query Select(params string[] columns)
    {
        if (!_validator.ValidateStringsNotEmpty(columns))
            throw new ArgumentException("Every column must be valid name.");
        
        _selectColumns.Clear();
        _selectColumns.AddRange(columns);
        return this;
    }

    public Query From(string table)
    {
        if (!_validator.ValidateStringsNotEmpty(table))
            throw new ArgumentException("TableName must be valid name.");
        FromTable = table;
        return this;
    }

    public Query Where(string column, object value)
    {
        return Where(column, "=", value);
    }

    public Query Where(string column, string sqlOperator, object value)
    {
        if (!_validator.ValidateStringsNotEmpty(column))
            throw new ArgumentException("ColumnName must be valid name.");
        _whereEntries.Add(new WhereClause(column, sqlOperator, value));
        return this;
    }
}
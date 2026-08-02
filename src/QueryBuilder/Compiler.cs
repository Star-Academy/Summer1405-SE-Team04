using System.Text;

namespace QueryBuilder;

public abstract class Compiler : ICompiler
{
    public (string Sql, List<object> Bindings) Compile(Query query)
    {
        var sqlBuilder = new StringBuilder();
        var bindings = new List<object>();

        _validateQuery(query);
        _appendSelectCommand(query, sqlBuilder);
        _appendFromCommand(query, sqlBuilder);
        _appendWhereCommand(query, sqlBuilder, bindings);

        return (sqlBuilder.ToString(), bindings);
    }

    private void _appendSelectCommand(Query query, StringBuilder sb)
    {
        sb.Append("SELECT ");
        sb.Append(string.Join(", ", query.SelectColumns.Select(WrapIdentifier)));
    }

    private void _appendFromCommand(Query query, StringBuilder sb)
    {
        sb.Append($" FROM {WrapIdentifier(query.FromTable)}");
    }

    private void _appendWhereCommand(Query query, StringBuilder sb, List<object> bindings)
    {
        if (query.WhereEntries.Count == 0)
            return;

        sb.Append(" WHERE ");
        sb.Append(string.Join(" AND ", query.WhereEntries.Select((entry, index) =>
        {
            bindings.Add(entry.value);
            return $"{WrapIdentifier(entry.column)} = {FormatParameter(index)}";
        })));
    }

    private static void _validateQuery(Query query)
    {
        if (query.SelectColumns.Count == 0)
            throw new ArgumentException("no column");
        if (query.FromTable.Length == 0)
            throw new ArgumentException("no from table");
    }

    public string FormatParameter(int index)
    {
        return $"@p{index}";
    }

    public abstract string WrapIdentifier(string identifier);
}
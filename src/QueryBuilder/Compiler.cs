using System.Text;
using QueryBuilder;

public interface ICompiler
{
    public (string Sql, List<object> Bindings) Compile(Query query);
}

public abstract class Compiler : ICompiler
{
    public (string Sql, List<object> Bindings) Compile(Query query)
    {
        var sqlBuilder = new StringBuilder();
        ValidateQuery(query);
        var bindings = new List<object>();

        sqlBuilder.Append("SELECT ");
        sqlBuilder.Append(string.Join(", ", query.SelectColumns.Select(c => WrapIndentifier(c))));

        sqlBuilder.Append($" FROM {WrapIndentifier(query.FromTable)}");

        if (query.WhereEntries.Count != 0)
        {
            sqlBuilder.Append(" WHERE ");
            sqlBuilder.Append(string.Join(" AND ", query.WhereEntries.Select((entry, index) =>
            {
                bindings.Add(entry.value);
                return $"{WrapIndentifier(entry.column)} = {FormatParameter(index)}";
            })));
        }
        return (sqlBuilder.ToString(), bindings);
    }

    private void ValidateQuery(Query query)
    {
        if (query.SelectColumns.Count == 0)
            throw new ArgumentException("no column");
        if (query.FromTable.Length == 0)
            throw new ArgumentException("no from table");
    }
    protected abstract string WrapIndentifier(string identifier);
    protected abstract string FormatParameter(int index);

}

public class PostgresCompiler : Compiler
{
    protected override string FormatParameter(int index)
    {
        return $"${index + 1}";
    }

    protected override string WrapIndentifier(string identifier)
    {
        return $"\"{identifier}\"";
    }
}


public class SqlServerCompiler : Compiler
{
    protected override string FormatParameter(int index)
    {
        return $"@p{index}";
    }

    protected override string WrapIndentifier(string identifier)
    {
        return $"[{identifier}]";
    }
}
using System.Security.Claims;
using QueryBuilder;

public interface Compiler
{
    public (string Sql, List<object> Bindings) Compile(Query query);
}

public class PostgresCompiler : Compiler
{
    public (string Sql, List<object> Bindings) Compile(Query query)
    {
        var bindings = new List<object>();

        if (query.SelectColumns.Count == 0)
            throw new ArgumentException("no column");
        if (query.FromTable.Length == 0)
            throw new ArgumentException("no from table");

        string sql = "SELECT ";
        for (int i = 0; i < query.SelectColumns.Count; ++i)
        {
            sql += $"\"{query.SelectColumns[i]}\"";
            if (i != query.SelectColumns.Count - 1)
                sql += ", ";
        }

        sql += $" FROM \"{query.FromTable}\"";

        if (query.WhereEntries.Count != 0)
        {
            sql += " WHERE ";
            for (int i = 0; i < query.WhereEntries.Count; i++)
            {
                var (column, value) = query.WhereEntries[i];
                sql += $"\"{column}\" = ${i + 1}";
                bindings.Add(value);
                if (i != query.WhereEntries.Count - 1)
                    sql += " AND ";

            }
        }
        return (sql, bindings);
    }
}


public class SqlServerCompiler : Compiler
{
    public (string Sql, List<object> Bindings) Compile(Query query)
    {
        var bindings = new List<object>();

        if (query.SelectColumns.Count == 0)
            throw new ArgumentException("no column");
        if (query.FromTable.Length == 0)
            throw new ArgumentException("no from table");

        string sql = "SELECT ";
        for (int i = 0; i < query.SelectColumns.Count; ++i)
        {
            sql += $"[{query.SelectColumns[i]}]";
            if (i != query.SelectColumns.Count - 1)
                sql += ", ";
        }

        sql += $" FROM [{query.FromTable}]";

        if (query.WhereEntries.Count != 0)
        {
            sql += " WHERE ";
            for (int i = 0; i < query.WhereEntries.Count; i++)
            {
                var (column, value) = query.WhereEntries[i];
                sql += $"[{column}] = @p{i}";
                bindings.Add(value);
                if (i != query.WhereEntries.Count - 1)
                    sql += " AND ";

            }
        }
        return (sql, bindings);
    }
}
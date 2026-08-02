using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Npgsql;
using QueryBuilder;

namespace SqlApp;

public abstract class SqlService
{
    private readonly Compiler _compiler;
    private readonly string _connString;

    protected SqlService(string connString)
    {
        _connString = connString;
        _compiler = createCompiler();
    }

    public async Task<DbDataReader> ExecuteQuery(Query query)
    {
        var queryResult = _compiler.Compile(query);

        var connection = createConnection(_connString);
        await connection.OpenAsync();

        var cmd = connection.CreateCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = queryResult.Sql;

        for (var i = 0; i < queryResult.Bindings.Count; i++)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = _compiler.FormatParameter(i);
            parameter.Value = queryResult.Bindings[i];
            cmd.Parameters.Add(parameter);
        }

        return await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
    }

    protected abstract Compiler createCompiler();
    protected abstract DbConnection createConnection(string connectionString);
}

public class PgService : SqlService
{
    public PgService(string connectionString) : base(connectionString)
    {
    }

    protected override Compiler createCompiler()
    {
        return new PostgresCompiler();
    }

    protected override DbConnection createConnection(string connectionString)
    {
        return new NpgsqlConnection(connectionString);
    }
}

public class MsService : SqlService
{
    public MsService(string connectionString) : base(connectionString)
    {
    }

    protected override Compiler createCompiler()
    {
        return new SqlServerCompiler();
    }

    protected override DbConnection createConnection(string connectionString)
    {
        return new SqlConnection(connectionString);
    }
}
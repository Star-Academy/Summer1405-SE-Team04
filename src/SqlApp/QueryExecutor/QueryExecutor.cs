using System.Data;
using System.Data.Common;
using QueryBuilder.Compilers;
using QueryBuilder.Models;

namespace SqlApp.QueryExecutor;

public class QueryExecutor : IQueryExecutor
{
    private readonly ICompiler _compiler;
    private readonly string _connectionString;
    private readonly DbProviderFactory _dbFactory;

    public QueryExecutor(DbProviderFactory dbFactory, ICompiler compiler, string connectionString)
    {
        _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<DbDataReader> ExecuteQuery(Query query)
    {
        ArgumentNullException.ThrowIfNull(query);
        var queryResult = _compiler.Compile(query);
        var connection = _dbFactory.CreateConnection()
                         ?? throw new InvalidOperationException($"Cannot create connection for {query}");
        connection.ConnectionString = _connectionString;
        try
        {
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = queryResult.SqlQuery;

            for (var i = 0; i < queryResult.Bindings.Count; i++)
            {
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = queryResult.Bindings[i].parameterName;
                parameter.Value = queryResult.Bindings[i].value;
                cmd.Parameters.Add(parameter);
            }

            return await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }
}
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using QueryBuilder.Compilers;
using QueryBuilder.Models;

namespace SqlApp.QueryExecutor;


public class QueryExecutor(DbProviderFactory dbFactory, ICompiler compiler, string connectionString)
    : IQueryExecutor
{
    public async Task<DbDataReader> ExecuteQuery(Query query)
    {
        var queryResult = compiler.Compile(query);
        var connection = dbFactory.CreateConnection()
                         ?? throw new InvalidOperationException($"Cannot create connection for {query}");
        connection.ConnectionString = connectionString;
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
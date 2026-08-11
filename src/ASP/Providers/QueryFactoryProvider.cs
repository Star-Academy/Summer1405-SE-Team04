using Microsoft.Data.SqlClient;
using Npgsql;
using SqlApp;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Providers;

public class QueryFactoryProvider : IQueryFactoryProvider
{
    private readonly Config _config = new Config();
    private readonly Lazy<QueryFactory> _postgresQueryFactory;
    private readonly Lazy<QueryFactory> _sqlServerQueryFactory;

    public QueryFactoryProvider()
    {
        _postgresQueryFactory = new Lazy<QueryFactory>(_createPostgresQueryFactory);
        _sqlServerQueryFactory = new Lazy<QueryFactory>(_createSqlServerQueryFactory);
    }

    public void Dispose()
    {
        if (_postgresQueryFactory.IsValueCreated)
            _postgresQueryFactory.Value.Dispose();
        if (_sqlServerQueryFactory.IsValueCreated)
            _sqlServerQueryFactory.Value.Dispose();
    }

    public QueryFactory GetQueryFactory(string dbName)
    {

        return dbName switch
        {
            "postgres" => _postgresQueryFactory.Value,
            "sqlserver" => _sqlServerQueryFactory.Value,
            _ => throw new ArgumentException(
                $"Unknown database '{dbName}'. Supported values are 'postgres' and 'sqlserver'.",
                nameof(dbName))
        };
    }
    private QueryFactory _createPostgresQueryFactory()
    {
        var connectionString = _config.BuildPostgresConnectionString();
        var connection = new NpgsqlConnection(connectionString);

        return new QueryFactory(connection, new PostgresCompiler());
    }
    private QueryFactory _createSqlServerQueryFactory()
    {
        var connectionString = _config.BuildSqlServerConnectionString();
        var connection = new SqlConnection(connectionString);

        return new QueryFactory(connection, new SqlServerCompiler());
    }

}

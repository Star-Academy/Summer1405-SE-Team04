using Microsoft.Data.SqlClient;
using Npgsql;
using SqlApp;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Providers;

public class QueryFactoryProvider : IQueryFactoryProvider
{
    private readonly Config _config = new Config();
    private readonly QueryFactory _postgresQueryFactory;
    private readonly QueryFactory _sqlServerQueryFactory;

    public QueryFactoryProvider()
    {
        _postgresQueryFactory = _createPostgresQueryFactory();
        _sqlServerQueryFactory = _createSqlServerQueryFactory();
    }

    public void Dispose()
    {
        _postgresQueryFactory.Dispose();
        _sqlServerQueryFactory.Dispose();
    }

    public QueryFactory GetQueryFactory(string dbName)
    {

        return dbName switch
        {
            "postgres" => _postgresQueryFactory,
            "sqlserver" => _sqlServerQueryFactory,
            _ => throw new ArgumentException()
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
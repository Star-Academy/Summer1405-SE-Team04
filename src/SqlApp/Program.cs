using Npgsql;
using QueryBuilder;
using Microsoft.Data.SqlClient;
using System.Data;
using DotNetEnv;
using System.Data.Common;
using SqlApp;

Env.TraversePath().Load();


var query = new Query()
    .From("Student")
    .Select("StudentNumber", "FirstName")
    .Where("IsMale", true);



await RunPgQuery(query);
await RunMsQuery(query);

return;


async Task RunPgQuery(Query query)
{
    Console.WriteLine("Postgres Execution Result :");
    var pgConnStr = BuildPostgresConnectionString();
    var pgService = new PgService(pgConnStr);
    await PrintQueryResultAsync(await pgService.ExecuteQuery(query));
}

async Task RunMsQuery(Query query)
{
    Console.WriteLine("Microsoft SQL Server Execution Result :");
    var msConnStr = BuildSqlServerConnectionString();
    var msService = new MsService(msConnStr);
    await PrintQueryResultAsync(await msService.ExecuteQuery(query));
}


async Task PrintQueryResultAsync(DbDataReader reader)
{
    while (await reader.ReadAsync())
    {
        Console.WriteLine($"STID: {reader["StudentNumber"]}, FirstName: {reader["FirstName"]}");
    }
}

static string BuildPostgresConnectionString()
{
    string host = Environment.GetEnvironmentVariable("PG_HOST") ?? "localhost";
    string port = Environment.GetEnvironmentVariable("PG_PORT") ?? "5432";
    string db = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "team04";
    string user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "mohaymen";
    string pass = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? string.Empty;

    return $"Host={host};Port={port};Database={db};Username={user};Password={pass};";
}

static string BuildSqlServerConnectionString()
{
    string server = Environment.GetEnvironmentVariable("MS_SERVER") ?? "localhost,1433";
    string db = Environment.GetEnvironmentVariable("MS_DATABASE") ?? "mohaymen-sqlserver";
    string pass = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD") ?? string.Empty;

    return $"Server={server};Database={db};User Id=sa;Password={pass};Connection Timeout=30;TrustServerCertificate=True;";
}
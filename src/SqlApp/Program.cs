using System.Data.Common;
using DotNetEnv;
using QueryBuilder;
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
    var rowNumber = 1;
    var fields = new List<string>();
    for (var i = 0; i < reader.FieldCount; i++) fields.Add(reader.GetName(i));
    Console.WriteLine(string.Join(", ", fields));
    while (await reader.ReadAsync())
        Console.WriteLine($"{rowNumber++}: {string.Join(", ", fields.Select(f => reader[f]))}");
}

static string BuildPostgresConnectionString()
{
    var host = Environment.GetEnvironmentVariable("PG_HOST") ?? "localhost";
    var port = Environment.GetEnvironmentVariable("PG_PORT") ?? "5432";
    var db = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "team04";
    var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "mohaymen";
    var pass = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? string.Empty;

    return $"Host={host};Port={port};Database={db};Username={user};Password={pass};";
}

static string BuildSqlServerConnectionString()
{
    var server = Environment.GetEnvironmentVariable("MS_SERVER") ?? "localhost,1433";
    var db = Environment.GetEnvironmentVariable("MS_DATABASE") ?? "mohaymen-sqlserver";
    var pass = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD") ?? string.Empty;

    return
        $"Server={server};Database={db};User Id=sa;Password={pass};Connection Timeout=30;TrustServerCertificate=True;";
}
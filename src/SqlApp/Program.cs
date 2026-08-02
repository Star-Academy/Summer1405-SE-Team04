using Npgsql;
using QueryBuilder;
using Microsoft.Data.SqlClient;
using System.Data;
using DotNetEnv;
using System.Data.Common;

Env.TraversePath().Load();

string pgConnStr = BuildPostgresConnectionString();
string msConnStr = BuildSqlServerConnectionString();

var query = new Query()
    .From("Student")
    .Select("StudentNumber", "FirstName")
    .Where("IsMale", true);


// Compile for PostgreSQL
var pgCompiler = new PostgresCompiler();
var pgResult = pgCompiler.Compile(query);

// Compile for SQL Server
var msCompiler = new SqlServerCompiler();
var msResult = msCompiler.Compile(query);


await using var dataSource = NpgsqlDataSource.Create(pgConnStr);
await using (var cmd = dataSource.CreateCommand(pgResult.Sql.ToLower()))
{
    foreach (var binding in pgResult.Bindings)
    {
        cmd.Parameters.AddWithValue(binding);
    }

    await using var reader = await cmd.ExecuteReaderAsync();
    await PrintQueryResultAsync(reader);

}

using (var connection = new SqlConnection(msConnStr))
{
    await connection.OpenAsync();
    Console.WriteLine("Connected successfully.");
    using (var command = connection.CreateCommand())
    {
        command.CommandType = CommandType.Text;
        command.CommandText = msResult.Sql;

        for (int i = 0; i < msResult.Bindings.Count; i++)
        {
            command.Parameters.AddWithValue($"@p{i}", msResult.Bindings[i]);
        }

        await using var reader = await command.ExecuteReaderAsync();
        await PrintQueryResultAsync(reader);
    }
    Console.WriteLine("Press any key to finish...");
    Console.ReadKey(true);
}

static async Task PrintQueryResultAsync(DbDataReader reader)
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
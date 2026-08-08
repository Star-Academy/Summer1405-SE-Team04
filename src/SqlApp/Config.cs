using DotNetEnv;

namespace SqlApp;

internal class Config
{
    private readonly string PgHost = Environment.GetEnvironmentVariable("PG_HOST") ?? "localhost";
    private readonly string PgPort = Environment.GetEnvironmentVariable("PG_PORT") ?? "5432";
    private readonly string PgDb = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "team04";
    private readonly string PgUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "mohaymen";
    private readonly string PgPass = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? string.Empty;

    private readonly string MsServer = Environment.GetEnvironmentVariable("MS_SERVER") ?? "localhost,1433";
    private readonly string MsDb = Environment.GetEnvironmentVariable("MS_DATABASE") ?? "mohaymen-sqlserver";
    private readonly string MsPass = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD") ?? string.Empty;
    public Config()
    {
        Env.TraversePath().Load();
    }
    public string BuildPostgresConnectionString()
    {
        return $"Host={PgHost};Port={PgPort};Database={PgDb};Username={PgUser};Password={PgPass};";
    }

    public string BuildSqlServerConnectionString()
    {
        return $"Server={MsServer};Database={MsDb};User Id=sa;Password={MsPass};Connection Timeout=30;TrustServerCertificate=True;";
    }
}
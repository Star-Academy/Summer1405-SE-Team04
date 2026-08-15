namespace SqlApp;

internal class Config : Iconfig
{
    public string MsDb { get; } = Environment.GetEnvironmentVariable("MS_DATABASE") ?? "mohaymen-sqlserver";

    public string MsHost { get; } = Environment.GetEnvironmentVariable("MS_HOST") ?? "localhost";
    public string MsPass { get; } = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD") ?? string.Empty;
    public string MsPort { get; } = Environment.GetEnvironmentVariable("MS_PORT") ?? "1433";
    public string MsServer { get; }
    public string PgDb { get; } = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "team04";
    public string PgHost { get; } = Environment.GetEnvironmentVariable("PG_HOST") ?? "localhost";
    public string PgPass { get; } = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? string.Empty;
    public string PgPort { get; } = Environment.GetEnvironmentVariable("PG_PORT") ?? "5432";
    public string PgUser { get; } = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "mohaymen";

    public Config()
    {
        MsServer = $"{MsHost},{MsPort}";
    }

    public string BuildPostgresConnectionString()
    {
        return $"Host={PgHost};Port={PgPort};Database={PgDb};Username={PgUser};Password={PgPass};";
    }

    public string BuildSqlServerConnectionString()
    {
        return
            $"Server={MsServer};Database={MsDb};User Id=sa;Password={MsPass};Connection Timeout=30;TrustServerCertificate=True;";
    }
}
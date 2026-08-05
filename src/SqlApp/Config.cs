internal static class Config
{
    public static string BuildPostgresConnectionString()
    {
        var host = Environment.GetEnvironmentVariable("PG_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("PG_PORT") ?? "5432";
        var db = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "team04";
        var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "mohaymen";
        var pass = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? string.Empty;

        return $"Host={host};Port={port};Database={db};Username={user};Password={pass};";
    }

    public static string BuildSqlServerConnectionString()
    {
        var server = Environment.GetEnvironmentVariable("MS_SERVER") ?? "localhost,1433";
        var db = Environment.GetEnvironmentVariable("MS_DATABASE") ?? "mohaymen-sqlserver";
        var pass = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD") ?? string.Empty;

        return
            $"Server={server};Database={db};User Id=sa;Password={pass};Connection Timeout=30;TrustServerCertificate=True;";
    }
}
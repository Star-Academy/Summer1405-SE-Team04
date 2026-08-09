using Npgsql;
using Testcontainers.PostgreSql;

namespace IntegrationTests.Postgres;

public class PostgresFixture : IAsyncLifetime
{
    public PostgreSqlContainer PostgresContainer { get; }

    public PostgresFixture()
    {
        PostgresContainer = new PostgreSqlBuilder()
        .WithImage("hub.hamdocker.ir/library/postgres:16")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    }
    public async ValueTask InitializeAsync()
    {
        await PostgresContainer.StartAsync();

        await CreateSchemaAsync();
        await SeedDataAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await PostgresContainer.DisposeAsync();
    }

    public async Task CreateSchemaAsync()
    {
        await using var connection = new NpgsqlConnection(PostgresContainer.GetConnectionString());
        await connection.OpenAsync();

        var queryString = """
        CREATE TABLE "Student"(
            "ID"            int PRIMARY KEY,
            "FirstName"     varchar(20),
            "IsMale"      BOOL,
            "Age"         int
        );
        """;

        await using var command = new NpgsqlCommand(queryString, connection);
        await command.ExecuteNonQueryAsync();
    }

    public async Task SeedDataAsync()
    {
        await using var connection = new NpgsqlConnection(PostgresContainer.GetConnectionString());
        await connection.OpenAsync();

        var queryString = """
        INSERT INTO "Student" VALUES
        ($1, $2, $3, $4);
        """;


        foreach (var student in TestData.Students)
        {
            await using var command = new NpgsqlCommand(queryString, connection);

            command.Parameters.AddWithValue(student.ID);
            command.Parameters.AddWithValue(student.FirstName);
            command.Parameters.AddWithValue(student.IsMale);
            command.Parameters.AddWithValue(student.Age);

            await command.ExecuteNonQueryAsync();
        }

    }

}
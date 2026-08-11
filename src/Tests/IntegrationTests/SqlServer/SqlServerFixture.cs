using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace IntegrationTests.SqlServer;

public class SqlServerFixture : IAsyncLifetime
{
    public MsSqlContainer MsContainer;

    public SqlServerFixture()
    {
        MsContainer = new MsSqlBuilder("mcr.hamdocker.ir/mssql/server:2022-latest")
        .Build();
    }
    public async ValueTask InitializeAsync()
    {
        await MsContainer.StartAsync();

        await CreateSchemaAsync();
        await SeedDataAsync();
    }
    public async ValueTask DisposeAsync()
    {
        await MsContainer.DisposeAsync();
    }
    public async Task CreateSchemaAsync()
    {
        await using var connection = new SqlConnection(MsContainer.GetConnectionString());
        await connection.OpenAsync();

        var queryString = """
        CREATE TABLE [Student](
            [ID]            int PRIMARY KEY,
            [FirstName]     nvarchar(20),
            [IsMale]        bit,
            [Age]           int
        );
        """;

        await using var command = new SqlCommand(queryString, connection);
        await command.ExecuteNonQueryAsync();
    }

    public async Task SeedDataAsync()
    {
        await using var connection = new SqlConnection(MsContainer.GetConnectionString());
        await connection.OpenAsync();

        var queryString = """
        INSERT INTO "Student" VALUES
        (@p0, @p1, @p2, @p3);
        """;

        foreach (var student in TestData.Students)
        {
            await using var command = new SqlCommand(queryString, connection);
            command.Parameters.AddWithValue("@p0", student.Id);
            command.Parameters.AddWithValue("@p1", student.FirstName);
            command.Parameters.AddWithValue("@p2", student.IsMale);
            command.Parameters.AddWithValue("@p3", student.Age);

            await command.ExecuteNonQueryAsync();
        }
    }

}
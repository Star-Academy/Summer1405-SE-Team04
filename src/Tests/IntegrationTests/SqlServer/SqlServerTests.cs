using AwesomeAssertions;
using Microsoft.Data.SqlClient;
using QueryBuilder.Compilers;
using QueryBuilder.Factory;
using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;
using QueryBuilder.Utils;
using SqlApp.QueryExecutor;

namespace IntegrationTests.SqlServer;

public class SqlServerTest : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _sqlServerFixture;

    public SqlServerTest(SqlServerFixture sqlServerFixture)
    {
        _sqlServerFixture = sqlServerFixture;
    }

    [Fact]
    public async Task Execute_Should_ReturnAll_When_SelectAllRows()
    {
        //Arrange
        var query = new Query().Select("ID", "FirstName", "IsMale", "Age").From("Student");
        var compiler = new Compiler(PostgresParameterFixer.Instance,
        new PostgresClauseCompilerFactory(),
        new SqlValidator());
        var executator = new QueryExecutor(SqlClientFactory.Instance,
        compiler,
        _sqlServerFixture.MsContainer.GetConnectionString());

        //Act
        await using var reader = await executator.ExecuteQuery(query);

        //Assert
        for (int i = 0; i < TestData.Students.Count; i++)
        {
            var expected = TestData.Students[i];

            (await reader.ReadAsync()).Should().BeTrue();

            reader.GetInt32(0).Should().Be(expected.ID);
            reader.GetString(1).Should().Be(expected.FirstName);
            reader.GetBoolean(2).Should().Be(expected.IsMale);
            reader.GetInt32(3).Should().Be(expected.Age);
        }

        (await reader.ReadAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task Execute_Should_ReturnAllColumns_When_WhereNameIsAli()
    {
        //Arrange
        var query = new Query().Select("ID", "FirstName", "IsMale", "Age").From("Student").Where("FirstName", "Ali");
        var compiler = new Compiler(PostgresParameterFixer.Instance,
        new PostgresClauseCompilerFactory(),
        new SqlValidator());
        var executator = new QueryExecutor(SqlClientFactory.Instance,
        compiler,
        _sqlServerFixture.MsContainer.GetConnectionString());

        //Act
        await using var reader = await executator.ExecuteQuery(query);

        //Assert
        (await reader.ReadAsync()).Should().BeTrue();

        reader.GetInt32(0).Should().Be(1);
        reader.GetString(1).Should().Be("Ali");
        reader.GetBoolean(2).Should().Be(true);
        reader.GetInt32(3).Should().Be(19);

        (await reader.ReadAsync()).Should().BeFalse();
    }
    [Fact]
    public async Task Execute_Should_ReturnSara_When_WhereAgeIsBiggerThan30()
    {
        //Arrange
        var query = new Query().Select("ID", "FirstName").From("Student").Where("Age", ">", 30);
        var compiler = new Compiler(PostgresParameterFixer.Instance,
        new PostgresClauseCompilerFactory(),
        new SqlValidator());
        var executator = new QueryExecutor(SqlClientFactory.Instance,
        compiler,
        _sqlServerFixture.MsContainer.GetConnectionString());

        //Act
        await using var reader = await executator.ExecuteQuery(query);

        //Assert
        (await reader.ReadAsync()).Should().BeTrue();

        reader.GetInt32(0).Should().Be(3);

        (await reader.ReadAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task Execute_Should_ReturnNoRows_When_NoStudentMatches()
    {
        //Arrage
        var query = new Query().Select("ID", "FirstName").From("Student").Where("Age", ">", 100);
        var compiler = new Compiler(PostgresParameterFixer.Instance,
            new PostgresClauseCompilerFactory(),
            new SqlValidator());
        var executator = new QueryExecutor(SqlClientFactory.Instance,
        compiler,
        _sqlServerFixture.MsContainer.GetConnectionString());

        //Act
        await using var reader = await executator.ExecuteQuery(query);

        //Assert
        (await reader.ReadAsync()).Should().BeFalse();
    }

}
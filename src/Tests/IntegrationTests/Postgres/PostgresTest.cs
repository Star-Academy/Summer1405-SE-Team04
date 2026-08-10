using AwesomeAssertions;
using Npgsql;
using QueryBuilder.Compilers;
using QueryBuilder.Factory;
using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;
using QueryBuilder.Utils;
using SqlApp.QueryExecutor;

namespace IntegrationTests.Postgres;

public class PostgresIntegrationTest : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture _postgresFixture;

    private readonly QueryExecutor _sut;
    public PostgresIntegrationTest(PostgresFixture postgresFixture)
    {
        _postgresFixture = postgresFixture;

        var compiler = new Compiler(PostgresParameterFixer.Instance,
        new PostgresClauseCompilerFactory(),
        new SqlValidator());
        _sut = new QueryExecutor(NpgsqlFactory.Instance,
        compiler,
        _postgresFixture.PostgresContainer.GetConnectionString());
    }


    [Fact]
    public async Task Execute_ShouldReturnAll_WhenSelectAllRows()
    {
        //Arrange
        var query = new Query().Select("ID", "FirstName", "IsMale", "Age").From("Student");


        //Act
        await using var reader = await _sut.ExecuteQuery(query);

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
    public async Task Execute_ShouldReturnAllColumns_WhenWhereNameIsAli()
    {
        //Arrange
        var query = new Query().Select("ID", "FirstName", "IsMale", "Age").From("Student").Where("FirstName", "Ali");

        //Act
        await using var reader = await _sut.ExecuteQuery(query);

        //Assert
        (await reader.ReadAsync()).Should().BeTrue();

        reader.GetInt32(0).Should().Be(1);
        reader.GetString(1).Should().Be("Ali");
        reader.GetBoolean(2).Should().Be(true);
        reader.GetInt32(3).Should().Be(19);

        (await reader.ReadAsync()).Should().BeFalse();
    }
    [Fact]
    public async Task Execute_ShouldReturnSara_WhenWhereAgeIsBiggerThan30()
    {
        //Arrange
        var query = new Query().Select("ID", "FirstName").From("Student").Where("Age", ">", 30);

        //Act
        await using var reader = await _sut.ExecuteQuery(query);

        //Assert
        (await reader.ReadAsync()).Should().BeTrue();

        reader.GetInt32(0).Should().Be(3);

        (await reader.ReadAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task Execute_ShouldReturnNoRows_WhenNoStudentMatches()
    {
        //Arrage
        var query = new Query().Select("ID", "FirstName").From("Student").Where("Age", ">", 100);

        //Act
        await using var reader = await _sut.ExecuteQuery(query);

        //Assert
        (await reader.ReadAsync()).Should().BeFalse();
    }
}
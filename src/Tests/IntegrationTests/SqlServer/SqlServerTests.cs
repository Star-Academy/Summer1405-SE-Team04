using System.Data.Common;
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
    private readonly QueryExecutor _sut;

    public SqlServerTest(SqlServerFixture sqlServerFixture)
    {
        _sqlServerFixture = sqlServerFixture;

        var compiler = new Compiler(SqlServerParameterFixer.Instance,
        new SqlServerClauseCompilerFactory(),
        new SqlValidator());

        _sut = new QueryExecutor(SqlClientFactory.Instance,
        compiler,
        _sqlServerFixture.MsContainer.GetConnectionString());
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

    [Fact]
    public async Task Execute_ShouldPropagateException_WhenTableIsNotExist()
    {
        // Arrange
        var query = new Query().Select("FirstName").From("NoTable");

        // Act
        Func<Task> act = () => _sut.ExecuteQuery(query);

        // Assert
        await act.Should().ThrowAsync<DbException>();
    }
}
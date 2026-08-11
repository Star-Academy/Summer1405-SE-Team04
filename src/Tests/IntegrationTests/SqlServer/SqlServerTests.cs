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

    private static Student _getStudentFromReader(DbDataReader reader)
    {
        var id = reader.GetInt32(0);
        var firstName = reader.GetString(1);
        var isMale = reader.GetBoolean(2);
        var age = reader.GetInt32(3);
        return new Student(id, firstName, isMale, age);
    }
    
    [Fact]
    public async Task Execute_ShouldReturnAll_WhenSelectAllRows()
    {
        // Arrange
        var query = new Query().Select("ID", "FirstName", "IsMale", "Age").From("Student");


        //Act
        await using var reader = await _sut.ExecuteQuery(query);

        //Assert
        foreach (var expectedStudent in TestData.Students)
        {
            (await reader.ReadAsync()).Should().BeTrue();
            
            var actualStudent = _getStudentFromReader(reader);
            actualStudent.Should().BeEquivalentTo(expectedStudent);
            
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
        (await reader.ReadAsync(TestContext.Current.CancellationToken)).Should().BeTrue();

        var actualStudent = _getStudentFromReader(reader);
        var expectedStudent = TestData.Students[0];
        
        actualStudent.Should().BeEquivalentTo(expectedStudent);
        

        (await reader.ReadAsync(TestContext.Current.CancellationToken)).Should().BeFalse();
    }
    [Fact]
    public async Task Execute_ShouldReturnSara_WhenWhereAgeIsBiggerThan30()
    {
        //Arrange
        var query = new Query().Select("ID", "FirstName").From("Student").Where("Age", ">", 30);

        //Act
        await using var reader = await _sut.ExecuteQuery(query);

        //Assert
        (await reader.ReadAsync(TestContext.Current.CancellationToken)).Should().BeTrue();

        reader.GetInt32(0).Should().Be(3);

        (await reader.ReadAsync(TestContext.Current.CancellationToken)).Should().BeFalse();
    }

    [Fact]
    public async Task Execute_ShouldReturnNoRows_WhenNoStudentMatches()
    {
        //Arrage
        var query = new Query().Select("ID", "FirstName").From("Student").Where("Age", ">", 100);

        //Act
        await using var reader = await _sut.ExecuteQuery(query);

        //Assert
        (await reader.ReadAsync(TestContext.Current.CancellationToken)).Should().BeFalse();
    }

    [Fact]
    public async Task Execute_ShouldPropagateException_WhenTableDoesNotExist()
    {
        // Arrange
        var query = new Query().Select("FirstName").From("NoTable");

        // Act
        Func<Task> act = () => _sut.ExecuteQuery(query);

        // Assert
        await act.Should().ThrowAsync<DbException>();
    }
}
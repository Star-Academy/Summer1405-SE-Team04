using System.Data.Common;
using AwesomeAssertions;
using Npgsql;
using Npgsql.Replication.PgOutput.Messages;
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

    private static Student _getStudentFromReader(DbDataReader reader)
    {
        var id = reader.GetInt32(0);
        var firstName = reader.GetString(1);
        var isMale = reader.GetBoolean(2);
        var age = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3);
        return new Student(id, firstName, isMale, age);
    }



    [Fact]
    public async Task Execute_ShouldReturnAll_WhenSelectAllRows()
    {
        //Arrange
        var query = new Query().Select("ID", "FirstName", "IsMale", "Age").From("Student");

        //Act
        await using var reader = await _sut.ExecuteQuery(query);

        //Assert

        foreach (var expectedStudent in TestData.Students)
        {
            (await reader.ReadAsync(TestContext.Current.CancellationToken)).Should().BeTrue();

            var actualStudent = _getStudentFromReader(reader);
            actualStudent.Should().BeEquivalentTo(expectedStudent);
        }

        (await reader.ReadAsync(TestContext.Current.CancellationToken)).Should().BeFalse();
    }

    [Fact]
    public async Task Execute_ShouldReturnCorrectUser_WhenWhereNameGiven()
    {
        //Arrange
        var query = new Query().Select("ID", "FirstName", "IsMale", "Age").From("Student").Where("FirstName", "Ali");

        //Act
        await using var reader = await _sut.ExecuteQuery(query);

        //Assert
        (await reader.ReadAsync(TestContext.Current.CancellationToken)).Should().BeTrue();

        var expectedStudent = TestData.Students[0];
        var actualStudent = _getStudentFromReader(reader);

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
        //Arrange
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



    [Fact]
    public async Task Execute_ShouldReturnCorrectStudents_WhenMultipleWhereConditionsGiven()
    {
        //Arrange
        var query = new Query()
            .Select("ID", "FirstName", "IsMale", "Age")
            .From("Student")
            .Where("IsMale", true)
            .Where("Age", ">", 15);

        //Act
        await using var reader = await _sut.ExecuteQuery(query);

        //Assert
        foreach (var expectedStudent in TestData.Students.Where(s => s.IsMale && s.Age > 15))
        {
            (await reader.ReadAsync(TestContext.Current.CancellationToken)).Should().BeTrue();

            var actualStudent = _getStudentFromReader(reader);

            actualStudent.Should().BeEquivalentTo(expectedStudent);
        }

        (await reader.ReadAsync(TestContext.Current.CancellationToken)).Should().BeFalse();
    }


    [Fact]
    public async Task Execute_ShouldThrowArgumentNullException_WhenFromIsNotGiven()
    {
        //Arrange
        var query = new Query()
            .Select("ID", "FirstName");

        //Act
        Func<Task> act = () => _sut.ExecuteQuery(query);

        //Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Execute_ShouldDoesntReturnFarhad_WhenWhereOnAge()
    {
        //Arrange
        var query = new Query()
            .Select("ID", "FirstName", "IsMale", "Age")
            .From("Student")
            .Where("Age", ">", 15);

        //Act
        await using var reader = await _sut.ExecuteQuery(query);

        //Assert
        foreach (var expectedStudent in TestData.Students.Where(s => s.Age > 15))
        {
            (await reader.ReadAsync(TestContext.Current.CancellationToken)).Should().BeTrue();

            var actualStudent = _getStudentFromReader(reader);

            actualStudent.Should().BeEquivalentTo(expectedStudent);
        }

        (await reader.ReadAsync(TestContext.Current.CancellationToken)).Should().BeFalse();
    }
    [Fact]
    public async Task Execute_ShouldThrowArgumentException_WhenFromIsEmpty()
    {
        //Arrange
        var query = new Query()
            .Select("ID", "FirstName");

        //Act
        var act = () => query.From("");

        //Assert
        act.Should().Throw<ArgumentException>();
    }


    [Fact]
    public async Task Execute_ShouldReturnStudentsYoungerThan20_WhenWhereAgeIsSmallerThan20()
    {
        //Arrange
        var query = new Query()
            .Select("ID", "FirstName", "IsMale", "Age")
            .From("Student")
            .Where("Age", "<", 20);

        //Act
        await using var reader = await _sut.ExecuteQuery(query);

        //Assert
        foreach (var expectedStudent in TestData.Students.Where(s => s.Age < 20))
        {
            (await reader.ReadAsync(TestContext.Current.CancellationToken)).Should().BeTrue();

            var actualStudent = _getStudentFromReader(reader);

            actualStudent.Should().BeEquivalentTo(expectedStudent);
        }

        (await reader.ReadAsync(TestContext.Current.CancellationToken)).Should().BeFalse();
    }


    [Fact]
    public async Task Execute_ShouldReturnStudents_WhenWhereFirstNameIsLike()
    {
        //Arrange
        var query = new Query()
            .Select("ID", "FirstName", "IsMale", "Age")
            .From("Student")
            .Where("FirstName", "LIKE", "Amir%");

        //Act
        await using var reader = await _sut.ExecuteQuery(query);

        //Assert
        while (await reader.ReadAsync(TestContext.Current.CancellationToken))
        {
            var actualStudent = _getStudentFromReader(reader);

            actualStudent.FirstName.Should().Contain("Amirali");

        }
    }


    [Fact]
    public async Task Execute_ShouldThrowDbException_WhenInvalidWhereOperatorIsGiven()
    {
        //Arrange
        var query = new Query()
            .Select("ID", "FirstName")
            .From("Student")
            .Where("Age", "^", 20);

        //Act
        Func<Task> act = () => _sut.ExecuteQuery(query);

        //Assert
        await act.Should().ThrowAsync<DbException>();
    }
}
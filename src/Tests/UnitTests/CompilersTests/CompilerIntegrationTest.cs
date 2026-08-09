using QueryBuilder.Compilers;
using QueryBuilder.Factory;
using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;
using QueryBuilder.Utils;

namespace UnitTests;

public class CompilerIntegrationTest
{
    [Fact]
    public void Compile_Should_ProduceDoubleQuotedSql_When_CompilingFullQueryWithPostgresFactory()
    {
        // Arrange
        var sut = new Compiler(PostgresParameterFixer.Instance, new PostgresClauseCompilerFactory(), new SqlValidator());
        var query = new Query().Select("FirstName", "Age").From("Student").Where("Age", ">", 10).Where("IsMale", true);

        // Act
        var (sqlQuery, _) = sut.Compile(query);

        // Assert
        sqlQuery.Should().Be("SELECT \"FirstName\", \"Age\" FROM \"Student\" WHERE \"Age\" > @p0 AND \"IsMale\" = @p1");
    }

    [Fact]
    public void Compile_Should_ProduceBracketedSql_When_CompilingFullQueryWithSqlServerFactory()
    {
        // Arrange
        var sut = new Compiler(SqlServerParameterFixer.Instance, new SqlServerClauseCompilerFactory(), new SqlValidator());
        var query = new Query().Select("FirstName", "Age").From("Student").Where("Age", ">", 10).Where("IsMale", true);

        // Act
        var (sqlQuery, _) = sut.Compile(query);

        // Assert
        sqlQuery.Should().Be("SELECT [FirstName], [Age] FROM [Student] WHERE [Age] > @p0 AND [IsMale] = @p1");
    }

    [Fact]
    public void Compile_Should_ProduceBindingsMatchingSqlPlaceholders_When_CompilingFullQueryWithPostgresFactory()
    {
        // Arrange
        var sut = new Compiler(PostgresParameterFixer.Instance, new PostgresClauseCompilerFactory(), new SqlValidator());
        var query = new Query().Select("FirstName", "Age").From("Student").Where("Age", ">", 10).Where("IsMale", true);

        // Act
        var (sqlQuery, bindings) = sut.Compile(query);

        // Assert
        bindings.Select(b => b.parameterName).Should().Equal("@p0", "@p1");
        foreach (var binding in bindings)
            sqlQuery.Should().Contain(binding.parameterName);
    }
}

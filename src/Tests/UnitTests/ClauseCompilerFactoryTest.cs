using QueryBuilder.Compilers;
using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.Factory;
using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;
using QueryBuilder.Utils;

namespace UnitTests;

public class ClauseCompilerFactoryTest
{
    [Fact]
    public void PostgresClauseCompilerFactory_CreateClauses_ProducesSelectFromWhereInOrder()
    {
        var clauses = new PostgresClauseCompilerFactory().CreateClauses().ToList();

        clauses.Should().HaveCount(3);
        clauses[0].Should().BeOfType<SelectClauseCompiler>();
        clauses[1].Should().BeOfType<FromClauseCompiler>();
        clauses[2].Should().BeOfType<WhereClauseCompiler>();
    }

    [Fact]
    public void SqlServerClauseCompilerFactory_CreateClauses_ProducesSelectFromWhereInOrder()
    {
        var clauses = new SqlServerClauseCompilerFactory().CreateClauses().ToList();

        clauses.Should().HaveCount(3);
        clauses[0].Should().BeOfType<SelectClauseCompiler>();
        clauses[1].Should().BeOfType<FromClauseCompiler>();
        clauses[2].Should().BeOfType<WhereClauseCompiler>();
    }

    [Fact]
    public void PostgresClauseCompilerFactory_CreateClausesTwice_ProducesFreshInstances()
    {
        var sut = new PostgresClauseCompilerFactory();

        var first = sut.CreateClauses().ToList();
        var second = sut.CreateClauses().ToList();

        first.Should().NotBeSameAs(second);
        first[0].Should().NotBeSameAs(second[0]);
    }

    [Fact]
    public void SqlServerClauseCompilerFactory_CreateClausesTwice_ProducesFreshInstances()
    {
        var sut = new SqlServerClauseCompilerFactory();

        var first = sut.CreateClauses().ToList();
        var second = sut.CreateClauses().ToList();

        first.Should().NotBeSameAs(second);
        first[0].Should().NotBeSameAs(second[0]);
    }

    [Fact]
    public void PostgresClauseCompilerFactory_CompileFullQuery_ProducesDoubleQuotedSql()
    {
        var sut = new Compiler(PostgresParameterFixer.Instance, new PostgresClauseCompilerFactory(), new SqlValidator());
        var query = new Query().Select("FirstName", "Age").From("Student").Where("Age", ">", 10).Where("IsMale", true);

        var (sqlQuery, _) = sut.Compile(query);

        sqlQuery.Should().Be("SELECT \"FirstName\", \"Age\" FROM \"Student\" WHERE \"Age\" > @p0 AND \"IsMale\" = @p1");
    }

    [Fact]
    public void SqlServerClauseCompilerFactory_CompileFullQuery_ProducesBracketedSql()
    {
        var sut = new Compiler(SqlServerParameterFixer.Instance, new SqlServerClauseCompilerFactory(), new SqlValidator());
        var query = new Query().Select("FirstName", "Age").From("Student").Where("Age", ">", 10).Where("IsMale", true);

        var (sqlQuery, _) = sut.Compile(query);

        sqlQuery.Should().Be("SELECT [FirstName], [Age] FROM [Student] WHERE [Age] > @p0 AND [IsMale] = @p1");
    }

    [Fact]
    public void PostgresClauseCompilerFactory_CompileFullQuery_ProducesBindingsMatchingSqlPlaceholders()
    {
        var sut = new Compiler(PostgresParameterFixer.Instance, new PostgresClauseCompilerFactory(), new SqlValidator());
        var query = new Query().Select("FirstName", "Age").From("Student").Where("Age", ">", 10).Where("IsMale", true);

        var (sqlQuery, bindings) = sut.Compile(query);

        bindings.Select(b => b.parameterName).Should().Equal("@p0", "@p1");
        foreach (var binding in bindings)
            sqlQuery.Should().Contain(binding.parameterName);
    }
}

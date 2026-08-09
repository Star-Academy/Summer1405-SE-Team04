using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.Factory;

namespace UnitTests;

public class ClauseCompilerFactoryTest
{
    [Fact]
    public void CreateClauses_Should_ProduceSelectFromWhereInOrder_When_UsingPostgresFactory()
    {
        // Act
        var clauses = new PostgresClauseCompilerFactory().CreateClauses().ToList();

        // Assert
        clauses.Should().HaveCount(3);
        clauses[0].Should().BeOfType<SelectClauseCompiler>();
        clauses[1].Should().BeOfType<FromClauseCompiler>();
        clauses[2].Should().BeOfType<WhereClauseCompiler>();
    }

    [Fact]
    public void CreateClauses_Should_ProduceSelectFromWhereInOrder_When_UsingSqlServerFactory()
    {
        // Act
        var clauses = new SqlServerClauseCompilerFactory().CreateClauses().ToList();

        // Assert
        clauses.Should().HaveCount(3);
        clauses[0].Should().BeOfType<SelectClauseCompiler>();
        clauses[1].Should().BeOfType<FromClauseCompiler>();
        clauses[2].Should().BeOfType<WhereClauseCompiler>();
    }

    [Fact]
    public void CreateClauses_Should_ProduceFreshInstances_When_PostgresFactoryCreateClausesIsCalledTwice()
    {
        // Arrange
        var sut = new PostgresClauseCompilerFactory();

        // Act
        var first = sut.CreateClauses().ToList();
        var second = sut.CreateClauses().ToList();

        // Assert
        first.Should().NotBeSameAs(second);
        first[0].Should().NotBeSameAs(second[0]);
    }

    [Fact]
    public void CreateClauses_Should_ProduceFreshInstances_When_SqlServerFactoryCreateClausesIsCalledTwice()
    {
        // Arrange
        var sut = new SqlServerClauseCompilerFactory();

        // Act
        var first = sut.CreateClauses().ToList();
        var second = sut.CreateClauses().ToList();

        // Assert
        first.Should().NotBeSameAs(second);
        first[0].Should().NotBeSameAs(second[0]);
    }
}

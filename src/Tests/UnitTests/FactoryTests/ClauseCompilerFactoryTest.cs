using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.Factory;

namespace UnitTests.FactoryTests;

public class ClauseCompilerFactoryTest
{
    [Fact]
    public void CreateClauses_ShouldProduceSelectFromWhereInOrder_WhenUsingPostgresFactory()
    {
        // Act
        var sut = new PostgresClauseCompilerFactory().CreateClauses().ToList();

        // Assert
        sut.Should().HaveCount(3);
        sut[0].Should().BeOfType<SelectClauseCompiler>();
        sut[1].Should().BeOfType<FromClauseCompiler>();
        sut[2].Should().BeOfType<WhereClauseCompiler>();
    }

    [Fact]
    public void CreateClauses_ShouldProduceSelectFromWhereInOrder_WhenUsingSqlServerFactory()
    {
        // Act
        var sut = new SqlServerClauseCompilerFactory().CreateClauses().ToList();

        // Assert
        sut.Should().HaveCount(3);
        sut[0].Should().BeOfType<SelectClauseCompiler>();
        sut[1].Should().BeOfType<FromClauseCompiler>();
        sut[2].Should().BeOfType<WhereClauseCompiler>();
    }

    [Fact]
    public void CreateClauses_ShouldProduceFreshInstances_WhenPostgresFactoryCreateClausesIsCalledTwice()
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
    public void CreateClauses_ShouldProduceFreshInstances_WhenSqlServerFactoryCreateClausesIsCalledTwice()
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
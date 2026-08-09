using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.Factory;

namespace UnitTests.FactoryTests;

public class PostgresClauseCompilerFactoryTest
{
    private readonly PostgresClauseCompilerFactory _sut;

    public PostgresClauseCompilerFactoryTest()
    {
        _sut = new PostgresClauseCompilerFactory();
    }

    [Fact]
    public void CreateClauses_ShouldProduceSelectFromWhereInOrder_WhenUsingPostgresFactory()
    {
        // Act
        var result = _sut.CreateClauses().ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].Should().BeOfType<SelectClauseCompiler>();
        result[1].Should().BeOfType<FromClauseCompiler>();
        result[2].Should().BeOfType<WhereClauseCompiler>();
    }

    [Fact]
    public void CreateClauses_ShouldProduceFreshInstances_WhenCreateClausesIsCalledTwice()
    {
        // Act
        var first = _sut.CreateClauses().ToList();
        var second = _sut.CreateClauses().ToList();

        // Assert
        first.Should().NotBeSameAs(second);
        first[0].Should().NotBeSameAs(second[0]);
    }
}

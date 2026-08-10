using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.Factory;

namespace UnitTests.Factory;

public class SqlServerClauseCompilerFactoryTest
{
    private readonly SqlServerClauseCompilerFactory _sut;

    public SqlServerClauseCompilerFactoryTest()
    {
        _sut = new SqlServerClauseCompilerFactory();
    }

    [Fact]
    public void CreateClauses_ShouldProduceSelectFromWhereInOrder_WhenUsingSqlServerFactory()
    {
        // Arrange

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
        // Arrange

        // Act
        var first = _sut.CreateClauses().ToList();
        var second = _sut.CreateClauses().ToList();

        // Assert
        first.Should().NotBeSameAs(second);
        first[0].Should().NotBeSameAs(second[0]);
    }
}

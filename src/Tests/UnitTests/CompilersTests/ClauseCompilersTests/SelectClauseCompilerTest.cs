using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;

namespace UnitTests;

public class SelectClauseCompilerTest
{
    private readonly IParameterFixer _parameterFixerMock;
    private readonly SelectClauseCompiler _sut;

    public SelectClauseCompilerTest()
    {
        _parameterFixerMock = Substitute.For<IParameterFixer>();
        _parameterFixerMock.WrapIdentifier(Arg.Any<string>()).Returns(x => x.Arg<string>());

        _sut = new SelectClauseCompiler(_parameterFixerMock);
    }

    [Fact]
    public void Compile_Should_ProduceSelectWithoutLeadingSpace_When_SelectingSingleColumn()
    {
        // Arrange
        var query = new Query().Select("A");

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Should().Be("SELECT A");
    }

    [Fact]
    public void Compile_Should_JoinColumnsWithCommaSpace_When_SelectingMultipleColumns()
    {
        // Arrange
        var query = new Query().Select("A", "B", "C");

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Should().Be("SELECT A, B, C");
    }

    [Fact]
    public void Compile_Should_ProduceSelectWithTrailingSpace_When_NoColumnsAreSelected()
    {
        // Arrange
        var query = new Query();

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Should().Be("SELECT ");
    }

    [Fact]
    public void Compile_Should_WrapEveryColumnIdentifier_When_Compiling()
    {
        // Arrange
        var query = new Query().Select("A", "B");

        // Act
        _sut.Compile(query);

        // Assert
        _parameterFixerMock.Received(1).WrapIdentifier("A");
        _parameterFixerMock.Received(1).WrapIdentifier("B");
    }

    [Fact]
    public void Compile_Should_EmitBothColumns_When_ColumnsAreDuplicated()
    {
        // Arrange
        var query = new Query().Select("A", "A");

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Should().Be("SELECT A, A");
    }
}

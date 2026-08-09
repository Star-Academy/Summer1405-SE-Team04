using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;

namespace UnitTests;

public class WhereClauseCompilerTest
{
    private readonly IParameterFixer _parameterFixerMock;
    private readonly WhereClauseCompiler _sut;

    public WhereClauseCompilerTest()
    {
        _parameterFixerMock = Substitute.For<IParameterFixer>();
        _parameterFixerMock.WrapIdentifier(Arg.Any<string>()).Returns(x => x.Arg<string>());
        _parameterFixerMock.FormatParameter(Arg.Any<int>()).Returns(x => x.Arg<int>().ToString());

        _sut = new WhereClauseCompiler(_parameterFixerMock);
    }

    [Fact]
    public void Compile_Should_ProduceEmptyString_When_NoWhereEntriesExist()
    {
        // Arrange
        var query = new Query();

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void Compile_Should_ProduceWhereWithLeadingSpace_When_SingleEntryExists()
    {
        // Arrange
        var query = new Query().Where("Age", 10);

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Should().Be("WHERE Age = 0");
    }

    [Fact]
    public void Compile_Should_JoinEntriesWithAnd_When_MultipleEntriesExist()
    {
        // Arrange
        var query = new Query().Where("Age", 10).Where("IsMale", true);

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Should().Be("WHERE Age = 0 AND IsMale = 1");
    }

    [Theory]
    [InlineData(">")]
    [InlineData("<=")]
    [InlineData("LIKE")]
    [InlineData("!=")]
    [InlineData("IS NOT")]
    public void Compile_Should_EmitOperatorVerbatim_When_CustomOperatorIsUsed(string op)
    {
        // Arrange
        var query = new Query().Where("Age", op, 10);

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Should().Be($"WHERE Age {op} 0");
    }

    [Fact]
    public void Compile_Should_FormatParameterPerEntryIndex_When_Compiling()
    {
        // Arrange
        var query = new Query().Where("Age", 10).Where("IsMale", true);

        // Act
        _sut.Compile(query);

        // Assert
        _parameterFixerMock.Received(1).FormatParameter(0);
        _parameterFixerMock.Received(1).FormatParameter(1);
    }

    [Fact]
    public void Compile_Should_ProduceDoubleSpace_When_OperatorIsNull()
    {
        // Arrange
        var query = new Query().Where("Age", null!, 10);

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Should().Be("WHERE Age  0");
    }

    [Fact]
    public void Compile_Should_NotTouchParameterFixer_When_NoWhereEntriesExist()
    {
        // Arrange
        var query = new Query();

        // Act
        _sut.Compile(query);

        // Assert
        _parameterFixerMock.DidNotReceive().FormatParameter(Arg.Any<int>());
    }
}

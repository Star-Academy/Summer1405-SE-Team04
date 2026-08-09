using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;

namespace UnitTests;

public class FromClauseCompilerTest
{
    private readonly IParameterFixer _parameterFixerMock;
    private readonly FromClauseCompiler _sut;

    public FromClauseCompilerTest()
    {
        _parameterFixerMock = Substitute.For<IParameterFixer>();
        _parameterFixerMock.WrapIdentifier(Arg.Any<string>()).Returns(x => x.Arg<string>());

        _sut = new FromClauseCompiler(_parameterFixerMock);
    }

    [Fact]
    public void Compile_Should_ProduceFromWithLeadingSpace_When_TableIsSpecified()
    {
        // Arrange
        var query = new Query().From("Student");

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Should().Be(" FROM Student");
    }

    [Fact]
    public void Compile_Should_ProduceFromWithEmptyIdentifier_When_NoTableIsSpecified()
    {
        // Arrange
        var query = new Query();

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Should().Be(" FROM ");
    }

    [Fact]
    public void Compile_Should_WrapTableIdentifier_When_Compiling()
    {
        // Arrange
        var query = new Query().From("Student");

        // Act
        _sut.Compile(query);

        // Assert
        _parameterFixerMock.Received(1).WrapIdentifier("Student");
    }
}

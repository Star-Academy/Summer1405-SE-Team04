using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;

namespace UnitTests.Compilers.ClauseCompilers;

public class FromClauseCompilerTest
{
    private readonly IParameterFixer _parameterFixer;
    private readonly FromClauseCompiler _sut;

    public FromClauseCompilerTest()
    {
        _parameterFixer = Substitute.For<IParameterFixer>();
        _parameterFixer.WrapIdentifier(Arg.Any<string>()).Returns(x => x.Arg<string>());

        _sut = new FromClauseCompiler(_parameterFixer);
    }

    [Fact]
    public void Compile_ShouldProduceFromWithLeadingSpace_WhenTableIsSpecified()
    {
        // Arrange
        var query = new Query().From("Student");

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Should().Be("FROM Student");
    }

    [Fact]
    public void Compile_ShouldWrapTableIdentifier_WhenCompiling()
    {
        // Arrange
        var query = new Query().From("Student");

        // Act
        _sut.Compile(query);

        // Assert
        _parameterFixer.Received(1).WrapIdentifier("Student");
    }
}
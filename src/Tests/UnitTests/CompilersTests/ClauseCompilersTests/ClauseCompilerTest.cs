using QueryBuilder.Compilers.ClauseCompilers;
using QueryBuilder.Models;
using QueryBuilder.ParameterFixer;
using NSubstitute;

namespace UnitTests;

public class ClauseCompilerTest
{
    private readonly IParameterFixer _parameterFixerMock;
    private readonly SelectClauseCompiler _selectSut;
    private readonly FromClauseCompiler _fromSut;
    private readonly WhereClauseCompiler _whereSut;

    public ClauseCompilerTest()
    {
        _parameterFixerMock = Substitute.For<IParameterFixer>();
        _parameterFixerMock.WrapIdentifier(Arg.Any<string>()).Returns(x => x.Arg<string>());
        _parameterFixerMock.FormatParameter(Arg.Any<int>()).Returns(x => x.Arg<int>().ToString());

        _selectSut = new SelectClauseCompiler(_parameterFixerMock);
        _fromSut = new FromClauseCompiler(_parameterFixerMock);
        _whereSut = new WhereClauseCompiler(_parameterFixerMock);
    }

    [Fact]
    public void SelectClauseCompiler_SingleColumn_ProducesSelectWithoutLeadingSpace()
    {
        // Arrange
        var query = new Query().Select("A");

        // Act
        var result = _selectSut.Compile(query);

        // Assert
        result.Should().Be("SELECT A");
    }

    [Fact]
    public void SelectClauseCompiler_MultiColumn_JoinsWithCommaSpace()
    {
        // Arrange
        var query = new Query().Select("A", "B", "C");

        // Act
        var result = _selectSut.Compile(query);

        // Assert
        result.Should().Be("SELECT A, B, C");
    }

    [Fact]
    public void SelectClauseCompiler_NoColumns_ProducesSelectWithTrailingSpace()
    {
        // Arrange
        var query = new Query();

        // Act
        var result = _selectSut.Compile(query);

        // Assert
        result.Should().Be("SELECT ");
    }

    [Fact]
    public void SelectClauseCompiler_Compile_WrapsEveryColumnIdentifier()
    {
        // Arrange
        var query = new Query().Select("A", "B");

        // Act
        _selectSut.Compile(query);

        // Assert
        _parameterFixerMock.Received(1).WrapIdentifier("A");
        _parameterFixerMock.Received(1).WrapIdentifier("B");
    }

    [Fact]
    public void SelectClauseCompiler_DuplicateColumns_EmitsBoth()
    {
        // Arrange
        var query = new Query().Select("A", "A");

        // Act
        var result = _selectSut.Compile(query);

        // Assert
        result.Should().Be("SELECT A, A");
    }

    [Fact]
    public void FromClauseCompiler_Table_ProducesFromWithLeadingSpace()
    {
        // Arrange
        var query = new Query().From("Student");

        // Act
        var result = _fromSut.Compile(query);

        // Assert
        result.Should().Be(" FROM Student");
    }

    [Fact]
    public void FromClauseCompiler_NoTable_ProducesFromWithEmptyIdentifier()
    {
        // Arrange
        var query = new Query();

        // Act
        var result = _fromSut.Compile(query);

        // Assert
        result.Should().Be(" FROM ");
    }

    [Fact]
    public void FromClauseCompiler_Compile_WrapsTableIdentifier()
    {
        // Arrange
        var query = new Query().From("Student");

        // Act
        _fromSut.Compile(query);

        // Assert
        _parameterFixerMock.Received(1).WrapIdentifier("Student");
    }

    [Fact]
    public void WhereClauseCompiler_NoEntries_ProducesEmptyString()
    {
        // Arrange
        var query = new Query();

        // Act
        var result = _whereSut.Compile(query);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void WhereClauseCompiler_SingleEntry_ProducesWhereWithLeadingSpace()
    {
        // Arrange
        var query = new Query().Where("Age", 10);

        // Act
        var result = _whereSut.Compile(query);

        // Assert
        result.Should().Be(" WHERE Age = 0");
    }

    [Fact]
    public void WhereClauseCompiler_MultiEntry_JoinsWithAnd()
    {
        // Arrange
        var query = new Query().Where("Age", 10).Where("IsMale", true);

        // Act
        var result = _whereSut.Compile(query);

        // Assert
        result.Should().Be(" WHERE Age = 0 AND IsMale = 1");
    }

    [Theory]
    [InlineData(">")]
    [InlineData("<=")]
    [InlineData("LIKE")]
    [InlineData("!=")]
    [InlineData("IS NOT")]
    public void WhereClauseCompiler_CustomOperator_EmitsOperatorVerbatim(string op)
    {
        // Arrange
        var query = new Query().Where("Age", op, 10);

        // Act
        var result = _whereSut.Compile(query);

        // Assert
        result.Should().Be($" WHERE Age {op} 0");
    }

    [Fact]
    public void WhereClauseCompiler_Compile_FormatsParameterPerEntryIndex()
    {
        // Arrange
        var query = new Query().Where("Age", 10).Where("IsMale", true);

        // Act
        _whereSut.Compile(query);

        // Assert
        _parameterFixerMock.Received(1).FormatParameter(0);
        _parameterFixerMock.Received(1).FormatParameter(1);
    }

    [Fact]
    public void WhereClauseCompiler_NullOperator_ProducesDoubleSpace()
    {
        // Arrange
        var query = new Query().Where("Age", null!, 10);

        // Act
        var result = _whereSut.Compile(query);

        // Assert
        result.Should().Be(" WHERE Age  0");
    }

    [Fact]
    public void WhereClauseCompiler_NoEntries_DoesNotTouchParameterFixer()
    {
        // Arrange
        var query = new Query();

        // Act
        _whereSut.Compile(query);

        // Assert
        _parameterFixerMock.DidNotReceive().FormatParameter(Arg.Any<int>());
    }
}

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
    public void Compile_Should_ProduceSelectWithoutLeadingSpace_When_SelectingSingleColumn()
    {
        // Arrange
        var query = new Query().Select("A");

        // Act
        var result = _selectSut.Compile(query);

        // Assert
        result.Should().Be("SELECT A");
    }

    [Fact]
    public void Compile_Should_JoinColumnsWithCommaSpace_When_SelectingMultipleColumns()
    {
        // Arrange
        var query = new Query().Select("A", "B", "C");

        // Act
        var result = _selectSut.Compile(query);

        // Assert
        result.Should().Be("SELECT A, B, C");
    }

    [Fact]
    public void Compile_Should_ProduceSelectWithTrailingSpace_When_NoColumnsAreSelected()
    {
        // Arrange
        var query = new Query();

        // Act
        var result = _selectSut.Compile(query);

        // Assert
        result.Should().Be("SELECT ");
    }

    [Fact]
    public void Compile_Should_WrapEveryColumnIdentifier_When_Compiling()
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
    public void Compile_Should_EmitBothColumns_When_ColumnsAreDuplicated()
    {
        // Arrange
        var query = new Query().Select("A", "A");

        // Act
        var result = _selectSut.Compile(query);

        // Assert
        result.Should().Be("SELECT A, A");
    }

    [Fact]
    public void Compile_Should_ProduceFromWithLeadingSpace_When_TableIsSpecified()
    {
        // Arrange
        var query = new Query().From("Student");

        // Act
        var result = _fromSut.Compile(query);

        // Assert
        result.Should().Be(" FROM Student");
    }

    [Fact]
    public void Compile_Should_ProduceFromWithEmptyIdentifier_When_NoTableIsSpecified()
    {
        // Arrange
        var query = new Query();

        // Act
        var result = _fromSut.Compile(query);

        // Assert
        result.Should().Be(" FROM ");
    }

    [Fact]
    public void Compile_Should_WrapTableIdentifier_When_Compiling()
    {
        // Arrange
        var query = new Query().From("Student");

        // Act
        _fromSut.Compile(query);

        // Assert
        _parameterFixerMock.Received(1).WrapIdentifier("Student");
    }

    [Fact]
    public void Compile_Should_ProduceEmptyString_When_NoWhereEntriesExist()
    {
        // Arrange
        var query = new Query();

        // Act
        var result = _whereSut.Compile(query);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void Compile_Should_ProduceWhereWithLeadingSpace_When_SingleEntryExists()
    {
        // Arrange
        var query = new Query().Where("Age", 10);

        // Act
        var result = _whereSut.Compile(query);

        // Assert
        result.Should().Be(" WHERE Age = 0");
    }

    [Fact]
    public void Compile_Should_JoinEntriesWithAnd_When_MultipleEntriesExist()
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
    public void Compile_Should_EmitOperatorVerbatim_When_CustomOperatorIsUsed(string op)
    {
        // Arrange
        var query = new Query().Where("Age", op, 10);

        // Act
        var result = _whereSut.Compile(query);

        // Assert
        result.Should().Be($" WHERE Age {op} 0");
    }

    [Fact]
    public void Compile_Should_FormatParameterPerEntryIndex_When_Compiling()
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
    public void Compile_Should_ProduceDoubleSpace_When_OperatorIsNull()
    {
        // Arrange
        var query = new Query().Where("Age", null!, 10);

        // Act
        var result = _whereSut.Compile(query);

        // Assert
        result.Should().Be(" WHERE Age  0");
    }

    [Fact]
    public void Compile_Should_NotTouchParameterFixer_When_NoWhereEntriesExist()
    {
        // Arrange
        var query = new Query();

        // Act
        _whereSut.Compile(query);

        // Assert
        _parameterFixerMock.DidNotReceive().FormatParameter(Arg.Any<int>());
    }
}

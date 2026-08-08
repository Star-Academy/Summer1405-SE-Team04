using QueryBuilder.Models;
using QueryBuilder.Utils;

namespace UnitTests;

public class SqlValidatorTest
{
    private readonly SqlValidator _sut;

    public SqlValidatorTest()
    {
        _sut = new SqlValidator();
    }

    [Fact]
    public void Should_ReturnTrue_When_ParamsContainItems()
    {
        // Act
        var result = _sut.ValidateParams([1, "a"]);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnFalse_When_ParamsArrayIsEmpty()
    {
        // Act
        var result = _sut.ValidateParams([]);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnTrue_When_ParamsContainNullElement()
    {
        // Act
        var result = _sut.ValidateParams([null!]);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("a")]
    [InlineData("a", "b")]
    [InlineData("a", "b", "c")]
    public void Should_ReturnTrue_When_AllStringsAreValidNames(params string[] inputs)
    {
        // Act
        var result = _sut.ValidateStringsNotEmpty(inputs);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnFalse_When_NoArgsAreProvided()
    {
        // Act
        var result = _sut.ValidateStringsNotEmpty();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("   ")]
    public void Should_ReturnFalse_When_ValueIsBlank(string input)
    {
        // Act
        var result = _sut.ValidateStringsNotEmpty(input);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnFalse_When_ElementIsNull()
    {
        // Act
        var result = _sut.ValidateStringsNotEmpty([null!]);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("a", "")]
    [InlineData("", "a")]
    [InlineData("a", "\t", "b")]
    public void Should_ReturnFalse_When_OneValueIsInvalidAmongValidOnes(params string[] inputs)
    {
        // Act
        var result = _sut.ValidateStringsNotEmpty(inputs);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Should_ThrowNullReferenceException_When_ArrayIsNull()
    {
        // Act
        var act = () => _sut.ValidateStringsNotEmpty(null!);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Should_ReturnTrue_When_QueryHasSelectAndFrom()
    {
        // Arrange
        var query = new Query().Select("a").From("t");

        // Act
        var result = _sut.ValidateQuery(query);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnFalse_When_QueryHasNoSelectColumns()
    {
        // Arrange
        var query = new Query().From("t");

        // Act
        var result = _sut.ValidateQuery(query);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnFalse_When_FromTableIsEmpty()
    {
        // Arrange
        var query = new Query().Select("a");

        // Act
        var result = _sut.ValidateQuery(query);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnTrue_When_WhereEntriesHaveInvalidOperators()
    {
        // Arrange
        var query = new Query().Select("a").From("t").Where("c", "BOGUS OP", 1);

        // Act
        var result = _sut.ValidateQuery(query);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Should_ThrowNullReferenceException_When_QueryIsNull()
    {
        // Act
        var act = () => _sut.ValidateQuery(null!);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}

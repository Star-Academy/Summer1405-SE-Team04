using QueryBuilder.Models;
using QueryBuilder.Utils;

namespace UnitTests.UtilsTests;

public class SqlValidatorTest
{
    private readonly SqlValidator _sut;

    public SqlValidatorTest()
    {
        _sut = new SqlValidator();
    }

    [Fact]
    public void ValidateParams_ShouldReturnTrue_WhenParamsContainItems()
    {
        // Act
        var result = _sut.ValidateParams([1, "a"]);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateParams_ShouldReturnFalse_WhenParamsArrayIsEmpty()
    {
        // Act
        var result = _sut.ValidateParams([]);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateParams_ShouldReturnTrue_WhenParamsContainNullElement()
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
    public void ValidateStringsNotEmpty_ShouldReturnTrue_WhenAllStringsAreValidNames(params string[] inputs)
    {
        // Act
        var result = _sut.ValidateStringsNotEmpty(inputs);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateStringsNotEmpty_ShouldReturnFalse_WhenNoArgsAreProvided()
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
    public void ValidateStringsNotEmpty_ShouldReturnFalse_WhenValueIsBlank(string input)
    {
        // Act
        var result = _sut.ValidateStringsNotEmpty(input);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateStringsNotEmpty_ShouldReturnFalse_WhenElementIsNull()
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
    public void ValidateStringsNotEmpty_ShouldReturnFalse_WhenOneValueIsInvalidAmongValidOnes(params string[] inputs)
    {
        // Act
        var result = _sut.ValidateStringsNotEmpty(inputs);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateStringsNotEmpty_ShouldThrowArgumentNullException_WhenArrayIsNull()
    {
        // Act
        var act = () => _sut.ValidateStringsNotEmpty(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("parameters");
    }

    [Fact]
    public void ValidateQuery_ShouldReturnTrue_WhenQueryHasSelectAndFrom()
    {
        // Arrange
        var query = new Query().Select("a").From("t");

        // Act
        var result = _sut.ValidateQuery(query);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateQuery_ShouldReturnFalse_WhenQueryHasNoSelectColumns()
    {
        // Arrange
        var query = new Query().From("t");

        // Act
        var result = _sut.ValidateQuery(query);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateQuery_ShouldReturnFalse_WhenFromTableIsEmpty()
    {
        // Arrange
        var query = new Query().Select("a");

        // Act
        var result = _sut.ValidateQuery(query);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateQuery_ShouldReturnTrue_WhenWhereEntriesHaveInvalidOperators()
    {
        // Arrange
        var query = new Query().Select("a").From("t").Where("c", "BOGUS OP", 1);

        // Act
        var result = _sut.ValidateQuery(query);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateQuery_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        // Act
        var act = () => _sut.ValidateQuery(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("query");
    }
}
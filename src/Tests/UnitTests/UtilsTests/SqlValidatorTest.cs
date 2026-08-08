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
    public void SqlValidator_ValidateParamsWithItems_ReturnsTrue()
    {
        // Act
        var result = _sut.ValidateParams([1, "a"]);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void SqlValidator_ValidateParamsWithEmptyArray_ReturnsFalse()
    {
        // Act
        var result = _sut.ValidateParams([]);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void SqlValidator_ValidateParamsWithNullElement_ReturnsTrue()
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
    public void SqlValidator_ValidateStringsNotEmptyWithValidNames_ReturnsTrue(params string[] inputs)
    {
        // Act
        var result = _sut.ValidateStringsNotEmpty(inputs);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void SqlValidator_ValidateStringsNotEmptyWithNoArgs_ReturnsFalse()
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
    public void SqlValidator_ValidateStringsNotEmptyWithBlankValue_ReturnsFalse(string input)
    {
        // Act
        var result = _sut.ValidateStringsNotEmpty(input);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void SqlValidator_ValidateStringsNotEmptyWithNullElement_ReturnsFalse()
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
    public void SqlValidator_ValidateStringsNotEmptyWithOneInvalidAmongValid_ReturnsFalse(params string[] inputs)
    {
        // Act
        var result = _sut.ValidateStringsNotEmpty(inputs);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void SqlValidator_ValidateStringsNotEmptyWithNullArray_ThrowsNullReference()
    {
        // Act
        var act = () => _sut.ValidateStringsNotEmpty(null!);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void SqlValidator_ValidateQueryWithSelectAndFrom_ReturnsTrue()
    {
        // Arrange
        var query = new Query().Select("a").From("t");

        // Act
        var result = _sut.ValidateQuery(query);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void SqlValidator_ValidateQueryWithNoSelectColumns_ReturnsFalse()
    {
        // Arrange
        var query = new Query().From("t");

        // Act
        var result = _sut.ValidateQuery(query);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void SqlValidator_ValidateQueryWithEmptyFromTable_ReturnsFalse()
    {
        // Arrange
        var query = new Query().Select("a");

        // Act
        var result = _sut.ValidateQuery(query);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void SqlValidator_ValidateQueryIgnoresWhereEntries_ReturnsTrue()
    {
        // Arrange
        var query = new Query().Select("a").From("t").Where("c", "BOGUS OP", 1);

        // Act
        var result = _sut.ValidateQuery(query);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void SqlValidator_ValidateQueryWithNull_ThrowsNullReference()
    {
        // Act
        var act = () => _sut.ValidateQuery(null!);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}

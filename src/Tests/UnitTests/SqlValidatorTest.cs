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
        var result = _sut.ValidateParams([1, "a"]);

        result.Should().BeTrue();
    }

    [Fact]
    public void SqlValidator_ValidateParamsWithEmptyArray_ReturnsFalse()
    {
        var result = _sut.ValidateParams([]);

        result.Should().BeFalse();
    }

    [Fact]
    public void SqlValidator_ValidateParamsWithNullElement_ReturnsTrue()
    {
        var result = _sut.ValidateParams([null!]);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("a")]
    [InlineData("a", "b")]
    [InlineData("a", "b", "c")]
    public void SqlValidator_ValidateStringsNotEmptyWithValidNames_ReturnsTrue(params string[] inputs)
    {
        var result = _sut.ValidateStringsNotEmpty(inputs);

        result.Should().BeTrue();
    }

    [Fact]
    public void SqlValidator_ValidateStringsNotEmptyWithNoArgs_ReturnsFalse()
    {
        var result = _sut.ValidateStringsNotEmpty();

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
        var result = _sut.ValidateStringsNotEmpty(input);

        result.Should().BeFalse();
    }

    [Fact]
    public void SqlValidator_ValidateStringsNotEmptyWithNullElement_ReturnsFalse()
    {
        var result = _sut.ValidateStringsNotEmpty([null!]);

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("a", "")]
    [InlineData("", "a")]
    [InlineData("a", "\t", "b")]
    public void SqlValidator_ValidateStringsNotEmptyWithOneInvalidAmongValid_ReturnsFalse(params string[] inputs)
    {
        var result = _sut.ValidateStringsNotEmpty(inputs);

        result.Should().BeFalse();
    }

    [Fact]
    public void SqlValidator_ValidateStringsNotEmptyWithNullArray_ThrowsNullReference()
    {
        var act = () => _sut.ValidateStringsNotEmpty(null!);

        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void SqlValidator_ValidateQueryWithSelectAndFrom_ReturnsTrue()
    {
        var query = new Query().Select("a").From("t");

        var result = _sut.ValidateQuery(query);

        result.Should().BeTrue();
    }

    [Fact]
    public void SqlValidator_ValidateQueryWithNoSelectColumns_ReturnsFalse()
    {
        var query = new Query().From("t");

        var result = _sut.ValidateQuery(query);

        result.Should().BeFalse();
    }

    [Fact]
    public void SqlValidator_ValidateQueryWithEmptyFromTable_ReturnsFalse()
    {
        var query = new Query().Select("a");

        var result = _sut.ValidateQuery(query);

        result.Should().BeFalse();
    }

    [Fact]
    public void SqlValidator_ValidateQueryIgnoresWhereEntries_ReturnsTrue()
    {
        var query = new Query().Select("a").From("t").Where("c", "BOGUS OP", 1);

        var result = _sut.ValidateQuery(query);

        result.Should().BeTrue();
    }

    [Fact]
    public void SqlValidator_ValidateQueryWithNull_ThrowsNullReference()
    {
        var act = () => _sut.ValidateQuery(null!);

        act.Should().Throw<NullReferenceException>();
    }
}

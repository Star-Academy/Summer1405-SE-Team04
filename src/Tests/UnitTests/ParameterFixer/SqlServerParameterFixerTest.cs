using QueryBuilder.ParameterFixer;

namespace UnitTests.ParameterFixer;

public class SqlServerParameterFixerTest
{
    private readonly SqlServerParameterFixer _sut = SqlServerParameterFixer.Instance;

    [Theory]
    [InlineData("id", "[id]")]
    [InlineData("Student", "[Student]")]
    public void WrapIdentifier_ShouldWrapInBrackets_WhenIdentifierIsSimple(string identifier, string expected)
    {
        // Arrange

        // Act
        var result = _sut.WrapIdentifier(identifier);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void WrapIdentifier_ShouldWrapEmptyValue_WhenIdentifierIsEmpty()
    {
        // Arrange

        // Act
        var result = _sut.WrapIdentifier("");

        // Assert
        result.Should().Be("[]");
    }

    [Fact]
    public void WrapIdentifier_ShouldNotEscapeEmbeddedBracket_WhenIdentifierContainsClosingBracket()
    {
        // Arrange

        // Act
        var result = _sut.WrapIdentifier("a]b");

        // Assert
        result.Should().Be("[a]b]");
    }

    [Fact]
    public void WrapIdentifier_ShouldPassPayloadThrough_WhenIdentifierContainsSqlInjectionPayload()
    {
        // Arrange

        // Act
        var result = _sut.WrapIdentifier("x]; DROP TABLE y--");

        // Assert
        result.Should().Be("[x]; DROP TABLE y--]");
    }

    [Fact]
    public void WrapIdentifier_ShouldThrowArgumentNullException_WhenIdentifierIsNull()
    {
        // Arrange

        // Act
        var act = () => _sut.WrapIdentifier(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0, "@p0")]
    [InlineData(1, "@p1")]
    [InlineData(42, "@p42")]
    public void FormatParameter_ShouldProduceIndexedPlaceholder_WhenIndexIsNonNegative(int index, string expected)
    {
        // Arrange

        // Act
        var result = _sut.FormatParameter(index);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void FormatParameter_ShouldProduceInvalidPlaceholder_WhenIndexIsNegative()
    {
        // Arrange

        // Act
        var result = _sut.FormatParameter(-1);

        // Assert
        result.Should().Be("@p-1");
    }

    [Fact]
    public void Instance_ShouldReturnSameSingleton_WhenAccessedTwice()
    {
        // Arrange

        // Act
        var first = SqlServerParameterFixer.Instance;
        var second = SqlServerParameterFixer.Instance;

        // Assert
        first.Should().BeSameAs(second);
    }
}

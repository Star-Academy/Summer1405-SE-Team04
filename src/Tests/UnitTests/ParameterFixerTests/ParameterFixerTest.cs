using QueryBuilder.ParameterFixer;

namespace UnitTests;

public class ParameterFixerTest
{
    [Theory]
    [InlineData("id", "\"id\"")]
    [InlineData("Student", "\"Student\"")]
    public void WrapIdentifier_ShouldWrapInDoubleQuotes_WhenUsingPostgresFixer(string identifier, string expected)
    {
        // Act
        var result = PostgresParameterFixer.Instance.WrapIdentifier(identifier);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("id", "[id]")]
    [InlineData("Student", "[Student]")]
    public void WrapIdentifier_ShouldWrapInBrackets_WhenUsingSqlServerFixer(string identifier, string expected)
    {
        // Act
        var result = SqlServerParameterFixer.Instance.WrapIdentifier(identifier);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void WrapIdentifier_ShouldWrapEmptyValue_WhenIdentifierIsEmptyStringInPostgres()
    {
        // Act
        var result = PostgresParameterFixer.Instance.WrapIdentifier("");

        // Assert
        result.Should().Be("\"\"");
    }

    [Fact]
    public void WrapIdentifier_ShouldWrapEmptyValue_WhenIdentifierIsEmptyStringInSqlServer()
    {
        // Act
        var result = SqlServerParameterFixer.Instance.WrapIdentifier("");

        // Assert
        result.Should().Be("[]");
    }

    [Theory]
    [InlineData(0, "@p0")]
    [InlineData(1, "@p1")]
    [InlineData(42, "@p42")]
    public void FormatParameter_ShouldProduceIndexedPlaceholder_WhenUsingPostgresFixer(int index, string expected)
    {
        // Act
        var result = PostgresParameterFixer.Instance.FormatParameter(index);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, "@p0")]
    [InlineData(1, "@p1")]
    [InlineData(42, "@p42")]
    public void FormatParameter_ShouldProduceIndexedPlaceholder_WhenUsingSqlServerFixer(int index, string expected)
    {
        // Act
        var result = SqlServerParameterFixer.Instance.FormatParameter(index);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void FormatParameter_ShouldProduceInvalidPlaceholder_WhenIndexIsNegative()
    {
        // Act
        var result = PostgresParameterFixer.Instance.FormatParameter(-1);

        // Assert
        result.Should().Be("@p-1");
    }

    [Fact]
    public void PostgresParameterFixerInstance_ShouldReturnSameSingleton_WhenAccessingTwice()
    {
        // Act & Assert
        PostgresParameterFixer.Instance.Should().BeSameAs(PostgresParameterFixer.Instance);
    }

    [Fact]
    public void SqlServerParameterFixerInstance_ShouldReturnSameSingleton_WhenAccessingTwice()
    {
        // Act & Assert
        SqlServerParameterFixer.Instance.Should().BeSameAs(SqlServerParameterFixer.Instance);
    }

    [Fact]
    public void WrapIdentifier_ShouldNotEscapeEmbeddedQuote_WhenWrappingIdentifierInPostgres()
    {
        // Act
        var result = PostgresParameterFixer.Instance.WrapIdentifier("a\"b");

        // Assert
        result.Should().Be("\"a\"b\"");
    }

    [Fact]
    public void WrapIdentifier_ShouldNotEscapeEmbeddedBracket_WhenWrappingIdentifierInSqlServer()
    {
        // Act
        var result = SqlServerParameterFixer.Instance.WrapIdentifier("a]b");

        // Assert
        result.Should().Be("[a]b]");
    }

    [Fact]
    public void WrapIdentifier_ShouldPassPayloadThrough_WhenIdentifierContainsSqlInjectionPayload()
    {
        // Act
        var result = PostgresParameterFixer.Instance.WrapIdentifier("x\"; DROP TABLE y--");

        // Assert
        result.Should().Be("\"x\"; DROP TABLE y--\"");
    }
}
